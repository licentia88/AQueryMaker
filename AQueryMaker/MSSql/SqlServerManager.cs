using System.Data;
using System.Data.Common;
using AQueryMaker.Extensions;
using AQueryMaker.Interfaces;
using Microsoft.Data.SqlClient;

namespace AQueryMaker.MSSql;

/// <summary>
/// Represents a SQL Server database manager that extends the <see cref="SqlQueryBuilder"/> class and implements the <see cref="IDatabaseManager"/> interface.
/// </summary>
public class SqlServerManager : SqlQueryBuilder, IDatabaseManager
{

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerManager"/> class with the specified database connection.
    /// </summary>
    /// <param name="dbConnection">The database connection.</param>
    public SqlServerManager(DbConnection dbConnection)
    {
        Connection = dbConnection;
    }

    //public new dbc Connection { get; set; }
    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> InsertAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(tableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(tableName), tableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        bool isIdentity = isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>();

        if (isIdentity)
            if (primaryKeyName != null)
                model.Remove(primaryKeyName);

        var insertStatement = CreateInsertStatement(tableName, model, primaryKeyName, isIdentity);

        var insertResult = await QueryAsync(insertStatement, model.GetAsWhereStatement());

        return insertResult.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> UpdateAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(tableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(tableName), tableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        //bool IsIdentity = (bool)(isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>());

        var updateStatement = CreateUpdateStatement(tableName, model, primaryKeyName);

        var updateResult = await QueryAsync(updateStatement, model.GetAsWhereStatement());

        return updateResult.FirstOrDefault();


    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> DeleteAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(tableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(tableName), tableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        //bool IsIdentity = (bool)(isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>());

        var deleteStatement = CreateDeleteStatement(tableName, primaryKeyName);

        await QueryAsync(deleteStatement, model.GetAsWhereStatement());

        return model;
    }

    /// <inheritdoc/>
    public Task<List<Dictionary<string, object>>> QueryAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return QueryAsync(query, CommandType.Text, whereStatementParameters);

    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> QueryAsync(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        if (Connection is not SqlConnection sqlConnection) throw new InvalidCastException();
        
        var command = sqlConnection.CreateCommand();

        await command.OpenAsync();

        command.CommandText = query;

        command.CommandType = commandType;

        AddWhereStatementParameters(command, whereStatementParameters);

        DbDataReader reader = await command.ExecuteReaderAsync();

        var result = await ExecuteCommandAsync(reader);


        await command.Connection.CloseAsync();

        return result;
    }

    public IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return StreamAsync(query, 100, whereStatementParameters);
    }

    public async IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        var pageIndex = 0;

        var command = Connection.CreateCommand();

        bool hasMoreRows;

        do
        {
            await command.OpenAsync();

            command.CommandText = $" {query}  OFFSET {pageIndex * pageIndex} ROWS  FETCH NEXT {pageIndex} ROWS ONLY "; 

            command.CommandType = CommandType.Text;

            AddWhereStatementParameters(command, whereStatementParameters);

            DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            var result = await ExecuteCommandAsync(reader);

            pageIndex++;

            hasMoreRows = reader.HasRows;

            yield return result;
        }
        while (hasMoreRows);
    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string procedureName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateStoredProcedureFieldMetaDataStatement(procedureName);

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;
 
        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null)
            await command.Connection.CloseAsync();

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> GetMethodParameters(string methodName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateMethodPropertyMetaDataStatement(methodName);

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, (nameof(methodName), methodName));

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetStoredProcedures()
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetStoredProceduresStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetTableFieldsAsync(string tableName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetTableFieldsStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, ("TABLE_NAME", tableName));

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetTableListAsync()
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetTableListStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetStoredProcedureParametersAsync(string storedProcedureName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetStoredProcedureParametersStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, ("SPECIFIC_NAME", storedProcedureName));

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }
}



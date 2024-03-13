using System.Data;
using System.Data.Common;
using AQueryMaker.Extensions;
using AQueryMaker.Interfaces;
using AQueryMaker.MSSql;
using Microsoft.Data.Sqlite;

namespace AQueryMaker.SQLite;

/// <summary>
/// Represents a SQL Server database manager that extends the <see cref="SqLiteQueryBuilder"/> class and implements the <see cref="IDatabaseManager"/> interface.
/// </summary>
public class SqLiteServerManager : SqLiteQueryBuilder, IDatabaseManager
{

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerManager"/> class with the specified database connection.
    /// </summary>
    /// <param name="dbConnection">The database connection.</param>
    public SqLiteServerManager(DbConnection dbConnection)
    {
        Connection = dbConnection;
    }

    //public new dbc Connection { get; set; }
    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> InsertAsync(string TableName, Dictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(TableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(TableName), TableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        bool isIdentity = isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>();

        if (isIdentity)
            if (primaryKeyName != null)
                Model.Remove(primaryKeyName);

        var insertStatement = CreateInsertStatement(TableName, Model, primaryKeyName, isIdentity);

        var insertResult = await QueryAsync(insertStatement, Model.GetAsWhereStatement());

        return insertResult.FirstOrDefault();
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> UpdateAsync(string TableName, Dictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(TableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(TableName), TableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        //bool IsIdentity = (bool)(isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>());

        var updateStatement = CreateUpdateStatement(TableName, Model, primaryKeyName);

        var updateResult = await QueryAsync(updateStatement, Model.GetAsWhereStatement());

        return updateResult.FirstOrDefault();


    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, object>> DeleteAsync(string TableName, Dictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        var isAutoInrementQuery = IsAutoIncrementStatement(TableName);

        var whereStatement = new KeyValuePair<string, object>(nameof(TableName), TableName);

        var isAutoIncrementResult = await QueryAsync(isAutoInrementQuery, CommandType.Text, whereStatement);

        string primaryKeyName = isAutoIncrementResult.First()["PrimaryKeyName"].ToString();

        //bool IsIdentity = (bool)(isAutoIncrementResult.First()["IS_IDENTITY"].CastTo<bool>());

        var deleteStatement = CreateDeleteStatement(TableName, primaryKeyName);

        await QueryAsync(deleteStatement, Model.GetAsWhereStatement());

        return Model;
    }

    /// <inheritdoc/>
    public Task<List<Dictionary<string, object>>> QueryAsync(string Query, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return QueryAsync(Query, CommandType.Text, WhereStatementParameters);

    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> QueryAsync(string Query, CommandType CommandType,
        params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        if (Connection is not SqliteConnection sqlConnection) throw new InvalidCastException();
        
        var command = sqlConnection.CreateCommand();

        await command.OpenAsync();

        command.CommandText = Query;

        command.CommandType = CommandType;

        AddWhereStatementParameters(command, WhereStatementParameters);

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

            command.CommandText = $" {query}  OFFSET {pageIndex * itemPerPage} ROWS  FETCH NEXT {itemPerPage} ROWS ONLY ";

            command.CommandType = CommandType.Text;

            AddWhereStatementParameters(command, whereStatementParameters);

            DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            var result = await ExecuteCommandAsync(reader);

            pageIndex++;

            hasMoreRows = reader.HasRows;

            await Connection.CloseAsync();

            yield return result;
        }
        while (hasMoreRows);
    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string ProcedureName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateStoredProcedureFieldMetaDataStatement(ProcedureName);

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;
 
        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null)
            await command.Connection.CloseAsync();

        return result;
    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> GetMethodParameters(string MethodName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateMethodPropertyMetaDataStatement(MethodName);

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, (nameof(MethodName), MethodName));

        DbDataReader reader = command.ExecuteReader(CommandBehavior.Default);

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

        DbDataReader reader = command.ExecuteReader(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetTableFieldsAsync(string TableName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetTableFieldsStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, ("TABLE_NAME", TableName));

        DbDataReader reader = command.ExecuteReader(CommandBehavior.Default);

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

        DbDataReader reader = command.ExecuteReader(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetStoredProcedureParametersAsync(string StoredProcedureName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetStoredProcedureParametersStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        AddWhereStatementParameters(command, ("SPECIFIC_NAME", StoredProcedureName));

        DbDataReader reader = command.ExecuteReader(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }
}



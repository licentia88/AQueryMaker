using System.Data;
using System.Data.Common;
using AQueryMaker.Extensions;
using AQueryMaker.Interfaces;
using Mapster;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace AQueryMaker.MSSql;

 
public class SqlServerManager : SqlQueryBuilder, IDatabaseManager
{
    public int TimeOut { get; set; }

   
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

        command.CommandTimeout = TimeOut;

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
        command.CommandTimeout = TimeOut;

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
    public async Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string procedureName)
    {
        var command = Connection.CreateCommand();
        command.CommandTimeout = TimeOut;

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
 
    public Task<DbDataReader> QueryReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return QueryReaderAsync(query, CommandType.Text, whereStatementParameters);
    }

    public async Task<DbDataReader> QueryReaderAsync(string query, CommandType commandType, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        if (Connection is not SqlConnection sqlConnection) throw new InvalidCastException();

        var command = sqlConnection.CreateCommand();

        command.CommandTimeout = TimeOut;

        await command.OpenAsync();

        command.CommandText = query;

        command.CommandType = commandType;

        AddWhereStatementParameters(command, whereStatementParameters);

        return await command.ExecuteReaderAsync();

     }

   

    public IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return StreamReaderAsync(query, 100, whereStatementParameters);
    }

    public async IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        var pageIndex = 0;

        var command = Connection.CreateCommand();
        command.CommandTimeout = TimeOut;

        bool hasMoreRows;

        do
        {
            await Connection.CloseAsync();

            await command.OpenAsync();

            command.CommandText = $" {query}  OFFSET {pageIndex * itemPerPage} ROWS  FETCH NEXT {itemPerPage} ROWS ONLY ";

            command.CommandType = CommandType.Text;

            AddWhereStatementParameters(command, whereStatementParameters);

            DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            //var result = await ExecuteCommandAsync(reader);

            pageIndex++;

            hasMoreRows = reader.HasRows;

            yield return reader;
            //await Connection.CloseAsync();

            //yield return result;
        }
        while (hasMoreRows);
    }

    public Task<List<TModel>> QueryAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return QueryAsync<TModel>(query, CommandType.Text, whereStatementParameters);

    }

    /// <inheritdoc/>
    public async Task<List<TModel>> QueryAsync<TModel>(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        if (Connection is not SqlConnection sqlConnection) throw new InvalidCastException();

        var command = sqlConnection.CreateCommand();
        command.CommandTimeout = TimeOut;

        await command.OpenAsync();

        command.CommandText = query;

        command.CommandType = commandType;

        AddWhereStatementParameters(command, whereStatementParameters);

        DbDataReader reader = await command.ExecuteReaderAsync();

        var result = await ExecuteCommandAsync(reader);


        await command.Connection.CloseAsync();

        return result.Adapt<List<TModel>>();
    }

    public IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return StreamAsync<TModel>(query, 100, whereStatementParameters);
    }

    public async IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        var pageIndex = 0;

        var command = Connection.CreateCommand();
        command.CommandTimeout = TimeOut;

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

            yield return result.Adapt<List<TModel>>();
        }
        while (hasMoreRows);
    }

}



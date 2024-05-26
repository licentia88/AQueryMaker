using System.Data;
using System.Data.Common;
using AQueryMaker.Interfaces;

namespace AQueryMaker;

public class QueryManager : IDatabaseManager
{
    private readonly IDatabaseManager _manager;

    public DbConnection Connection => _manager.Connection;

     public int TimeOut
    {
        get => _manager.TimeOut;
        set => _manager.TimeOut = value;
    }

    public QueryManager(IDatabaseManager manager)
    {
        _manager = manager;
    }

    public void SetConnectionTimeOut(int Seconds)
    {
        TimeOut = Seconds;
    }
    /// <summary>
    /// Asynchronously inserts a new record into the specified table using the provided model.
    /// </summary>
    public Task<Dictionary<string, object>> InsertAsync(string tableName, Dictionary<string, object> model,
        CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        return _manager.InsertAsync(tableName, model, commandBehavior);
    }

    /// <summary>
    /// Asynchronously deletes records from the specified table based on the provided model.
    /// </summary>
    public Task<Dictionary<string, object>> DeleteAsync(string tableName, Dictionary<string, object> model,
        CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        return _manager.DeleteAsync(tableName, model, commandBehavior);
    }

    /// <summary>
    /// Asynchronously updates records in the specified table using the provided model.
    /// </summary>
    public Task<Dictionary<string, object>> UpdateAsync(string tableName, Dictionary<string, object> model,
        CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        return _manager.UpdateAsync(tableName, model, commandBehavior);
    }


    /// <summary>
    /// Asynchronously executes a query with optional where statement parameters and returns the result set as a list of dictionaries.
    /// </summary>
    public Task<List<Dictionary<string, object>>> QueryAsync(string query,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryAsync(query, whereStatementParameters);
    }

    /// <summary>
    /// Asynchronously executes a query with optional where statement parameters and command behavior, command type, and returns the result set as a list of dictionaries.
    /// </summary>
    public Task<List<Dictionary<string, object>>> QueryAsync(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryAsync(query, commandType, whereStatementParameters);
    }

    public Task<DbDataReader> QueryReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryReaderAsync(query, whereStatementParameters);
    }

    public Task<DbDataReader> QueryReaderAsync(string query, CommandType commandType, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryReaderAsync(query, commandType, whereStatementParameters);
    }

    /// <summary>
    /// Asynchronously retrieves metadata for a stored procedure's fields.
    /// Dictionary Keys
    /// NAME, SYSTEM_TYPE_NAME
    /// </summary>
    public Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string procedureName)
    {
        return _manager.GetStoredProcedureFieldsAsync(procedureName);
    }

    /// <summary>
    /// Asynchronously retrieves metadata for a method's parameters.
    /// </summary>
    public Task<List<Dictionary<string, object>>> GetMethodParameters(string methodName)
    {
        return _manager.GetMethodParameters(methodName);
    }

    public IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamAsync(query, whereStatementParameters);
    }

    public IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, int itemPerPage,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamAsync(query, itemPerPage, whereStatementParameters);
    }

    /// <summary>
    /// Retrieves list of Stored Procedures
    /// Dictionary Keys : SP_NAME
    /// </summary>
    /// <returns></returns>
    public Task<List<Dictionary<string, object>>> GetStoredProcedures()
    {
        return _manager.GetStoredProcedures();
    }

    public Task<List<Dictionary<string, object>>> GetTableFieldsAsync(string tableName)
    {
        return _manager.GetTableFieldsAsync(tableName);
    }

    public Task<List<Dictionary<string, object>>> GetTableListAsync()
    {
        return _manager.GetTableListAsync();
    }

    public Task<List<Dictionary<string, object>>> GetStoredProcedureParametersAsync(string storedProcedureName)
    {
        return _manager.GetStoredProcedureParametersAsync(storedProcedureName);
    }

    public Task<List<TModel>> QueryAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryAsync<TModel>(query,whereStatementParameters);

    }

    public Task<List<TModel>> QueryAsync<TModel>(string query, CommandType commandType, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.QueryAsync<TModel>(query,commandType, whereStatementParameters);
    }

    public IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamReaderAsync(query, whereStatementParameters);

    }

    public IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamReaderAsync(query, itemPerPage, whereStatementParameters);
    }

    public IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamAsync<TModel>(query, whereStatementParameters);
    }

    public IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return _manager.StreamAsync<TModel>(query,itemPerPage, whereStatementParameters);
    }
}
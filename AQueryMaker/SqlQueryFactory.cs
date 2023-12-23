using System.Data;
using System.Data.Common;
using AQueryMaker.Interfaces;

namespace AQueryMaker;

public class SqlQueryFactory : IDatabaseManager
{
    private readonly IDatabaseManager Manager;

    public DbConnection Connection => Manager.Connection;

    public SqlQueryFactory(IDatabaseManager manager)
    {
        Manager = manager;
    }

    /// <summary>
    /// Asynchronously inserts a new record into the specified table using the provided model.
    /// </summary>
    public Task<IDictionary<string, object>> InsertAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.InsertAsync(TableName, Model, CommandBehavior);
    }

    /// <summary>
    /// Asynchronously deletes records from the specified table based on the provided model.
    /// </summary>
    public Task<IDictionary<string, object>> DeleteAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.DeleteAsync(TableName, Model, CommandBehavior);
    }

    /// <summary>
    /// Asynchronously updates records in the specified table using the provided model.
    /// </summary>
    public Task<IDictionary<string, object>> UpdateAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.UpdateAsync(TableName, Model, CommandBehavior);
    }

   

    /// <summary>
    /// Asynchronously executes a query with optional where statement parameters and returns the result set as a list of dictionaries.
    /// </summary>
    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, WhereStatementParameters);
    }

    /// <summary>
    /// Asynchronously executes a query with optional where statement parameters and command behavior, command type, and returns the result set as a list of dictionaries.
    /// </summary>
    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, CommandType CommandType,
        params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, CommandType, WhereStatementParameters);
    }

    /// <summary>
    /// Asynchronously retrieves metadata for a stored procedure's fields.
    /// Dictionary Keys
    /// NAME, SYSTEM_TYPE_NAME
    /// </summary>
    public Task<List<IDictionary<string, object>>> GetStoredProcedureFieldsAsync(string ProcedureName)
    {
        return Manager.GetStoredProcedureFieldsAsync(ProcedureName);
    }

    /// <summary>
    /// Asynchronously retrieves metadata for a method's parameters.
    /// </summary>
    public Task<List<IDictionary<string, object>>> GetMethodParameters(string MethodName)
    {
        return Manager.GetMethodParameters(MethodName);
    }

    public IAsyncEnumerable<List<IDictionary<string, object>>> StreamAsync(string Query, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.StreamAsync(Query, WhereStatementParameters);
    }

    public IAsyncEnumerable<List<IDictionary<string, object>>> StreamAsync(string Query, int ItemPerPage, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.StreamAsync(Query, ItemPerPage, WhereStatementParameters);
    }

    /// <summary>
    /// Retrieves list of Stored Procedures
    /// Dictionary Keys : SP_NAME
    /// </summary>
    /// <returns></returns>
    public Task<List<IDictionary<string, object>>> GetStoredProcedures()
    {
        return Manager.GetStoredProcedures();
    }

    public Task<List<IDictionary<string, object>>> GetTableFieldsAsync(string TableName)
    {
        return Manager.GetTableFieldsAsync(TableName);
    }

    public Task<List<IDictionary<string, object>>> GetTableListAsync()
    {
        return Manager.GetTableListAsync();

    }

    public Task<List<IDictionary<string, object>>> GetStoredProcedureParametersAsync(string StoredProcedureName)
    {
        return Manager.GetStoredProcedureParametersAsync(StoredProcedureName);
    }
}



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

    public Task<IDictionary<string, object>> InsertAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.InsertAsync(TableName, Model, CommandBehavior);
    }

    public Task<IDictionary<string, object>> DeleteAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.DeleteAsync(TableName, Model, CommandBehavior);
    }

    public Task<IDictionary<string, object>> UpdateAsync(string TableName, IDictionary<string, object> Model, CommandBehavior CommandBehavior = CommandBehavior.Default)
    {
        return Manager.UpdateAsync(TableName, Model, CommandBehavior);
    }

    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, params (string Key, object Value)[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, WhereStatementParameters);
    }

    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, CommandBehavior CommandBehavior, CommandType CommandType, params (string Key, object Value)[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, CommandBehavior, CommandType, WhereStatementParameters);
    }
    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, WhereStatementParameters);
    }

    public Task<List<IDictionary<string, object>>> QueryAsync(string Query, CommandBehavior CommandBehavior, CommandType CommandType, params KeyValuePair<string, object>[] WhereStatementParameters)
    {
        return Manager.QueryAsync(Query, CommandBehavior, CommandType, WhereStatementParameters);
    }

    public Task<List<IDictionary<string, object>>> GetStoredProcedureFieldMetaDataAsync(string ProcedureName)
    {
        return Manager.GetStoredProcedureFieldMetaDataAsync(ProcedureName);
    }

    public Task<List<IDictionary<string, object>>> GetMethodParameters(string MethodName)
    {
        return Manager.GetMethodParameters(MethodName);
    }

    
}



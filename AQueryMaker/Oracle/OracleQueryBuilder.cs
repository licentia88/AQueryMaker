using System.Data.Common;
using AQueryMaker.Interfaces;

namespace AQueryMaker.Oracle;

public class OracleQueryBuilder : DatabaseManager, IQueryStringBuilder
{
    public string CreateDeleteStatement(string tableName, string primaryKey)
    {
        throw new NotImplementedException();
    }

    string UserId()
    {
        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
        builder.ConnectionString = Connection.ConnectionString;

        return builder["User Id"].ToString();
    }

    public string CreateInsertStatement(string tableName, Dictionary<string, object> model, string primaryKey, bool isAutoIncrement)
    {
        throw new NotImplementedException();
    }

    public string CreateStoredProcedureFieldMetaDataStatement(string storedProcedure)
    {
        throw new NotImplementedException();
    }

    public string CreateUpdateStatement(string tableName, Dictionary<string, object> model, string primaryKey)
    {
        throw new NotImplementedException();
    }

    public string IsAutoIncrementStatement(string tableName)
    {
        throw new NotImplementedException();
    }

   
}



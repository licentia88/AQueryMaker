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
    public string CreateGetStoredProceduresStatement()
    {
        
        return $@"
                SELECT OBJECT_NAME AS SP_NAME
                  FROM ALL_OBJECTS
                  JOIN ALL_USERS
                    ON owner = USERNAME
                 WHERE object_type = 'FUNCTION'
                   AND USERNAME = '{UserId()}'
                   AND STATUS = 'VALID'
                ".Trim();
    }

    public string CreateGetTableFieldsStatement()
    {
        throw new NotImplementedException();
    }

    public string CreateGetTableListStatement()
    {
        return $"SELECT TABLE_NAME FROM SYS.ALL_TABLES WHERE OWNER = '{UserId()}' ";
    }

    public string CreateGetStoredProcedureParametersStatement()
    {
        return $@"SELECT ARGUMENT_NAME AS PARAMETER_NAME 
                  FROM ALL_ARGUMENTS 
                  WHERE OBJECT_NAME =@OBJECT_NAME 
                  AND OWNER = '{UserId()}'
                  AND ARGUMENT_NAME IS NOT NULL 
                  ORDER BY POSITION".Trim();
    }

    public string CreateGetStoredProcedureParametersStatement(string procedureName)
    {
        return $@"SELECT ARGUMENT_NAME AS PARAMETER_NAME 
                  FROM ALL_ARGUMENTS 
                  WHERE OBJECT_NAME = '{procedureName}'
                  AND OWNER = '{UserId()}'
                  AND ARGUMENT_NAME IS NOT NULL 
                  ORDER BY POSITION".Trim();
    }

    public string CreateInsertStatement(string tableName, Dictionary<string, object> model, string primaryKey, bool isAutoIncrement)
    {
        throw new NotImplementedException();
    }

    public string CreateMethodPropertyMetaDataStatement(string methodName)
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



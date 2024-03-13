using AQueryMaker.Interfaces;

namespace AQueryMaker.SQLite;

public class SqLiteQueryBuilder : DatabaseManager, IQueryStringBuilder
{
    /// <summary>
    /// Creates an SQL DELETE statement for deleting a record from the specified table.
    /// </summary>
    /// <param name="TableName">The name of the table.</param>
    /// <param name="PrimaryKey">The primary key column name.</param>
    /// <returns>The generated SQL DELETE statement.</returns>
    public string CreateDeleteStatement(string TableName, string PrimaryKey)
    {
        var whereStatememt = $"WHERE [{PrimaryKey}] = @{PrimaryKey}";

        var query = $"DELETE FROM [{TableName}] {whereStatememt}";

        return query;
    }

    /// <summary>
    /// Creates an SQL INSERT statement for inserting a record into the specified table.
    /// </summary>
    /// <param name="TableName">The name of the table.</param>
    /// <param name="Model">The model or parameters to insert.</param>
    /// <param name="PrimaryKey">The primary key column name.</param>
    /// <param name="IsAutoIncrement">Specifies whether the primary key is auto-incrementing.</param>
    /// <returns>The generated SQL INSERT statement.</returns>
    public string CreateInsertStatement(string TableName, Dictionary<string, object> Model, string PrimaryKey, bool IsAutoIncrement)
    {
        var fields = Model.Select(x => x.Key).ToList();

        var fieldsString = string.Join(", ", fields.Select(x => $"[{x}]"));

        var valueString = string.Join(", ", fields.Select(x => $"@{x}"));

        var query = $"SET NOCOUNT ON;\n\n INSERT INTO [{TableName}] ({fieldsString}) VALUES ({valueString}) ";

        var whereClause = IsAutoIncrement ? $" [{PrimaryKey}] = SCOPE_IDENTITY() " : $" [{PrimaryKey}] = @{PrimaryKey} ";

        var selectQuery = $" \n SELECT [{PrimaryKey}], {fieldsString} FROM {TableName} WHERE @@ROWCOUNT = 1 AND {whereClause}  ";

        return query + selectQuery;
    }

    /// <summary>
    /// Creates an SQL statement for retrieving the property metadata of the specified method.
    /// </summary>
    /// <param name="MethodName">The name of the method.</param>
    /// <returns>The generated SQL statement.</returns>
    public string CreateMethodPropertyMetaDataStatement(string MethodName)
    {
        return $"SELECT P.NAME AS PARAMETER_NAME FROM SYS.PARAMETERS P INNER JOIN SYS.OBJECTS O ON O.OBJECT_ID = P.OBJECT_ID WHERE O.TYPE IN ('FN', 'IF', 'TF', 'P') AND O.NAME = @{nameof(MethodName)} ";
    }

    /// <summary>
    /// Creates an SQL statement for retrieving the field metadata of the specified stored procedure.
    /// </summary>
    /// <param name="StoredProcedure">The name of the stored procedure.</param>
    /// <returns>The generated SQL statement.</returns>
    public string CreateStoredProcedureFieldMetaDataStatement(string StoredProcedure)
    {
        return $"SELECT NAME, SYSTEM_TYPE_NAME FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET('EXEC {StoredProcedure}', NULL, 0)";
    }

    /// <summary>
    /// Creates an SQL statement for retrieving the list of user defined stored procedures.
    /// </summary>
    /// <returns>The generated SQL statement.</returns>
    public string CreateGetStoredProceduresStatement()
    {
        return $"SELECT NAME AS SP_NAME FROM SYS.PROCEDURES WHERE TYPE = 'P' AND IS_MS_SHIPPED = 0 ORDER BY NAME;";
    }

    /// <summary>
    /// Creates an SQL UPDATE statement for updating an existing record in the specified table.
    /// </summary>
    /// <param name="TableName">The name of the table.</param>
    /// <param name="Model">The model or parameters to update.</param>
    /// <param name="PrimaryKey">The primary key column name.</param>
    /// <returns>The generated SQL UPDATE statement.</returns>
    public string CreateUpdateStatement(string TableName, Dictionary<string, object> Model, string PrimaryKey)
    {
        var fields = Model.Where(x => x.Key is not null && !x.Key.Equals(PrimaryKey)).Select(x => x.Key).ToList();

        var fieldsString = string.Join(", ", fields.Select(x => $"[{x}]"));

        var setFields = fields.Select(x => $" [{x}] = @{x} ").ToList();

        var setString = "SET " + string.Join(", ", setFields);

        var whereStatememt = $"WHERE [{PrimaryKey}] = @{PrimaryKey} ";

        var query = $"UPDATE [{TableName}] \n {setString} \n {whereStatememt} ; \n";

        var selectQuery = $" SELECT {PrimaryKey}, {fieldsString} FROM {TableName} \n {whereStatememt} ; \n";

        return query + selectQuery;
    }


    /// <summary>
    /// Creates an SQL statement for checking if the specified table has an auto-increment primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The generated SQL statement.</returns>
    public string IsAutoIncrementStatement(string tableName)
    {
        return $"SELECT C.NAME PrimaryKeyName, C.IS_IDENTITY  FROM SYS.COLUMNS C JOIN SYS.INDEX_COLUMNS IC ON C.OBJECT_ID = IC.OBJECT_ID AND C.COLUMN_ID = IC.COLUMN_ID JOIN SYS.INDEXES I ON IC.OBJECT_ID = I.OBJECT_ID AND IC.INDEX_ID = I.INDEX_ID WHERE I.IS_PRIMARY_KEY = 1 AND I.OBJECT_ID = OBJECT_ID(@{nameof(tableName)})\n";
    }

    public string CreateGetTableFieldsStatement()
    {
        return "SELECT COLUMN_NAME  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TABLE_NAME";
    }

    public string CreateGetTableListStatement()
    {
        return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY  TABLE_NAME ASC ";

    }

    public string CreateGetStoredProcedureParametersStatement()
    {
        return @"SELECT SUBSTRING(PARAMETER_NAME,2,LEN(PARAMETER_NAME)) PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS
                      WHERE SPECIFIC_NAME = @SPECIFIC_NAME
                        ORDER BY ORDINAL_POSITION";
    }

    public string CreateGetStoredProcedureParametersStatement(string procedureName)
    {
        throw new NotImplementedException();
    }
}
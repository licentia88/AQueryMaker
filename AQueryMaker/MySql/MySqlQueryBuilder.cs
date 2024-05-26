using AQueryMaker.Interfaces;

namespace AQueryMaker.MySql;

public class MySqlQueryBuilder : DatabaseManager, IQueryStringBuilder
{
    /// <summary>
    /// Creates an SQL DELETE statement for deleting a record from the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <returns>The generated SQL DELETE statement.</returns>
    public string CreateDeleteStatement(string tableName, string primaryKey)
    {
        var whereStatememt = $"WHERE [{primaryKey}] = @{primaryKey}";

        var query = $"DELETE FROM [{tableName}] {whereStatememt}";

        return query;
    }

    /// <summary>
    /// Creates an SQL INSERT statement for inserting a record into the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to insert.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <param name="isAutoIncrement">Specifies whether the primary key is auto-incrementing.</param>
    /// <returns>The generated SQL INSERT statement.</returns>
    public string CreateInsertStatement(string tableName, Dictionary<string, object> model, string primaryKey, bool isAutoIncrement)
    {
        var fields = model.Select(x => x.Key).ToList();

        var fieldsString = string.Join(", ", fields.Select(x => $"[{x}]"));

        var valueString = string.Join(", ", fields.Select(x => $"@{x}"));

        var query = $"SET NOCOUNT ON;\n\n INSERT INTO [{tableName}] ({fieldsString}) VALUES ({valueString}) ";

        var whereClause = isAutoIncrement ? $" [{primaryKey}] = SCOPE_IDENTITY() " : $" [{primaryKey}] = @{primaryKey} ";

        var selectQuery = $" \n SELECT [{primaryKey}], {fieldsString} FROM {tableName} WHERE @@ROWCOUNT = 1 AND {whereClause}  ";

        return query + selectQuery;
    }

    /// <summary>
    /// Creates an SQL statement for retrieving the property metadata of the specified method.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>The generated SQL statement.</returns>
    public string CreateMethodPropertyMetaDataStatement(string methodName)
    {
        return $"SELECT P.NAME AS PARAMETER_NAME FROM SYS.PARAMETERS P INNER JOIN SYS.OBJECTS O ON O.OBJECT_ID = P.OBJECT_ID WHERE O.TYPE IN ('FN', 'IF', 'TF', 'P') AND O.NAME = @{nameof(methodName)} ";
    }

    /// <summary>
    /// Creates an SQL statement for retrieving the field metadata of the specified stored procedure.
    /// </summary>
    /// <param name="storedProcedure">The name of the stored procedure.</param>
    /// <returns>The generated SQL statement.</returns>
    public string CreateStoredProcedureFieldMetaDataStatement(string storedProcedure)
    {
        return $"SELECT NAME, SYSTEM_TYPE_NAME FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET('EXEC {storedProcedure}', NULL, 0)";
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
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to update.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <returns>The generated SQL UPDATE statement.</returns>
    public string CreateUpdateStatement(string tableName, Dictionary<string, object> model, string primaryKey)
    {
        var fields = model.Where(x => x.Key is not null && !x.Key.Equals(primaryKey)).Select(x => x.Key).ToList();

        var fieldsString = string.Join(", ", fields.Select(x => $"[{x}]"));

        var setFields = fields.Select(x => $" [{x}] = @{x} ").ToList();

        var setString = "SET " + string.Join(", ", setFields);

        var whereStatememt = $"WHERE [{primaryKey}] = @{primaryKey} ";

        var query = $"UPDATE [{tableName}] \n {setString} \n {whereStatememt} ; \n";

        var selectQuery = $" SELECT {primaryKey}, {fieldsString} FROM {tableName} \n {whereStatememt} ; \n";

        return query + selectQuery;
    }


    /// <summary>
    /// Creates an SQL statement for checking if the specified table has an auto-increment primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The generated SQL statement.</returns>
    public string IsAutoIncrementStatement(string tableName)
    {
        return $"SELECT COLUMN_NAME AS PrimaryKeyName, IF(EXTRA LIKE '%auto_increment%', 1, 0) AS IS_IDENTITY " +
               $"FROM INFORMATION_SCHEMA.COLUMNS " +
               $"WHERE TABLE_SCHEMA = DATABASE() " +
               $"AND TABLE_NAME = '{tableName}' " +
               $"AND COLUMN_KEY = 'PRI';";
    }

    public string CreateGetTableFieldsStatement()
    {
        return "SELECT COLUMN_NAME " +
               "FROM INFORMATION_SCHEMA.COLUMNS " +
               "WHERE TABLE_NAME = @TABLE_NAME " +
               "AND TABLE_SCHEMA = DATABASE(); ";

    }

    public string CreateGetTableListStatement()
    {
        return "SELECT TABLE_NAME " +
               "FROM INFORMATION_SCHEMA.TABLES " +
               "WHERE TABLE_TYPE = 'BASE TABLE' " +
               "AND TABLE_SCHEMA = DATABASE() " +
               "ORDER BY TABLE_NAME ASC; ";

    }

    public string CreateGetStoredProcedureParametersStatement()
    {
        return @"SELECT SUBSTRING(PARAMETER_NAME,2,LEN(PARAMETER_NAME)) PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS
                      WHERE SPECIFIC_NAME = @SPECIFIC_NAME
                        ORDER BY ORDINAL_POSITION";
    }

    public string CreateGetStoredProcedureParametersStatement(string procedureName)
    {
        return @$"SELECT SUBSTRING(PARAMETER_NAME,2,LEN(PARAMETER_NAME)) PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS
                      WHERE SPECIFIC_NAME = '{procedureName}'
                        ORDER BY ORDINAL_POSITION";
    }
}
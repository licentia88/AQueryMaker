using AQueryMaker.Interfaces;

namespace AQueryMaker.MSSql;

public class SqlQueryBuilder : DatabaseManager, IQueryStringBuilder
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
    /// Creates an SQL statement for retrieving the field metadata of the specified stored procedure.
    /// </summary>
    /// <param name="storedProcedure">The name of the stored procedure.</param>
    /// <returns>The generated SQL statement.</returns>
    public string CreateStoredProcedureFieldMetaDataStatement(string storedProcedure)
    {
        return $"SELECT NAME, SYSTEM_TYPE_NAME FROM SYS.DM_EXEC_DESCRIBE_FIRST_RESULT_SET('EXEC {storedProcedure}', NULL, 0)";
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
        return $"SELECT C.NAME PrimaryKeyName, C.IS_IDENTITY  FROM SYS.COLUMNS C JOIN SYS.INDEX_COLUMNS IC ON C.OBJECT_ID = IC.OBJECT_ID AND C.COLUMN_ID = IC.COLUMN_ID JOIN SYS.INDEXES I ON IC.OBJECT_ID = I.OBJECT_ID AND IC.INDEX_ID = I.INDEX_ID WHERE I.IS_PRIMARY_KEY = 1 AND I.OBJECT_ID = OBJECT_ID(@{nameof(tableName)})\n";
    }
}
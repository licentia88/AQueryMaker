namespace AQueryMaker.Interfaces;

/// <summary>
/// Represents a query string builder interface for generating SQL statements.
/// </summary>
public interface IQueryStringBuilder
{
    /// <summary>
    /// Creates an SQL INSERT statement for inserting a record into the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to insert.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <param name="isAutoIncrement">Specifies whether the primary key is auto-incrementing.</param>
    /// <returns>The generated SQL INSERT statement.</returns>
    string CreateInsertStatement(string tableName, Dictionary<string, object> model, string primaryKey, bool isAutoIncrement);

    /// <summary>
    /// Creates an SQL UPDATE statement for updating an existing record in the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to update.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <returns>The generated SQL UPDATE statement.</returns>
    string CreateUpdateStatement(string tableName, Dictionary<string, object> model, string primaryKey);

    /// <summary>
    /// Creates an SQL DELETE statement for deleting a record from the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="primaryKey">The primary key column name.</param>
    /// <returns>The generated SQL DELETE statement.</returns>
    string CreateDeleteStatement(string tableName, string primaryKey);

    /// <summary>
    /// Creates an SQL statement for checking if the specified table has an auto-increment primary key.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The generated SQL statement.</returns>
    string IsAutoIncrementStatement(string tableName);

    /// <summary>
    /// Creates an SQL statement for retrieving the field metadata of the specified stored procedure.
    /// </summary>
    /// <param name="storedProcedure">The name of the stored procedure.</param>
    /// <returns>The generated SQL statement.</returns>
    string CreateStoredProcedureFieldMetaDataStatement(string storedProcedure);

    /// <summary>
    /// Creates an SQL statement for retrieving the property metadata of the specified method.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>The generated SQL statement.</returns>
    string CreateMethodPropertyMetaDataStatement(string methodName);

    string CreateGetStoredProceduresStatement();

    string CreateGetTableFieldsStatement();
    string CreateGetTableListStatement();

    string CreateGetStoredProcedureParametersStatement();
    
}

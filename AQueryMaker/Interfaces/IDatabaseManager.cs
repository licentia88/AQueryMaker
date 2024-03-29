﻿using System.Data;
using System.Data.Common;

namespace AQueryMaker.Interfaces;

/// <summary>
/// Represents a database manager interface for executing common database operations.
/// </summary>
public interface IDatabaseManager
{
    /// <summary>
    /// Gets the database connection.
    /// </summary>
    DbConnection Connection { get; }

    /// <summary>
    /// Inserts a new record into the specified table asynchronously.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to insert.</param>
    /// <param name="commandBehavior">The command behavior. Default is CommandBehavior.Default.</param>
    /// <returns>A task representing the asynchronous operation. The inserted record as a dictionary.</returns>
    Task<Dictionary<string, object>> InsertAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default);

    /// <summary>
    /// Updates an existing record in the specified table asynchronously.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to update.</param>
    /// <param name="commandBehavior">The command behavior. Default is CommandBehavior.Default.</param>
    /// <returns>A task representing the asynchronous operation. The updated record as a dictionary.</returns>
    Task<Dictionary<string, object>> UpdateAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default);

    /// <summary>
    /// Deletes a record from the specified table asynchronously.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="model">The model or parameters to delete.</param>
    /// <param name="commandBehavior">The command behavior. Default is CommandBehavior.Default.</param>
    /// <returns>A task representing the asynchronous operation. The deleted record as a dictionary.</returns>
    Task<Dictionary<string, object>> DeleteAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default);

    ///// <summary>
    ///// Executes a database query asynchronously with the specified WHERE statement parameters.
    ///// </summary>
    ///// <param name="Query">The SQL query.</param>
    ///// <param name="WhereStatementParameters">The WHERE statement parameters.</param>
    ///// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    //Task<List<Dictionary<string, object>>> QueryAsync(string Query, params (string Key, object Value)[] WhereStatementParameters);

    ///// <summary>
    ///// Executes a database query asynchronously with the specified command behavior, command type, and WHERE statement parameters.
    ///// </summary>
    ///// <param name="Query">The SQL query.</param>
    ///// <param name="CommandBehavior">The command behavior.</param>
    ///// <param name="CommandType">The command type.</param>
    ///// <param name="WhereStatementParameters">The WHERE statement parameters.</param>
    ///// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    //Task<List<Dictionary<string, object>>> QueryAsync(string Query, CommandBehavior CommandBehavior, CommandType CommandType, params (string Key, object Value)[] WhereStatementParameters);

    /// <summary>
    /// Executes a database query asynchronously with the specified WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    Task<List<Dictionary<string, object>>> QueryAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query asynchronously with the specified command behavior, command type, and WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="commandType">The command type.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    Task<List<Dictionary<string, object>>> QueryAsync(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query with streaming asynchronously with the specified WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query with streaming asynchronously with the specified command behavior, command type, and WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="itemPerPage"></param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query,int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Retrieves the field metadata of a stored procedure asynchronously.
    /// </summary>
    /// <param name="procedureName">The name of the stored procedure.</param>
    /// <returns>A task representing the asynchronous operation. A list of field metadata as dictionaries.</returns>
    Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string procedureName);

    /// <summary>
    /// Retrieves list of Stored Procedures
    /// </summary>
    /// <returns></returns>
    Task<List<Dictionary<string, object>>> GetStoredProcedures();

    /// <summary>
    /// Retrieves the method parameters asynchronously.
    /// </summary>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A task representing the asynchronous operation. A list of method parameters as dictionaries.</returns>
    Task<List<Dictionary<string, object>>> GetMethodParameters(string methodName);

    Task<List<Dictionary<string, object>>> GetTableFieldsAsync(string tableName);

    Task<List<Dictionary<string, object>>> GetTableListAsync();

    Task<List<Dictionary<string, object>>> GetStoredProcedureParametersAsync(string storedProcedureName);
    

}




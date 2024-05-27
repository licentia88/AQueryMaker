using System.Data;
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

    public int TimeOut { get; set; }

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
    /// Executes a database query asynchronously with the specified WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    Task<List<TModel>> QueryAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query asynchronously with the specified command behavior, command type, and WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="commandType">The command type.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    Task<List<TModel>> QueryAsync<TModel>(string query, CommandType commandType,params KeyValuePair<string, object>[] whereStatementParameters);

    Task<DbDataReader> QueryReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    Task<DbDataReader> QueryReaderAsync(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters);

    IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    IAsyncEnumerable<DbDataReader> StreamReaderAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters);

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
    IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query with streaming asynchronously with the specified WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, params KeyValuePair<string, object>[] whereStatementParameters);

    /// <summary>
    /// Executes a database query with streaming asynchronously with the specified command behavior, command type, and WHERE statement parameters as key-value pairs.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="itemPerPage"></param>
    /// <param name="whereStatementParameters">The WHERE statement parameters as key-value pairs.</param>
    /// <returns>A task representing the asynchronous operation. A list of records as dictionaries.</returns>
    IAsyncEnumerable<List<TModel>> StreamAsync<TModel>(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters);

    

}




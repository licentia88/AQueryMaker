using System.Data;
using System.Data.Common;

namespace AQueryMaker.Extensions;

/// <summary>
/// Provides extension methods for working with database queries.
/// </summary>
internal static class QueryExtensions
{
    /// <summary>
    /// Returns an array of key-value pairs representing the elements of the specified model dictionary.
    /// </summary>
    /// <param name="model">The model dictionary.</param>
    /// <returns>An array of key-value pairs.</returns>
    public static KeyValuePair<string, object>[] GetAsWhereStatement(this Dictionary<string, object> model)
    {
        return model.Select(arg => new KeyValuePair<string, object>(arg.Key, arg.Value)).ToArray();
    }


    /// <summary>
    /// Retrieves the value of the specified column from the provided DbDataReader.
    /// </summary>
    /// <param name="reader">The DbDataReader.</param>
    /// <param name="columnName">The name of the column.</param>
    /// <returns>The column value.</returns>
    internal static object GetColumnValue(this DbDataReader reader, string columnName)
    {
        return reader.IsDBNull(columnName) ? default! : reader[columnName];
    }

    /// <summary>
    /// Asynchronously opens the database connection associated with the provided DbCommand, if it is not already open.
    /// </summary>
    /// <param name="command">The DbCommand.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task OpenAsync(this DbCommand command)
    {
        if (command.Connection != null && command.Connection.State != ConnectionState.Open)
            await command.Connection.OpenAsync();
    }

    /// <summary>
    /// Asynchronously opens the database connection, if it is not already open, associated with the provided DbConnection.
    /// </summary>
    /// <param name="connection">The DbConnection.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task OpenAsync(this DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
    }

    /// <summary>
    /// Opens the database connection associated with the provided DbCommand, if it is not already open.
    /// </summary>
    /// <param name="command">The DbCommand.</param>
    public static void Open(this DbCommand command)
    {
        if (command.Connection != null && command.Connection.State != ConnectionState.Open)
            command.Connection.Open();
    }

    /// <summary>
    /// Opens the database connection, if it is not already open, associated with the provided DbConnection.
    /// </summary>
    /// <param name="connection">The DbConnection.</param>
    public static void Open(this DbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
            connection.Open();
    }
}





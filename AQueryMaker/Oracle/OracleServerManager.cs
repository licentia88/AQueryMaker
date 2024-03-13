using System.ComponentModel;
using System.Data;
using System.Data.Common;
using AQueryMaker.Extensions;
using AQueryMaker.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace AQueryMaker.Oracle;

public class OracleServerManager : OracleQueryBuilder, IDatabaseManager
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="dbConnection"></param>
    public OracleServerManager(DbConnection dbConnection)
    {
        Connection = dbConnection;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Dictionary<string, object>> DeleteAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Dictionary<string, object>>> GetMethodParameters(string methodName)
    {
        throw new NotImplementedException();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<Dictionary<string, object>>> GetStoredProcedureFieldsAsync(string procedureName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Dictionary<string, object>>> GetStoredProcedureParametersAsync(string storedProcedureName)
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetStoredProcedureParametersStatement(storedProcedureName);

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public async Task<List<Dictionary<string, object>>> GetStoredProcedures()
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetStoredProceduresStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    public Task<List<Dictionary<string, object>>> GetTableFieldsAsync(string tableName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Dictionary<string, object>>> GetTableListAsync()
    {
        var command = Connection.CreateCommand();

        await command.OpenAsync();

        var metadataQuery = CreateGetTableListStatement();

        command.CommandText = metadataQuery;

        command.CommandType = CommandType.Text;

        DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.Default);

        var result = await ExecuteCommandAsync(reader);

        if (command.Connection != null) await command.Connection.CloseAsync();

        return result;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Dictionary<string, object>> InsertAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        throw new NotImplementedException();
    }


    public Task<List<Dictionary<string, object>>> QueryAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return QueryAsync(query, CommandType.Text, whereStatementParameters);

    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object>>> QueryAsync(string query, CommandType commandType,
        params KeyValuePair<string, object>[] whereStatementParameters)
    {
        if (Connection is not OracleConnection sqlConnection) throw new InvalidCastException();

        var command = sqlConnection.CreateCommand();

        await command.OpenAsync();

        command.CommandText = query;

        command.CommandType = commandType;

        AddWhereStatementParameters(command, whereStatementParameters);

        DbDataReader reader = await command.ExecuteReaderAsync();

        var result = await ExecuteCommandAsync(reader);


        await command.Connection.CloseAsync();

        return result;
    }

    public IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        return StreamAsync(query, 100, whereStatementParameters);
    }

    public async IAsyncEnumerable<List<Dictionary<string, object>>> StreamAsync(string query, int itemPerPage, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        var pageIndex = 0;

        var command = Connection.CreateCommand();

        bool hasMoreRows;

        do
        {
            await command.OpenAsync();

            command.CommandText = $" {query}  OFFSET {pageIndex * itemPerPage} ROWS  FETCH NEXT {itemPerPage} ROWS ONLY ";

            command.CommandType = CommandType.Text;

            AddWhereStatementParameters(command, whereStatementParameters);

            DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            var result = await ExecuteCommandAsync(reader);

            pageIndex++;

            hasMoreRows = reader.HasRows;

            await Connection.CloseAsync();

            yield return result;
        }
        while (hasMoreRows);
    }


    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<Dictionary<string, object>> UpdateAsync(string tableName, Dictionary<string, object> model, CommandBehavior commandBehavior = CommandBehavior.Default)
    {
        throw new NotImplementedException();
    }
}



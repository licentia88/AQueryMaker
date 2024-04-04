using System.Data;
using System.Data.Common;

namespace AQueryMaker;

public abstract class DatabaseManager
{
    public DbConnection Connection { get; set; }

    /// <summary>
    /// Asynchronously opens the database connection if it's not already open.
    /// </summary>
    internal async Task OpenConnectionAsync()
    {
        if (Connection.State != ConnectionState.Open)
            await Connection.OpenAsync();

    }

    /// <summary>
    /// Opens the database connection if it's not already open.
    /// </summary>
    internal void OpenConnection()
    {
        if (Connection.State != ConnectionState.Open)
            Connection.Open();
    }

    /// <summary>
    /// Adds parameters to the database command based on the model's field arguments.
    /// </summary>
    internal void AddParameters(DbCommand command, Dictionary<string, object> model, params string[] fieldArgs)
    {
        try
        {
            if (model is null) return;

            DbParameter[] parameters = new DbParameter[fieldArgs.Length];

            for (int i = 0; i < fieldArgs?.Length; i++)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{fieldArgs[i]}";
                parameter.Value = model[fieldArgs[i]] ?? DBNull.Value;

                parameters[i] = parameter;
            }

            if (parameters is not null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);
        }
        catch
        {
            // Handle exception or log the error
        }
    }

    /// <summary>
    /// Adds parameters to the database command based on the provided where statement parameters.
    /// </summary>
    public virtual  void AddWhereStatementParameters(DbCommand command, params (string Key, object Value)[] whereStatementParameters)
    {
        try
        {
            if (whereStatementParameters is null) return;

            var numberOfPropertyValues = whereStatementParameters.Length;

            DbParameter[] parameters = new DbParameter[numberOfPropertyValues];
            var parameterIndex = 0;

            foreach (var statement in whereStatementParameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{statement.Key}";
                parameter.Value = statement.Value ?? DBNull.Value;

                parameters[parameterIndex] = parameter;

                parameterIndex++;
            }

            if (parameters is not null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);
        }
        catch
        {
            // Handle exception or log the error
        }
    }

    /// <summary>
    /// Adds parameters to the database command based on the provided where statement parameters.
    /// </summary>
    virtual internal void AddWhereStatementParameters(DbCommand command, params KeyValuePair<string, object>[] whereStatementParameters)
    {
        try
        {
            if (whereStatementParameters is null) return;

            var numberOfPropertyValues = whereStatementParameters.Length;

             
            DbParameter[] parameters = new DbParameter[numberOfPropertyValues];

            var parameterIndex = 0;

            foreach (var statement in whereStatementParameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@{statement.Key}";
                parameter.Value = statement.Value ?? DBNull.Value;


               
                parameters[parameterIndex] = parameter;

                parameterIndex++;
            }

            if (parameters.Length > 0)
                command.Parameters.AddRange(parameters);
        }
        catch 
        {
            // Handle exception or log the error
        }
    }

    /// <summary>
    /// Retrieves the result set from the data reader and returns it as a list of dictionaries.
    /// </summary>
    private async Task<List<Dictionary<string, object>>> GetDataTableFromDataReaderAsync(DbDataReader dataReader)
    {
        var resultSet = new List<Dictionary<string, object>>();
        var columns = dataReader.GetColumnSchema();

        while (await dataReader.ReadAsync())
        {
            var newObj = new Dictionary<string, object>();

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var columnValue = dataReader[columnName];
                newObj[columnName] = columnValue is DBNull ? null : columnValue;
                //newObj.Add(columnName,  );
            }

            resultSet.Add(newObj);
        }

        return resultSet;
    }

    /// <summary>
    /// Retrieves the result set from the data reader and returns it as a list of dictionaries.
    /// </summary>
    private List<Dictionary<string, object>> GetDataTableFromDataReader(DbDataReader dataReader)
    {
        var resultSet = new List<Dictionary<string, object>>();
        var columns = dataReader.GetColumnSchema();

        while (dataReader.Read())
        {
            var newObj = new Dictionary<string, object>();

            foreach (var column in columns)
            {
                var columnName = column.ColumnName;
                var columnValue = dataReader[columnName];
                newObj[columnName] = columnValue is DBNull ? null : columnValue;
            }

            resultSet.Add(newObj);
        }

        return resultSet;
    }

    /// <summary>
    /// Executes the database command and returns the result set as a list of dictionaries.
    /// </summary>
    public List<Dictionary<string, object>> ExecuteCommand(DbDataReader reader)
    {
        return GetDataTableFromDataReader(reader);
    }

    /// <summary>
    /// Executes the database command asynchronously and returns the result set as a list of dictionaries.
    /// </summary>
    public async Task<List<Dictionary<string, object>>> ExecuteCommandAsync(DbDataReader reader )
    {
         return await GetDataTableFromDataReaderAsync(reader);
    }


}



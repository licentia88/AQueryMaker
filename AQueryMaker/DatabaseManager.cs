using System.Data.Common;

namespace AQueryMaker;

public abstract class DatabaseManager
{
    public DbConnection Connection { get; set; }

    /// <summary>
    /// Adds parameters to the database command based on the provided where statement parameters.
    /// </summary>
    internal virtual void AddWhereStatementParameters(DbCommand command, params KeyValuePair<string, object>[] whereStatementParameters)
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
            }
            resultSet.Add(newObj);
        }

        return resultSet;
    }

    /// <summary>
    /// Executes the database command asynchronously and returns the result set as a list of dictionaries.
    /// </summary>
    protected async Task<List<Dictionary<string, object>>> ExecuteCommandAsync(DbDataReader reader )
    {
         return await GetDataTableFromDataReaderAsync(reader);
    }


}



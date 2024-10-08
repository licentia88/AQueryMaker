using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AQueryMaker.Extensions;

/// <summary>
/// Provides methods to build SQL queries for a given model.
/// </summary>
public static class AQueryMakerExtensions
{
    /// <summary>
    /// Builds a SQL query for the specified model type with the given parameters.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="dbContext">DbContext</param>
    /// <param name="parameters">The parameters to include in the query.</param>
    /// <returns>A tuple containing the query string and the parameters.</returns>
    public static (string query, KeyValuePair<string, object>[] parameters) BuildQuery<TModel>(this DbContext dbContext,
        params KeyValuePair<string, object>[] parameters)
    {
        var tableName = typeof(TModel).Name;
        var alias = GenerateRandomAlias();
        var queryBuilder = new StringBuilder($"SELECT * FROM {tableName} {alias}");

        // Handle table joins for inherited types
        var parentAliases = new Dictionary<string, string>();
        AddParentJoins(typeof(TModel), alias, queryBuilder, parentAliases);

        // If no parameters, return query without WHERE clause
        if (parameters == null || !parameters.Any())
            return (queryBuilder.ToString(), parameters);

        // Build WHERE clause
        var whereClauses = parameters.Select(p => BuildWhereClause(p, alias, parentAliases, typeof(TModel), dbContext));
        queryBuilder.Append(" WHERE ").Append(string.Join(" AND ", whereClauses));

        // // Build WHERE clause
        // var whereClauses = parameters.Select(p => BuildWhereClause(p, alias, parentAliases, typeof(TModel)));
        // queryBuilder.Append(" WHERE ").Append(string.Join(" AND ", whereClauses));

        return (queryBuilder.ToString(), parameters);
    }

    
    /// <summary>
    /// Adds LEFT JOINs for base types to handle inherited properties.
    /// </summary>
    private static void AddParentJoins(Type type, string alias, StringBuilder queryBuilder, Dictionary<string, string> parentAliases)
    {
        Type baseType = type.BaseType;
        while (baseType != null && baseType != typeof(object))
        {
            var parentAlias = GenerateRandomAlias();
            var primaryKey = GetPrimaryKey(baseType);
            queryBuilder.AppendLine($" LEFT JOIN {baseType.Name} {parentAlias} ON {alias}.{primaryKey} = {parentAlias}.{primaryKey}");
            parentAliases[baseType.Name] = parentAlias;
            baseType = baseType.BaseType;
        }
    }

    /// <summary>
    /// Gets the appropriate alias for a key based on the class it belongs to.
    /// </summary>
    private static string GetAliasForKey(string[] parts, string defaultAlias, Dictionary<string, string> parentAliases, Type modelType)
    {
        if (parts.Length == 2 && parentAliases.ContainsKey(parts[0]))
            return parentAliases[parts[0]];

        var propertyType = modelType.GetProperty(parts.Length == 2 ? parts[1] : parts[0])?.DeclaringType;
        while (propertyType != null && propertyType != typeof(object))
        {
            if (parentAliases.TryGetValue(propertyType.Name, out var parentAlias))
                return parentAlias;

            propertyType = propertyType.BaseType;
        }

        return defaultAlias;
    }

    /// <summary>
    /// Generates a random alias for use in SQL queries.
    /// </summary>
    private static string GenerateRandomAlias()
    {
        var random = new Random();
        return $"{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}{random.Next(10)}{random.Next(10)}";
    }
    
    /// <summary>
    /// Gets the name of the primary key property for the specified model type.
    /// </summary>
    /// <param name="type">The type of the model.</param>
    /// <returns>The name of the primary key property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the primary key is not found.</exception>
    public static string GetPrimaryKey(Type type)
    {
        var primaryKeyProperty = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (primaryKeyProperty == null) throw new InvalidOperationException("Primary key not found for the model.");

        return primaryKeyProperty.Name;
    }
    
    /// <summary>
    /// Builds the WHERE clause for a given parameter.
    /// </summary>
    private static string BuildWhereClause(KeyValuePair<string, object> param, string alias, Dictionary<string, string> parentAliases, Type modelType, DbContext dbContext)
    {
        var (key, value) = param;
        var column = key.Contains('.') ? key.Split('.')[1] : key;
        var aliasToUse = GetAliasForKey(key.Split('.'), alias, parentAliases, modelType);

        // Switch to a switch statement to allow multiple lines of logic
        switch (value)
        {
            case DateTime dateValue:
                 
                // Return the DateTime-specific WHERE clause
                return GetDateTimeWhereClause(aliasToUse, column, key, dbContext);

            default:
                return $"{aliasToUse}.{column} = @{key}";
        }
    }

    /// <summary>
    /// Generates the WHERE clause for DateTime values based on the database provider.
    /// </summary>
    private static string GetDateTimeWhereClause(string alias, string column, string parameterKey, DbContext dbContext)
    {
        var providerName = dbContext.Database.ProviderName;

        return providerName switch
        {
            string name when name.Contains("SqlServer") => $"CONVERT(date, {alias}.{column}, 103) = CONVERT(date, @{parameterKey}, 103)",
            string name when name.Contains("Oracle") => $"TO_CHAR({alias}.{column}, 'DD/MM/YYYY') = TO_CHAR(@{parameterKey}, 'DD/MM/YYYY')",
            string name when name.Contains("MySql") => $"DATE_FORMAT({alias}.{column}, '%d/%m/%Y') = DATE_FORMAT(@{parameterKey}, '%d/%m/%Y')",
            _ => throw new NotSupportedException("Unsupported database provider for DateTime comparison.")
        };
    }

}
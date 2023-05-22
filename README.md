# AQueryMaker

AQueryMaker is a lightweight library for executing queries in a convenient and efficient way while also providing inbuild queries
such as Create, Read, Update and Delete

## Installation

You can install AQueryMaker using NuGet Package Manager or by adding the library as a reference to your project.

### NuGet Package Manager

To install AQueryMaker via NuGet Package Manager, run the following command in the Package Manager Console:


### Manual Installation

To manually install AQueryMaker, follow these steps:

1. Download the latest release from the GitHub repository (provide link here).
2. Add the AQueryMaker.dll reference to your project.
3. Import the AQueryMaker namespace in your code files.

## Getting Started

To start using AQueryMaker, follow these steps:

1.  Create an Instance of SqlQueryFactory



```csharp

var sqlManager = new SqlServerManager("DbConnection Instance here");
SqlQueryFactory factory = new SqlQueryFactory(sqlManager);

or

Create the following methods or a similar method to create connection.

internal static IDatabaseManager AddDatabaseResolver(Connections settings)
    {
        var connection = CreateConnection(settings);

        if (connection is OracleConnection)
        {
            return new OracleServerManager(connection);
        }

        if (connection is SqlConnection)
        {
            return new SqlServerManager(connection);
        }


        throw new InvalidOperationException("Unsupported database type");
    }
    
 Create a method to inject
 
private static void AddConnectionFactories(this IServiceCollection services, List<Connections> connections)
    {
        services.AddSingleton<IDictionary<string, Func<SqlQueryFactory>>>(provider =>
        {
            var connectionFactories = new Dictionary<string, Func<SqlQueryFactory>>();

            foreach (var ConnectionSetting in connections)
            {
                connectionFactories.Add(ConnectionSetting.Name, () => new SqlQueryFactory(ConnectionHelper.AddDatabaseResolver(ConnectionSetting)));
            }
            return connectionFactories;
        });
    }
    
    Use as 
    
     protected readonly IDictionary<string, Func<SqlQueryFactory>> ConnectionFactory;

    public SqlQueryFactory SqlQueryFactory(string connectionName) => ConnectionFactory[connectionName]?.Invoke();

    public YourService(IServiceProvider provider)
    {
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();
    }
 ```

## Examples

 
 

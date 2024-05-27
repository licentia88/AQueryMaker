# AQueryMaker
AQueryMaker is a C# library designed to simplify database operations by providing a streamlined interface for executing SQL queries and managing database connections. It supports MSSQL, MySQL, and Oracle databases, and seamlessly integrates with Entity Framework Core.

## Features
* Simplified interface for executing SQL queries
* Support for asynchronous database operations
* Streamlined database connection management
* Integration with Entity Framework Core for seamless usage
* Compatibility with MSSQL, MySQL, and Oracle databases
* Streaming data retrieval capability for large datasets

## Installation
To install AQueryMaker via NuGet Package Manager, run the following command:

```powershell
dotnet add package AQueryMaker
```

## Usage
### Basic Usage
To use AQueryMaker in your project, follow these steps:

#### 1. Initialize Database Connection Manager:

Initialize the QueryManager by passing an instance of IDatabaseManager.

```csharp
IDatabaseManager manager = new SqlServerManager(new SqlConnection(connectionString));
var queryManager = new QueryManager(manager);
```

or

```csharp
IDatabaseManager manager = new MySqlServerManager(new MySqlConnection(connectionString));
var queryManager = new QueryManager(manager);
```

or


```csharp
IDatabaseManager manager = new OraclelServerManager(new OracleConnection(connectionString));
var queryManager = new QueryManager(manager);
```

#### 2. Execute Queries:

Use the methods provided by QueryManager to execute queries.

```csharp
var result = await queryManager.QueryAsync("SELECT * FROM TableName");
```

### Entity Framework Integration
AQueryMaker seamlessly integrates with Entity Framework Core to simplify database operations.

#### 1. Initialize QueryManager:

Initialize the QueryManager using Entity Framework's database connection.

```csharp
var dbContext = new YourDbContext();
var queryManager = dbContext.Manager(); //Gets connection based on
```

alternatively you can explicitly call dbContext.SqlManager() or dbContext.MySqlManager() or dbContext.OracleManager()

#### 2. Execute Queries:

Use the methods provided by QueryManager to execute queries.

```csharp
var result = await queryManager.QueryAsync("SELECT * FROM TableName");
```


## Examples
#### Streaming Data

To stream data from the database, use the StreamAsync method provided by QueryManager. Please note that in order to use the stream mode, the query must contain an ORDER BY clause.

```csharp
await foreach (var data in Db.Manager().StreamAsync("SELECT * FROM USERS OrderBy 1",10)) // 10 is batch size, items per page
{
    yield return data;
}
```

## Inserting a Record
```csharp
var model = new Dictionary<string, object>
{
    { "ColumnName", value }
};

var result = await queryManager.InsertAsync("TableName", model);
```
## Updating a Record
```csharp
var model = new Dictionary<string, object>
{
    { "ColumnName", value }
};

var result = await queryManager.UpdateAsync("TableName", model);
```

## Deleting a Record

```csharp
var model = new Dictionary<string, object>
{
    { "ColumnName", value }
};

var result = await queryManager.DeleteAsync("TableName", model);
```

## Querying with Parameters
```csharp
var result = await queryManager.QueryAsync("SELECT * FROM TableName WHERE ColumnName = @Value", new KeyValuePair<string, object>("Value", value));
```

## Supported Databases
AQueryMaker supports the following databases:

* MSSQL
* MySQL
* Oracle (Note: CRUD operations are not supported)

## Contribution
Contributions to AQueryMaker are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request.


## License
This project is licensed under the MIT License.

## Contact
For any inquiries or support, please contact me.

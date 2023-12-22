using System.Data.Common;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;

namespace AQueryMaker.Extensions;

public static class DatabaseExtensions
{
    
    public static SqlServerManager SqlManager(this DbConnection connection)
    {
        return new SqlServerManager(connection);
    }
    
    public static OracleServerManager OracleManager(this DbConnection connection)
    {
        return new OracleServerManager(connection);
    }
}
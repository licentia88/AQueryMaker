using System.Data.Common;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using Microsoft.EntityFrameworkCore;

namespace AQueryMaker.Extensions;

public static class DatabaseExtensions
{
    public static SqlServerManager SqlManager(this DbContext context)
    {
        return new SqlServerManager(context.Database.GetDbConnection());
    }
    
    public static SqlServerManager SqlManager(this DbConnection connection)
    {
        return new SqlServerManager(connection);
    }
    
    public static OracleServerManager OracleManager(this DbConnection connection)
    {
        return new OracleServerManager(connection);
    }
    public static OracleServerManager OracleManager(this DbContext context)
    {
        return new OracleServerManager(context.Database.GetDbConnection());
    }
}
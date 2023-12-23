using System.Data.Common;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using Microsoft.EntityFrameworkCore;

namespace AQueryMaker.Extensions;

// ReSharper disable once UnusedType.Global
public static class DatabaseExtensions
{
    // ReSharper disable once UnusedMember.Global
    public static SqlServerManager SqlManager(this DbContext context)
    {
        return new SqlServerManager(context.Database.GetDbConnection());
    }
    
    // ReSharper disable once UnusedMember.Global
    public static SqlServerManager SqlManager(this DbConnection connection)
    {
        return new SqlServerManager(connection);
    }
    
    // ReSharper disable once UnusedMember.Global
    public static OracleServerManager OracleManager(this DbConnection connection)
    {
        return new OracleServerManager(connection);
    }
    
    // ReSharper disable once UnusedMember.Global
    public static OracleServerManager OracleManager(this DbContext context)
    {
        return new OracleServerManager(context.Database.GetDbConnection());
    }
}
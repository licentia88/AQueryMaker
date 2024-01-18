using System.Data.Common;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using Microsoft.EntityFrameworkCore;

namespace AQueryMaker.Extensions;

// ReSharper disable once UnusedType.Global
public static class DatabaseExtensions
{
    // ReSharper disable once UnusedMember.Global
    public static SqlServerManager SqlManager(this DbContext context) => new(context.Database.GetDbConnection());

    // ReSharper disable once UnusedMember.Global
    public static SqlServerManager SqlManager(this DbConnection connection) => new(connection);
    
    // ReSharper disable once UnusedMember.Global
    public static OracleServerManager OracleManager(this DbConnection connection) => new(connection);
    
    // ReSharper disable once UnusedMember.Global
    public static OracleServerManager OracleManager(this DbContext context) => new(context.Database.GetDbConnection());
  
}
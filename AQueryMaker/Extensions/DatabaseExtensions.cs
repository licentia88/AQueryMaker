using System.Data.Common;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using Microsoft.EntityFrameworkCore;

namespace AQueryMaker.Extensions;

// ReSharper disable once UnusedType.Global
public static class DatabaseExtensions
{
    // ReSharper disable once UnusedMember.Global
    public static QueryManager Manager(this DbContext context)
    {
        return context.Database.ProviderName switch
        {
            "Microsoft.EntityFrameworkCore.SqlServer" => new QueryManager(SqlManager(context.Database.GetDbConnection())),
            "Oracle.EntityFrameworkCore" => new QueryManager(OracleManager(context.Database.GetDbConnection())),
            _ => default
        };
    }

    // ReSharper disable once UnusedMember.Global
    public static SqlServerManager SqlManager(this DbConnection connection) => new(connection);
    
    // ReSharper disable once UnusedMember.Global
    public static OracleServerManager OracleManager(this DbConnection connection) => new(connection);
    
  
}
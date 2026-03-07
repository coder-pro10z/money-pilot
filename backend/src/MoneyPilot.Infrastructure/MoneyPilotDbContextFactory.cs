using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MoneyPilot.Infrastructure.Data;
using System.IO;

using Npgsql.EntityFrameworkCore.PostgreSQL;   // <-- Add this
public class MoneyPilotDbContextFactory : IDesignTimeDbContextFactory<MoneyPilotDbContext>
{
    public MoneyPilotDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json (and optionally appsettings.Development.json)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();


        var optionsBuilder = new DbContextOptionsBuilder<MoneyPilotDbContext>();

        // Read the provider from configuration (set in appsettings.json)
        var provider = configuration["DatabaseProvider"];
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        if (provider == "Postgres")
        {
            optionsBuilder.UseNpgsql(connectionString,
                x => x.MigrationsAssembly("MoneyPilot.Infrastructure"));
        }
        else
        {
            // Fallback to SQL Server (your local connection)
            optionsBuilder.UseSqlServer(connectionString,
                x => x.MigrationsAssembly("MoneyPilot.Infrastructure"));
        }

        return new MoneyPilotDbContext(optionsBuilder.Options);
    }
}
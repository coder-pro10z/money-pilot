using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MoneyPilot.Infrastructure.Data;

public class MoneyPilotDbContextFactory
    : IDesignTimeDbContextFactory<MoneyPilotDbContext>
{
    public MoneyPilotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MoneyPilotDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-48C94E6\\SQLEXPRESS;Database=MoneyPilotDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

        return new MoneyPilotDbContext(optionsBuilder.Options);
    }
}

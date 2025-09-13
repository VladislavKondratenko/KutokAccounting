using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace KutokAccounting.DataProvider;

public class KutokDbContextFactory : IDesignTimeDbContextFactory<KutokDbContext>
{
    public KutokDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KutokDbContext>();

        optionsBuilder.UseSqlite(KutokConfigurations.ConnectionString);

        return new KutokDbContext(optionsBuilder.Options);
    }
}
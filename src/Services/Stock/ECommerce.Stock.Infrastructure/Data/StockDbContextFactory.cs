using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Stock.Infrastructure.Data;

public class StockDbContextFactory : IDesignTimeDbContextFactory<StockDbContext>
{
    public StockDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StockDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=stockdb;Username=stockuser;Password=stockpass");
        return new StockDbContext(optionsBuilder.Options);
    }
}

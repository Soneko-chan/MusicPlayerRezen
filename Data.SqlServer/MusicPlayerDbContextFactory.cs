using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data.SqlServer;

public class MusicPlayerDbContextFactory : IDesignTimeDbContextFactory<MusicPlayerDbContext>
{
    public MusicPlayerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.database.json")
        .Build();

        return CreateDbContext(configuration);
    }
    public MusicPlayerDbContext CreateDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<MusicPlayerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new MusicPlayerDbContext(optionsBuilder.Options);
    }
}
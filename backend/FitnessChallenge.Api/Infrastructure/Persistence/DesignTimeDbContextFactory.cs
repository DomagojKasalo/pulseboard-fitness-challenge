using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FitnessChallenge.Api.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FitnessDbContext>
{
    public FitnessDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FitnessDbContext>()
            .UseSqlite("Data Source=fitness.db")
            .Options;

        return new FitnessDbContext(options);
    }
}

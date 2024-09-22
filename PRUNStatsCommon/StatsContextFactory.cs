using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PRUNStatsCommon
{
    public class StatsContextFactory : IDesignTimeDbContextFactory<StatsContext>
    {
        public StatsContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"Creating DbContext with connstring: {args.FirstOrDefault()?[..20]}");

            var optionsBuilder = new DbContextOptionsBuilder<StatsContext>();
            optionsBuilder.UseSqlServer(args.FirstOrDefault());

            return new StatsContext(optionsBuilder.Options);
        }
    }
}

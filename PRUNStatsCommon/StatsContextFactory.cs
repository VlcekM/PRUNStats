using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PRUNStatsCommon
{
    public class StatsContextFactory : IDesignTimeDbContextFactory<StatsContext>
    {
        public StatsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StatsContext>();
            optionsBuilder.UseSqlServer(args.FirstOrDefault());

            return new StatsContext(optionsBuilder.Options);
        }
    }
}

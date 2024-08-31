using Microsoft.EntityFrameworkCore;
using PRUNStatsCommon.Bases;
using PRUNStatsCommon.Companies;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Planets;
using PRUNStatsCommon.Users;

namespace PRUNStatsCommon
{
    public class StatsContext(DbContextOptions<StatsContext> options) : DbContext(options)
    {
        public DbSet<CompanyModel> Companies { get; set; } = null!;

        public DbSet<CorporationModel> Corporations { get; set; } = null!;

        public DbSet<PlanetModel> Planets { get; set; } = null!;

        public DbSet<UserModel> Users { get; set;} = null!;

        public DbSet<BaseModel> Bases { get; set; } = null!;
    }
}

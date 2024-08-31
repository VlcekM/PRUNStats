using Microsoft.EntityFrameworkCore;
using PRUNStatsCommon.Companies.RowItems;

namespace PRUNStatsCommon.Companies.Services
{
    public class CompanyService(IDbContextFactory<StatsContext> _contextFactory)
    {
        //public async Task<List<CompanyRowItem>> FetchCompanyRowItems()
        //{
        //    await using var context = await _contextFactory.CreateDbContextAsync();

        //    return await context.Companies
        //        .AsNoTracking()
        //        .AsSplitQuery()
        //        .Select(CompanyRowItem.FromModel())
        //        .ToListAsync();
        //}
    }
}

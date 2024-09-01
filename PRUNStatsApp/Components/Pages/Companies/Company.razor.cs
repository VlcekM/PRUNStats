using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PRUNStatsCommon.Companies.Models;

namespace PRUNStatsApp.Components.Pages.Companies
{
    public partial class Company : ComponentBase
    {
        [Parameter]
        public Guid? CompanyId { get; set; }

        private CompanyModel? LoadedCompany { get; set; }

        private string LoadedCompanyName => LoadedCompany?.CompanyName ?? "Loading...";

        protected override async Task OnInitializedAsync()
        {
            //get the company with the ID
            await using var ctxt =  await _contextFactory.CreateDbContextAsync();
            LoadedCompany = await ctxt.Companies
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Bases)
                .ThenInclude(x => x.Planet)
                .Include(x => x.Corporation)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.PRGUID == CompanyId);
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PRUNStatsCommon.Companies.Models;

namespace PRUNStatsApp.Components.Pages.Companies
{
    public partial class Company : ComponentBase
    {
        [Parameter]
        public Guid? CompanyId { get; set; }

        private CompanyModel? LoadedCompany { get; set; }

        private string LoadedCompanyName => LoadedCompany?.CompanyName ?? "Loading...";

        private string CorporationName { get; set; } = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;
            //get the company with the ID
            LoadedCompany = await _dbContext.Companies
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Bases)
                .ThenInclude(x => x.Planet)
                .Include(x => x.Corporation)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.PRGUID == CompanyId);

            CorporationName = LoadedCompany?.Corporation?.CorporationName ?? string.Empty;
            StateHasChanged();
        }
    }
}

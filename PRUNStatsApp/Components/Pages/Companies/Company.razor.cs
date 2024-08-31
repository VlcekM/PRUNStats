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
                .FirstOrDefaultAsync(x => x.PRGUID == CompanyId);
        }
    }
}

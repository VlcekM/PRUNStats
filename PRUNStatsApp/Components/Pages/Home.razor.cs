using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace PRUNStatsApp.Components.Pages
{
    public partial class Home : ComponentBase
    {
        private int TrackedCompanies { get; set; }
        private int TrackedBases { get; set; }
        private int TrackedPlanets { get; set; }
        private int TrackedPlayers { get; set; }
        private int TrackedCorporations { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            TrackedCompanies = await dbContext.Companies.CountAsync();
            TrackedBases = await dbContext.Bases.CountAsync();
            TrackedPlanets = await dbContext.Planets.CountAsync();
            TrackedPlayers = await dbContext.Users.CountAsync();
            TrackedCorporations = await dbContext.Corporations.CountAsync();
        }
    }
}

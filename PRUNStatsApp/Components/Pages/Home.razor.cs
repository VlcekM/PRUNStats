using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PRUNStatsCommon;
using PRUNStatsCommon.Companies.Models;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Planets;

namespace PRUNStatsApp.Components.Pages
{
    public partial class Home : ComponentBase
    {
        private int TrackedCompanies { get; set; }
        private int TrackedBases { get; set; }
        private int TrackedPlanets { get; set; }
        private int TrackedPlayers { get; set; }
        private int TrackedCorporations { get; set; }

        // Faction Chart
        private double[] CompanyFactionDistrib { get; set; } = [];
        private string[] FactionNames { get; set; } = [];
        private ChartOptions FactionChartOptions { get; set; } = new();

        // Popular Planets
        private List<PlanetModel> PopularPlanets { get; set; } = [];
        private ApexCharts.ApexChartOptions<PlanetModel> PopularPlanetsOptions { get; set; }

        // Most base players
        private List<CompanyModel> MostBaseCompanies { get; set; } = [];
        private ApexCharts.ApexChartOptions<CompanyModel> MostBaseCompaniesOptions { get; set; }


        private bool LoadedData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            TrackedCompanies = await dbContext.Companies.CountAsync();
            TrackedBases = await dbContext.Bases.CountAsync();
            TrackedPlanets = await dbContext.Planets.CountAsync();
            TrackedPlayers = await dbContext.Users.CountAsync();
            TrackedCorporations = await dbContext.Corporations.CountAsync();

            await BuildFactionChartAsync(dbContext);

            await BuildPopularPlanetsAsync(dbContext);

            await BuildMostBasesCompaniesAsync(dbContext);

            LoadedData = true;
        }

        private async Task BuildFactionChartAsync(StatsContext dbContext)
        {
            var companyDistrib = await dbContext.Companies
                .GroupBy(x => x.Faction)
                .Select(x => new { Faction = x.Key, Count = x.Count() })
                .ToListAsync();

            CompanyFactionDistrib = new double[4];
            FactionNames = new string[4];

            var antares = companyDistrib.FirstOrDefault(x => x.Faction == Faction.AntaresInitiative);
            var castillo = companyDistrib.FirstOrDefault(x => x.Faction == Faction.CastilloItoMercantile);
            var insitor = companyDistrib.FirstOrDefault(x => x.Faction == Faction.InsitorCooperative);
            var neo = companyDistrib.FirstOrDefault(x => x.Faction == Faction.NEOCharterExploration);

            CompanyFactionDistrib[0] = antares?.Count ?? 0;
            FactionNames[0] = $"Antares ({CompanyFactionDistrib[0]:N0})";

            CompanyFactionDistrib[1] = castillo?.Count ?? 0;
            FactionNames[1] = $"Castillo-Ito ({CompanyFactionDistrib[1]:N0})";

            CompanyFactionDistrib[2] = insitor?.Count ?? 0;
            FactionNames[2] = $"Insitor ({CompanyFactionDistrib[2]:N0})";

            CompanyFactionDistrib[3] = neo?.Count ?? 0;
            FactionNames[3] = $"NEO ({CompanyFactionDistrib[3]:N0})";

            FactionChartOptions = new ChartOptions
            {
                ChartPalette =
                [
                    "#F4511E",
                    "#E10909",
                    "#009920",
                    "#F4C626"
                ]
            };
        }

        private async Task BuildPopularPlanetsAsync(StatsContext dbContext)
        {
            //only top 10 planets
            var topPlanetCount = 10;

            //get the top X planets by company count
            PopularPlanets = await dbContext.Planets
                .Include(x => x.Bases)
                .OrderByDescending(x => x.Bases.Count)
                .Take(topPlanetCount)
                .ToListAsync();

            PopularPlanetsOptions = new ApexChartOptions<PlanetModel>
            {
                Theme = new Theme
                {
                    Mode = Mode.Dark
                },
                Chart = new Chart
                {
                    Toolbar = new Toolbar
                    {
                        Show = false
                    }
                },
                PlotOptions = new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true,
                    }
                },
            };
        }

        private async Task BuildMostBasesCompaniesAsync(StatsContext dbContext)
        {
            //only top 10 players
            var topCompanyCount = 10;

            //get the top X companies by base count
            MostBaseCompanies = await dbContext.Companies
                .Include(x => x.Bases)
                .Include(x => x.User)
                .OrderByDescending(x => x.Bases.Count)
                .Take(topCompanyCount)
                .ToListAsync();

            MostBaseCompaniesOptions = new ApexChartOptions<CompanyModel>
            {
                Theme = new Theme
                {
                    Mode = Mode.Dark
                },
                Chart = new Chart
                {
                    Toolbar = new Toolbar
                    {
                        Show = false
                    }
                },
                PlotOptions = new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true,
                    }
                },
            };
        }
    }
}

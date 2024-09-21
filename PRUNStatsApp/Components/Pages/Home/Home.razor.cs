using System.Diagnostics;
using ApexCharts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using PRUNStatsCommon;
using PRUNStatsCommon.Companies.Models;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Planets;

namespace PRUNStatsApp.Components.Pages.Home
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
        private ApexChartOptions<PlanetModel>? PopularPlanetsOptions { get; set; }

        // Most base players
        private List<CompanyModel> MostBaseCompanies { get; set; } = [];
        private ApexChartOptions<CompanyModel>? MostBaseCompaniesOptions { get; set; }

        private bool LoadedData { get; set; } = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            TrackedCompanies = await _statsContext.Companies.CountAsync();
            TrackedBases = await _statsContext.Bases.CountAsync();
            TrackedPlanets = await _statsContext.Planets.CountAsync();
            TrackedPlayers = await _statsContext.Users.CountAsync();
            TrackedCorporations = await _statsContext.Corporations.CountAsync();

            await BuildFactionChartAsync(_statsContext);

            await BuildPopularPlanetsAsync(_statsContext);

            await BuildMostBasesCompaniesAsync(_statsContext);

            LoadedData = true;
            StateHasChanged();
        }

        private async Task BuildFactionChartAsync(StatsContext dbContext)
        {
            var companyDistrib = await dbContext.Companies
                .AsNoTracking()
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
                .AsNoTracking()
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
                    },
                    FontFamily = "Open Sans, sans-serif",
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
                .AsNoTracking()
                .Include(x => x.Bases)
                .Include(x => x.User)
                .OrderByDescending(x => x.Bases.Count)
                .Take(topCompanyCount)
                .AsSplitQuery()
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
                    },
                    FontFamily = "Open Sans, sans-serif",
                },
                PlotOptions = new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        Horizontal = true,
                    },
                },
            };
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using PRUNStatsCommon.Companies.Models;
using PRUNStatsCommon.Companies.RowItems;
using System.Linq;

namespace PRUNStatsApp.Components.Pages.Companies
{
    public partial class CompanyList : ComponentBase
    {
        private MudDataGrid<CompanyRowItem> DataGrid { get; set; } = null!;

        private string SearchFilter { get; set; } = string.Empty;
        private string PlayerFilter { get; set; } = string.Empty;
        private string CorporationFilter { get; set; } = string.Empty;
        private string PlanetFilter { get; set; } = string.Empty;

        private IReadOnlyCollection<Faction> DisplayedFactions { get; set; } = [ Faction.AntaresInitiative,
            Faction.CastilloItoMercantile, Faction.InsitorCooperative, Faction.NEOCharterExploration ];

        /// <summary>
        /// This variable is used to prevent the filtering from being applied before the first render.
        /// </summary>
        private bool AfterFirstRender { get; set; }

        #region Handlers

        protected override void OnInitialized()
        {
            AfterFirstRender = true;
        }

        private async Task OnCompanyFilterChanged(string text)
        {
            if (!AfterFirstRender) return;
            SearchFilter = text;
            await DataGrid.ReloadServerData();
        }

        private async Task OnFactionFilterChanged(IReadOnlyCollection<Faction> value)
        {
            if (!AfterFirstRender) return;
            DisplayedFactions = value;
            await DataGrid.ReloadServerData();
        }

        private async Task OnPlayerFilterChanged(string text)
        {
            if (!AfterFirstRender) return;
            PlayerFilter = text;
            await DataGrid.ReloadServerData();
        }

        private async Task OnCorporationFilterChanged(string text)
        {
            if (!AfterFirstRender) return;
            CorporationFilter = text;
            await DataGrid.ReloadServerData();
        }

        private async Task OnPlanetFilterChanged(string text)
        {
            if (!AfterFirstRender) return;
            PlanetFilter = text;
            await DataGrid.ReloadServerData();
        }

        private void OnRowClicked(DataGridRowClickEventArgs<CompanyRowItem> eventArgs)
        {
            _navigationManager.NavigateTo(RouteConstants.Company + eventArgs.Item.PRGUID);
        }

        private async Task<GridData<CompanyRowItem>> ServerReloadAsync(GridState<CompanyRowItem> state)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var queryable = context.Companies
                .Where(x => x.Faction != null)
                .Select(CompanyRowItem.FromModel())
                .AsNoTracking();

            //filtering
            queryable = ApplyFiltering(queryable);
            queryable = ApplySorting(queryable, state);

            var data = await queryable
                .Skip(state.Page * state.PageSize)
                .Take(state.PageSize)
                .ToListAsync();

            return new GridData<CompanyRowItem>
            {
                Items = data,
                TotalItems = await queryable.CountAsync()
            };
        }

        #endregion Handlers

        private IQueryable<CompanyRowItem> ApplyFiltering(IQueryable<CompanyRowItem> queryable)
        {
            var result = queryable;
            if (!string.IsNullOrWhiteSpace(SearchFilter))
            {
                result = result.Where(x => x.Code.Contains(SearchFilter) || x.Name.Contains(SearchFilter));
            }

            if (!string.IsNullOrWhiteSpace(PlayerFilter))
            {   
                result = result.Where(x => x.UserName.Contains(PlayerFilter));
            }

            if (!string.IsNullOrWhiteSpace(CorporationFilter))
            {
                result = result.Where(x => x.CorporationName.Contains(CorporationFilter));
            }

            if (!string.IsNullOrWhiteSpace(PlanetFilter))
            {
                result = result.Where(x => 
                x.BasesOnPlanets
                    .Select(y => y.PlanetName)
                    .Contains(PlanetFilter)
                || 
                x.BasesOnPlanets
                    .Select(y => y.NaturalId)
                    .Contains(PlanetFilter));
            }

            var displayedFactions = DisplayedFactions.ToList();
            if (displayedFactions.Count < 4)
            {
                result = result.Where(x => x.Faction != null && displayedFactions.Contains(x.Faction.Value));
            }

            return result;
        }

        private IQueryable<CompanyRowItem> ApplySorting(IQueryable<CompanyRowItem> queryable, GridState<CompanyRowItem> state)
        {
            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition is null)
            {
                return queryable.OrderBy(x => x.Code);
            }
            else
            {
                return sortDefinition.SortBy switch
                {
                    nameof(CompanyRowItem.Code) => sortDefinition.Descending
                        ? queryable.OrderByDescending(x => x.Code)
                        : queryable.OrderBy(x => x.Code),
                    nameof(CompanyRowItem.Name) => sortDefinition.Descending
                        ? queryable.OrderByDescending(x => x.Name)
                        : queryable.OrderBy(x => x.Name),
                    nameof(CompanyRowItem.UserName) => sortDefinition.Descending
                        ? queryable.OrderByDescending(x => x.UserName)
                        : queryable.OrderBy(x => x.UserName),
                    nameof(CompanyRowItem.CorporationName) => sortDefinition.Descending
                        ? queryable.OrderByDescending(x => x.CorporationName)
                        : queryable.OrderBy(x => x.CorporationName),
                    _ => queryable
                };
            }
        }
    }
}

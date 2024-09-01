using PRUNStatsCommon.Companies.Models;
using PRUNStatsCommon.Planets;
using System.Linq.Expressions;

namespace PRUNStatsCommon.Companies.RowItems
{
    public class CompanyRowItem
    {
        public Guid PRGUID { get; set; }

        public string? Code { get; set; } 

        public string? Name { get; set; }

        public string? UserName { get; set; }

        public Faction? Faction { get; set; }

        public string FactionText => Faction switch
        {
            Models.Faction.AntaresInitiative => "Antares",
            Models.Faction.CastilloItoMercantile => "Castillo-Ito",
            Models.Faction.ExodusCouncil => "Exodus",
            Models.Faction.InsitorCooperative => "Insitor",
            Models.Faction.NEOCharterExploration => "NEO",
            _ => "(unknown)"
        };

        public string? CorporationName { get; set; }

        public List<BaseOnPlanet> BasesOnPlanets { get; set; } = [];

        public string BasesOnPlanetsText => string.Join(", ",
            BasesOnPlanets
                .Select(x => string.IsNullOrWhiteSpace(x.PlanetName) ? x.NaturalId : x.PlanetName));

        public DateTime? FIOUpdateTimestamp { get; set; }
        public DateTime? CreatedOnDateTime { get; set; }

        public static Expression<Func<CompanyModel, CompanyRowItem>> FromModel()
        {
            return x => new CompanyRowItem
            {
                PRGUID = x.PRGUID.Value,
                Code = x.CompanyCode,
                Name = x.CompanyName,
                UserName = x.User.Username,
                Faction = x.Faction,
                CorporationName = x.Corporation.CorporationName,
                BasesOnPlanets = x.Bases.Select(y => new BaseOnPlanet { NaturalId = y.Planet.NaturalId, PlanetName = y.Planet.Name }).ToList(),
                FIOUpdateTimestamp = x.LastUpdatedFIO,
                CreatedOnDateTime = x.CreatedAt
            };
        }

        public class BaseOnPlanet
        {
            public string NaturalId { get; set; } = string.Empty;
            public string PlanetName { get; set; } = string.Empty;
        }
    }
}

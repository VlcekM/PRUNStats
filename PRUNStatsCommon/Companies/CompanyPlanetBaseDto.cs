using System.Text.Json.Serialization;

namespace PRUNStatsCommon.Companies
{
    public class CompanyPlanetBaseDto
    {
        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? PlanetId { get; set; }
        public required string PlanetNaturalId { get; set; }
        public required string PlanetName { get; set; }
    }
}

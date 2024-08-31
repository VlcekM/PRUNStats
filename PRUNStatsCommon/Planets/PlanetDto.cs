using System.Text.Json.Serialization;

namespace PRUNStatsCommon.Planets
{
    public class PlanetDto
    {
        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? PlanetId { get; set; }
        public required string PlanetNaturalId { get; set; }
        public required string PlanetName { get; set; }
    }
}

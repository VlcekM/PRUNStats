using System.Text.Json.Serialization;

namespace PRUNStatsCommon.Planets
{
    public class PlanetDto
    {
        [JsonConverter(typeof(JsonConverterGuid))]
        public required Guid PlanetId { get; set; }
        public required string PlanetNaturalId { get; set; }
        public required string PlanetName { get; set; }
    }
}

using System.Text.Json.Serialization;
using PRUNStatsCommon.Planets;

namespace PRUNStatsCommon.Companies
{
    public class CompanyDto
    {
        public List<PlanetDto> Planets { get; set; } = [];

        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? UserDataId { get; set; }

        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? SubscriptionLevel { get; set; }
        public string? Tier { get; set; }
        public bool Team { get; set; }
        public bool Pioneer { get; set; }
        public bool Moderator { get; set; }
        public long CreatedEpochMs { get; set; }

        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? CompanyId { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyCode { get; set; }
        public required string CountryId { get; set; }
        public required string CountryCode { get; set; }
        public required string CountryName { get; set; }

        [JsonConverter(typeof(JsonConverterGuid))]
        public Guid? CorporationId { get; set; }
        public string? CorporationName { get; set; }
        public string? CorporationCode { get; set; }
        public required string OverallRating { get; set; }
        public required string ActivityRating { get; set; }
        public required string ReliabilityRating { get; set; }
        public required string StabilityRating { get; set; }
        public string? HeadquartersNaturalId { get; set; }
        public int HeadquartersLevel { get; set; }
        public int HeadquartersBasePermits { get; set; }
        public int HeadquartersUsedBasePermits { get; set; }
        public int AdditionalBasePermits { get; set; }
        public int AdditionalProductionQueueSlots { get; set; }
        public bool RelocationLocked { get; set; }
        public int NextRelocationTimeEpochMs { get; set; }
        public string? UserNameSubmitted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

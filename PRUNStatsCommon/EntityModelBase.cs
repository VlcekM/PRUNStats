using System.ComponentModel.DataAnnotations;

namespace PRUNStatsCommon
{
    public abstract class EntityModelBase
    {
        [Key]
        public int Id { get; set; }

        public required DateTime FirstImportedAtUTC { get; set; }
        public required DateTime LastUpdatedAtUTC { get; set; }
    }
}

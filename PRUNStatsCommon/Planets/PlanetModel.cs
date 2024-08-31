using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRUNStatsCommon.Bases;

namespace PRUNStatsCommon.Planets
{
    [Table("Planets", Schema = "prun")]
    public class PlanetModel : EntityModelBase
    {
        [MaxLength(10)]
        public required string NaturalId { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [InverseProperty(nameof(BaseModel.Planet))]
        public virtual ICollection<BaseModel> Bases { get; set; } = [];
    }
}

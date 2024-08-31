using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRUNStatsCommon.Companies.Models;

namespace PRUNStatsCommon.Corporations
{
    [Table("Corporations", Schema = "prun")]
    public class CorporationModel : EntityModelFullBase
    {
        [MaxLength(100)]
        public required string CorporationName { get; set; }
        [MaxLength(4)]
        public required string CorporationCode { get; set; }

        [InverseProperty(nameof(CompanyModel.Corporation))]
        public virtual ICollection<CompanyModel> Companies { get; set; } = [];
    }
}

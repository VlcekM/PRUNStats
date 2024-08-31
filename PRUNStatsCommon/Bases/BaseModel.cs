using PRUNStatsCommon.Companies;
using PRUNStatsCommon.Planets;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRUNStatsCommon.Bases
{
    [Table("Bases", Schema = "prun")]
    public class BaseModel : EntityModelBase
    {
        public required CompanyModel Company { get; set; }
        public required PlanetModel Planet { get; set; }

    }
}

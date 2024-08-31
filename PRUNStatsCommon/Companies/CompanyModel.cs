using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRUNStatsCommon.Bases;
using PRUNStatsCommon.Corporations;
using PRUNStatsCommon.Users;

namespace PRUNStatsCommon.Companies
{
    [Table("Companies", Schema = "prun")]
    public class CompanyModel : EntityModelBase
    {
        public required UserModel User { get; set; }

        public CorporationModel? Corporation { get; set; }

        [MaxLength(100)]
        public required string CompanyName { get; set; }
        [MaxLength(4)]
        public required string CompanyCode { get; set; }

        public Faction? Faction { get; set; }

        [InverseProperty(nameof(BaseModel.Company))]
        public virtual ICollection<BaseModel> Bases { get; set; } = [];
    }

    public enum Faction
    {
        AntaresInitiative,
        CastilloItoMercantile,
        ExodusCouncil,
        InsitorCooperative,
        NEOCharterExploration
    }
}

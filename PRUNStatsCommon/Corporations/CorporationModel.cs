﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRUNStatsCommon.Companies;

namespace PRUNStatsCommon.Corporations
{
    [Table("Corporations", Schema = "prun")]
    public class CorporationModel : EntityModelBase
    {
        [MaxLength(100)]
        public required string CorporationName { get; set; }
        [MaxLength(4)]
        public required string CorporationCode { get; set; }

        [InverseProperty(nameof(CompanyModel.Corporation))]
        public virtual ICollection<CompanyModel> Companies { get; set; } = [];
    }
}

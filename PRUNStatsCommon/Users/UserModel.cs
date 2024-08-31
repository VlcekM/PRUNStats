using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRUNStatsCommon.Users
{
    [Table("Users", Schema = "prun")]
    public class UserModel : EntityModelFullBase
    {
        [MaxLength(50)]
        public required string Username { get; set; }
    }
}

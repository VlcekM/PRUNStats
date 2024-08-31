using System.ComponentModel.DataAnnotations;

namespace PRUNStatsCommon
{
    public abstract class EntityModelFullBase : EntityModelBase
    {
        public Guid? PRGUID { get; set; }
    }
}

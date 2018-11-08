using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TainingRegisterWindow
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string registerDaysVal { get; set; }

        [StringLength(75)]
        public string registerDaysTxt { get; set; }
    }
}
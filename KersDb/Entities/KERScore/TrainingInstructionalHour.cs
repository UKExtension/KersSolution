 using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TainingInstructionalHour
    {
        [Key]
        public int Id { get; set; }

        [StringLength(25)]
        public string iHoursTxt { get; set; }

        public int iHourValue { get; set; }
    }
}
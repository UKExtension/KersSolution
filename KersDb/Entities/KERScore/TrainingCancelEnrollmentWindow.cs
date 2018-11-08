using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TrainingCancelEnrollmentWindow{
        [Key]
        public int Id { get; set; }

        public int cancelDaysVal { get; set; }

        [StringLength(75)]
        public string cancelDaysTxt { get; set; }
    }
}
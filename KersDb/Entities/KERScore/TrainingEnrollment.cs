using System;
using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TrainingEnrollment{
        [Key]
        public int Id { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(3)]
        public string puid { get; set; }

        [StringLength(50)]
        public PlanningUnit PlanningUnit { get; set; }

        [StringLength(8)]
        public KersUser Attendie { get; set; }

        [StringLength(25)]
        public string TrainingId { get; set; }

        [StringLength(25)]
        public string eStatus { get; set; }

        public DateTime? enrolledDate { get; set; }

        public DateTime? cancelledDate { get; set; }

        public bool? attended { get; set; }

        public bool? evaluationMessageSent { get; set; }
    }
}
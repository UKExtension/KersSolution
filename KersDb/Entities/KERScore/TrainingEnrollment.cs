using System;
using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TrainingEnrollment{
        [Key]
        public int Id { get; set; }
        public DateTime? rDT { get; set; }
        public string puid { get; set; }
        public int PlanningUnitId { get; set; }
        public PlanningUnit PlanningUnit { get; set; }
        public KersUser Attendie { get; set; }
        public int AttendieId { get; set; }
        public string TrainingId { get; set; }
        public string eStatus { get; set; }
        public DateTime? enrolledDate { get; set; }
        public DateTime? cancelledDate { get; set; }
        public bool? attended { get; set; }
        public bool? evaluationMessageSent { get; set; }
    }
}
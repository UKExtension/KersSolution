using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{
    public partial class SnapEd_Commitment
    {
        public int Id { get; set; }
        public int? SnapEd_ActivityTypeId { get; set; }
        public SnapEd_ActivityType SnapEd_ActivityType {get;set;}
        public int? SnapEd_ProjectTypeId { get; set; }
        public SnapEd_ProjectType SnpaEdProjectType {get;set;}
        public int? Common_FiscalYearId { get; set; }
        [ForeignKey("Common_FiscalYearId")]
        public FiscalYear FiscalYear {get;set;}
        public int? KersUserId { get; set; }
        public int? KersUserId1 { get; set; }
        [ForeignKey("KersUserId1")]
        public KersUser KersUser {get;set;}
        public int? Amount { get; set; }
    }
}

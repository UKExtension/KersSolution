using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore{
    public partial class SnapEd_ActivityType
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Measurement { get; set; }
        public int? Common_FiscalYearId { get; set; }
        [ForeignKey("Common_FiscalYearId")]
        public FiscalYear FiscalYear {get;set;}
        public byte? PerProject { get; set; }
    }
}

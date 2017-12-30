using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{
    public partial class SnapEd_ProjectType
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public int? Common_FiscalYearId { get; set; }
        [ForeignKey("Common_FiscalYearId")]
        public FiscalYear FiscalYear {get;set;}
    }
}

namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zCesTaxExemptFinancialYearLookup")]
    public partial class zCesTaxExemptFinancialYearLookup
    {
        [Key]
        public int rID { get; set; }

        [StringLength(100)]
        public string fYearTitle { get; set; }
    }
}

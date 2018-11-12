namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("zCesTaxExemptHowFundsHandledLookup")]
    public partial class zCesTaxExemptHowFundsHandledLookup
    {
        [Key]
        public int fundsHandledID { get; set; }

        [StringLength(300)]
        public string fundsHandledDescBak { get; set; }

        [StringLength(300)]
        public string fundsHandledDesc { get; set; }

        public int? exemptStatusDerivedFromID { get; set; }

        [StringLength(100)]
        public string exemptStatusDerivedFromDesc { get; set; }
    }
}

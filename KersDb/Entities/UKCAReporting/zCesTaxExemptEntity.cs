namespace Kers.Models.Entities.UKCAReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("zCesTaxExemptEntity")]
    public partial class zCesTaxExemptEntity
    {
        [Key]
        [Column(Order = 0)]
        public int rID { get; set; }

        [Column(Order = 1)]
        public DateTime rDT { get; set; }

        public int? oldEntityID { get; set; }

        [StringLength(8)]
        public string rBY { get; set; }

        [StringLength(200)]
        public string rBYName { get; set; }

        [StringLength(10)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(200)]
        public string eName { get; set; }

        [StringLength(50)]
        public string eID { get; set; }

        [StringLength(200)]
        public string eBankName { get; set; }

        [StringLength(200)]
        public string eBankAcct { get; set; }

        [StringLength(50)]
        public string eFinancialYear { get; set; }

        public bool? eProgANR { get; set; }

        public bool? eProgHORT { get; set; }

        public bool? eProgFCS { get; set; }

        public bool? eProg4HYD { get; set; }

        public bool? eProgCED { get; set; }

        public bool? eProgFA { get; set; }

        [StringLength(100)]
        public string eProgComposite { get; set; }

        [StringLength(20)]
        public string DonorsReceivedAck { get; set; }

        public int? eTaxStatusDerivedFromID { get; set; }

        public int? eTaxExemptSrcFundsHandledID { get; set; }

        [StringLength(200)]
        public string eTaxExemptSrcExtDist_DistName { get; set; }

        [StringLength(50)]
        public string eTaxExemptSrcExtDist_EIN { get; set; }

        [StringLength(200)]
        public string eTaxExemptSrc501c_orgName { get; set; }

        [StringLength(50)]
        public string ein501c { get; set; }

        public int? eTaxExemptSrc501cResidesFIPs { get; set; }

        [StringLength(25)]
        public string dtDocAnnBudget { get; set; }

        [StringLength(25)]
        public string dtDocAnnFinancialRpt { get; set; }

        [StringLength(25)]
        public string dtDocAnnAuditRpt { get; set; }

        [StringLength(25)]
        public string dtDocAnnInvRpt { get; set; }

        [StringLength(25)]
        public string dtDocIRSLOD { get; set; }

        [StringLength(25)]
        public string dtDocMOU { get; set; }

        [StringLength(25)]
        public string dtDocIRS990 { get; set; }

        [StringLength(1000)]
        public string eEntityGeoRepresentFIPs { get; set; }

        [StringLength(1700)]
        public string eEntityGeoRepresentCntyNames { get; set; }
    }
}

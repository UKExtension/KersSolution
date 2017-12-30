using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class SAP_HR_ACTIVE
    {
        [Key]
        public int rID { get; set; }

        [StringLength(8)]
        public string PersonID { get; set; }

        [StringLength(8)]
        public string PERNR { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(12)]
        public string JCC { get; set; }

        [StringLength(40)]
        public string Title { get; set; }

        [StringLength(10)]
        public string CostCenter { get; set; }

        [StringLength(4)]
        public string BusArea { get; set; }

        [StringLength(10)]
        public string OrgUnit { get; set; }

        [StringLength(5)]
        public string DeptCode { get; set; }

        [StringLength(1)]
        public string Prime { get; set; }

        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(40)]
        public string Lname { get; set; }

        [StringLength(40)]
        public string Fname { get; set; }

        [StringLength(1)]
        public string MI { get; set; }

        [StringLength(8)]
        public string PosNo { get; set; }

        [StringLength(12)]
        public string Userid { get; set; }

        [StringLength(5)]
        public string SAP_weekly_hrs { get; set; }

        [StringLength(5)]
        public string SAP_daily_hrs { get; set; }

        [StringLength(1)]
        public string activeAssignment { get; set; }

        public DateTime? dtInsert { get; set; }
    }
}

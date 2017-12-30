using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzPlanningUnit
    {
        [Key]
        public int rID { get; set; }

        public int? orderID { get; set; }

        public int? regID { get; set; }

        public int? distID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(250)]
        public string planningUnitName { get; set; }

        public bool? reportsExtension { get; set; }
        public List<zEmpRptProfile> zEmpRptProfiles { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzExtensionPosition
    {
        [Key]
        public int rID { get; set; }

        public int? orderID { get; set; }

        [StringLength(50)]
        public string posCode { get; set; }

        [StringLength(300)]
        public string posTitle { get; set; }
        public List<zEmpRptProfile> zEmpRptProfiles { get; set; }
    }
}

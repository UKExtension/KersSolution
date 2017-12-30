using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzGeneralLocation:IEntityBase
    {
        [Key]
        [Column("rID")]
        public int Id { get; set; }

        public int? orderID { get; set; }

        [StringLength(50)]
        public string locationID { get; set; }

        [StringLength(300)]
        public string locationName { get; set; }
        public List<zEmpRptProfile> zEmpRptProfiles { get; set; }
    }
}

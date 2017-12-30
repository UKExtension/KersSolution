using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERS2017
{

    public partial class zMap
    {
        [Key]
        public int mapID { get; set; }

        public DateTime? rDT { get; set; }

        [StringLength(8)]
        public string rBY { get; set; }

        [StringLength(200)]
        public string rByName { get; set; }

        public int? FY { get; set; }

        [StringLength(10)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(200)]
        public string mapTitle { get; set; }
    }
}

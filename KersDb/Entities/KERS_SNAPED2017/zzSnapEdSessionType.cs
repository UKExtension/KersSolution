using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERS_SNAPED2017
{

    [Table("zzSnapEdSessionTypes")]
    public partial class zzSnapEdSessionType
    {
        [Key]
        public int rID { get; set; }

        public int? FY { get; set; }

        public int? orderID { get; set; }
        public string snapDirectSessionTypeName {get;set;}


    }
}

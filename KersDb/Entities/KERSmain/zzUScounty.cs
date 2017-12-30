using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    public partial class zzUScounty
    {
        [Key]
        public int cntyFIPS { get; set; }

        [StringLength(75)]
        public string cntyName { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int stateFIPS { get; set; }

        [StringLength(75)]
        public string stateName { get; set; }

        [StringLength(50)]
        public string stateNameShort { get; set; }
    }
}

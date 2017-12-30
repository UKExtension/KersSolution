using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    [Table("zEmpRoleTypes")]
    public partial class zEmpRoleType : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? created { get; set; }

        public DateTime? updated { get; set; }

        public bool enabled { get; set; }

        public bool selfEnrolling { get; set; }

        [StringLength(50)]
        public string shortTitle { get; set; }

        [StringLength(250)]
        public string title { get; set; }

        public string description { get; set; }

        public List<zEmpProfileRole> empRoles {get; set;}

    }
}

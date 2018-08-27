using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    [Table("zEmpProfileRoles")]
    public partial class zEmpProfileRole:IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public int zEmpRoleTypeId { get; set; } 
        public zEmpRoleType zEmpRoleType { get; set; }     
        public KersUser User { get; set; }
    }
}

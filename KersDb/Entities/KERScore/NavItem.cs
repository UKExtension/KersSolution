using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class NavItem:IEntityBase, IEntityCredentials
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public String name {get; set;}
        public Boolean isRelative {get; set;}
        public String route {get; set;}
        public int order {get; set;}
        public int? EmployeePositionId {get; set;}
        public int? zEmpRoleTypeId {get; set;}
        public int? isContyStaff {get;set;}
        
    }
}
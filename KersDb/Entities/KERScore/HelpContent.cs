using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class HelpContent : IEntityBase, IEntityCredentials
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String Title {get;set;}
        [Column(TypeName = "text")]
        public String Body {get; set;}
        public int CategoryId {get; set;}
        public HelpCategory Category {get; set;}
        public DateTime Created {get; set;}
        public DateTime Updated {get; set;}
        public KersUser CreatedBy {get; set;}
        public KersUser LastUpdatedBy {get; set;}
        public int? EmployeePositionId {get; set;}
        public int? zEmpRoleTypeId {get; set;}
        public int? isContyStaff {get;set;}
    }
}

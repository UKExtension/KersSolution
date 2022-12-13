using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class HelpCategory : IEntityBase, IEntityCredentials
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String Title {get;set;}
        [Column(TypeName = "text")]
        public String Description {get; set;}
        public int ParentId {get; set; }
        public HelpCategory Parent {get; set;}
        public DateTime Created {get; set;}
        public DateTime Updated {get; set;}
        public KersUser CreatedBy {get; set;}
        public KersUser LastUpdatedBy {get; set;}
        [ForeignKey("CategoryId")]
        public List<HelpContent> HelpContents {get;set;}
        public int? EmployeePositionId {get; set;}
        public int? zEmpRoleTypeId {get; set;}
        public int? isContyStaff {get;set;}
    }
}

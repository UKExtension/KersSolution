using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderStage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string Restriction { get; set; } // Restrict control to area, region or district
        public ICollection<LadderStageRole> LadderStageRoles {get;set;}
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class AffirmativeMakeupDiversityTypeGroup : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order {get; set;}
        public Boolean Render {get;set;}
        public List<AffirmativeMakeupDiversityType> Types {get; set;}
        
    }
}
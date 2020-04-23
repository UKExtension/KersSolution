using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderKersUserLevel : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public KersUser KersUser { get; set; }
        public LadderLevel LadderLevel {get;set;}
        public LadderApplication LadderApplication {get;set;}
        
    }
}
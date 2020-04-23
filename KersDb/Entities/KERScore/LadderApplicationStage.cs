using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderApplicationStage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public LadderApplication LadderApplication { get; set; }
        public DateTime Created { get; set; }
        public LadderStage LadderStage {get;set;}
        public KersUser KersUser {get;set;}
        public string  Note {get;set;}
        
    }
}
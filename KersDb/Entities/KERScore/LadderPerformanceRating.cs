using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderPerformanceRating : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public LadderApplication LadderApplication {get;set;}
        public int LadderApplicationId {get;set;}
        public string Year { get; set; }
        public string Ratting {get;set;}
        public int Order { get; set; }
        
    }
}
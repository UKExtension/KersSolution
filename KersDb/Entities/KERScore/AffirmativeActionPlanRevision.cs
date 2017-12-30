using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class AffirmativeActionPlanRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String Agents {get;set;}
        [Column(TypeName = "text")]
        public String Description {get;set;}
        [Column(TypeName = "text")]
        public String Goals {get;set;}
        [Column(TypeName = "text")]
        public String Strategies {get;set;}
        [Column(TypeName = "text")]
        public String Efforts {get;set;}
        [Column(TypeName = "text")]
        public String Success {get;set;}
        public List<AffirmativeMakeupValue> MakeupValues {get;set;}
        public List<AffirmativeSummaryValue> SummaryValues {get;set;}
        public KersUser CreatedBy {get;set;}
        public DateTime Created {get;set;}
        
    }
}
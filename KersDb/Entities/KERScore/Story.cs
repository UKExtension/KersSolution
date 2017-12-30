using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Story : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public int? PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public List<StoryRevision> Revisions {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}
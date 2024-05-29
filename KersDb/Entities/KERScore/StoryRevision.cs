using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class StoryRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StoryId {get;set;}
        public DateTime Created {get;set;}
        public int MajorProgramId {get;set;}
        public MajorProgram MajorProgram {get;set;}
        public int PlanOfWorkId {get;set;}
        public PlanOfWork PlanOfWork{get;set;}
        public int StoryOutcomeId {get;set;}
        public StoryOutcome StoryOutcome {get;set;}
        public List<StoryImage> StoryImages {get;set;}
        public string Title {get;set;}
        [Column(TypeName = "text")]
        public string Story {get;set;}
        public int Reach {get;set;}
        public string AudienceOther {get;set;}
        public List<StoryAudienceConnection> StoryAudienceConnections {get;set;}
        public bool IsSnap {get;set;}

    }
}
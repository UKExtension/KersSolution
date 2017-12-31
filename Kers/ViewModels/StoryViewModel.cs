using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{

    public class StoryViewModel
    {
        public int StoryId {get;set;}
        public KersUser KersUser { get; set; }
        public PlanningUnit PlanningUnit { get; set; }
        public DateTime Updated { get; set; }
        public MajorProgram MajorProgram {get;set;}
        public PlanOfWorkRevision PlanOfWork {get;set;}
        public StoryOutcome StoryOutcome {get;set;}
        public string Title {get;set;}
        public string Story {get;set;}
        public string ImageName {get;set;}

    }
}
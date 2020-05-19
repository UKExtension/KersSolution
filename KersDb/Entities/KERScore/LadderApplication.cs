using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderApplication : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public LadderLevel LadderLevel {get;set;}
        public int LadderLevelId {get;set;}
        public string PositionNumber { get; set; }
        public string ProgramOfStudy {get;set;}
        public string Evidence {get;set;}
        public int? NumberOfYears {get;set;}
        public DateTime LastPromotion {get;set;}
        public DateTime StartDate {get;set;}
        public LadderEducationLevel LadderEducationLevel {get;set;}
        public int LadderEducationLevelId {get;set;}
        public KersUser KersUser { get; set; }
        public int KersUserId { get; set; }
        public Boolean Draft {get;set;}
        public Boolean Approved {get;set;}
        public LadderStage LastStage {get;set;}
        public int? LastStageId {get;set;}
        public List<LadderApplicationStage> Stages {get;set;}
        public List<LadderPerformanceRating> Ratings {get;set;}
        public List<LadderImage> Images {get;set;}
        public DateTime Created {get;set;}
        public DateTime LastUpdated {get;set;}
    }
}
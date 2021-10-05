using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Activity : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public int? PlanningUnitId {get;set;}
        public DateTime ActivityDate {get;set;}
        public float Hours {get;set;}
        public string Title {get;set;}
        public int Audience {get;set;}
        public int MajorProgramId {get;set;}
        public MajorProgram MajorProgram {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public List<ActivityRevision> Revisions {get;set;}
        [ForeignKey("LastRevisionId")]
        public ActivityRevision LastRevision {get;set;}
        public int LastRevisionId {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
        public List<ActivityImage> ActivityImages {get;set;}
        public List<ActivitySignUpEntry> ActivitySignUpEntrys {get;set;}
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Contact : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public DateTime ContactDate {get;set;}
        public int Audience {get;set;}
        public float Days{get;set;}
        public int? PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get; set;}
        public int MajorProgramId {get;set;}
        public MajorProgram MajorProgram {get;set;}
        public List<ContactRevision> Revisions {get;set;}
        public ContactRevision LastRevision {get;set;}
        public int? LastRevisionId {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}
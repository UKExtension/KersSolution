using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ActivityRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ActivityId {get;set;}
        public DateTime Created {get;set;}
        public DateTime ActivityDate {get;set;}
        public float Hours{get;set;}
        public int MajorProgramId {get;set;}
        public MajorProgram MajorProgram {get;set;}
        public string Title {get;set;}
        [Column(TypeName = "text")]
        public string Description {get;set;}
        public List<ActivityOptionSelection> ActivityOptionSelections {get;set;}
        public List<RaceEthnicityValue> RaceEthnicityValues {get;set;}
        public int Female {get;set;}
        public int Male {get;set;}
        public List<ActivityOptionNumberValue> ActivityOptionNumbers {get;set;}
        public bool isSnap {get;set;}
        public int? classicSnapId {get;set;}
        public int? classicIndirectSnapId {get;set;}
        public int? SnapDirectId {get;set;}
        public SnapDirect SnapDirect {get;set;}
        public int? SnapIndirectId {get;set;}
        public SnapIndirect SnapIndirect {get;set;}
        public int? SnapPolicyId {get;set;}
        public SnapPolicy SnapPolicy {get;set;}
        public int SnapCopies {get;set;}
        public int SnapCopiesBW {get;set;}
        public Boolean SnapAdmin {get;set;}
        public Boolean IsPolicy {get;set;}
    }
}
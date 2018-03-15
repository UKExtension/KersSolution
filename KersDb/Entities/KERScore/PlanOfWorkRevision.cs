using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class PlanOfWorkRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title {get; set;}
        public string AgentsInvolved {get; set;}
        public int? Mp1Id {get; set;}
        public MajorProgram Mp1 {get; set;}
        public int? Mp2Id {get; set;}
        public MajorProgram Mp2 {get; set;}
        public int? Mp3Id {get; set;}
        public MajorProgram Mp3 {get; set;}
        public int? Mp4Id {get; set;}
        public MajorProgram Mp4 {get; set; }
        [Column(TypeName="text")]
        public string Situation {get; set;}
        [Column(TypeName="text")]
        public string LongTermOutcomes {get; set;}
        [Column(TypeName="text")]
        public string IntermediateOutcomes {get; set;}
        [Column(TypeName="text")]
        public string InitialOutcomes {get; set;}
        [Column(TypeName="text")]
        public string Learning {get; set;}
        [Column(TypeName="text")]
        public string Evaluation {get; set;}
        public DateTime Created {get; set;}
        public KersUser By {get; set;}
        public Map Map {get; set;}

        public int PlanOfWorkId { get; set; }
        public PlanOfWork PlanOfWork { get; set; }

    }
}
using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class ProgramIndicator : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName="text")]
        public string Question {get; set;}
        public int order {get; set;}
        public int MajorProgramId {get; set;}
        public MajorProgram MajorProgram {get; set;}
        public int IsYouth {get; set;}
        public int Identifier {get; set;}

    }
}

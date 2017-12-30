using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class MajorProgram : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ShortName {get; set;}
        public string Name {get; set;}
        public int PacCode {get; set;}
        public int order {get; set;}
        public StrategicInitiative StrategicInitiative {get;set;}
        public ProgramCategory ProgramCategory {get; set;}
        public List<ProgramIndicator> ProgramIndicators {get;set;}

    }
}

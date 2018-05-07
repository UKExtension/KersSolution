using System.Collections.Generic;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Data{
    public class PerProgramActivities{
        public List<IRaceEthnicityValue> RaceEthnicityValues {get;set;}
        public List<IOptionNumberValue> OptionNumberValues {get; set;}
        public float Hours {get;set;}
        public float Multistate {get;set;}
        public int Audience {get;set;}
        public int Female {get;set;}
        public int Male {get;set;}
        public MajorProgram MajorProgram {get;set;}
    }
}

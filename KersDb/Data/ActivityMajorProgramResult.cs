using System.Collections.Generic;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Data{
    public class ActivityMajorProgramResult{
        public List<int> Ids;
        public float Hours;
        public int Audience;
        public MajorProgram MajorProgram;
    }
}

using System.Collections.Generic;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Data{
    public class ActivityRevisionsPerMonth{
        public List<ActivityRevision> Revs;
        public int Month;
        public int Year;
    }
}

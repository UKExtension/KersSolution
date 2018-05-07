using System.Collections.Generic;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Data{
    public class ActivityPersonResult{
        public List<int> Ids;
        public float Hours;
        public int Audience;
        public int Female {get;set;}
        public int Male {get;set;}
        public KersUser KersUser;
    }
}

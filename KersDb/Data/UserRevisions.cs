using System.Collections.Generic;
using Kers.Models.Entities.KERScore;
namespace Kers.Models.Data{
    public class UserRevisions{
        public KersUser User;
        public List<ActivityRevision> Revisions;
    }
}
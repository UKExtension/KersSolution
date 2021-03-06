using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;

namespace Kers.Models.Repositories
{
    public class HelpContentRepository : EntityBaseRepository<HelpContent>, IHelpContentRepository
    {

        public HelpContentRepository(KERScoreContext context)
            : base(context)
        { }
    }
}
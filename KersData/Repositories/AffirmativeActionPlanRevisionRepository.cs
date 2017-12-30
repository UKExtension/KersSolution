using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;

namespace Kers.Models.Repositories
{
    public class AffirmativeActionPlanRevisionRepository : EntityBaseRepository<AffirmativeActionPlanRevision>, IAffirmativeActionPlanRevisionRepository
    {

        private KERScoreContext coreContext;
        public AffirmativeActionPlanRevisionRepository(KERScoreContext context) : base(context)
        { 
            this.coreContext = context;
        }

    }
}
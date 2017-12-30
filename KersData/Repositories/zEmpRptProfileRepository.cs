using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Contexts;

namespace Kers.Models.Repositories
{
    public class zEmpRptProfileRepository : EntityBaseRepository<zEmpRptProfile>, IzEmpRptProfileRepository
    {
        public zEmpRptProfileRepository(KERSmainContext context)
            : base(context)
        { }
    }
}
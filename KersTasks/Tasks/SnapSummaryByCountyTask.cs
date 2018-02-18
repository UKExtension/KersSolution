using System;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Tasks{
    class SnapSummaryByCountyTask:TaskBase, ITask{

        public SnapSummaryByCountyTask(KERScoreContext context, IDistributedCache cache):base(context, cache){

        }
        public object run(string[] arguments){

            var fiscalYearRepo = new FiscalYearRepository( context );
            var repo = new SnapDirectRepository(context, cache);
            var str = repo.TotalByCounty(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
            return str;
                      
        }
    }
}
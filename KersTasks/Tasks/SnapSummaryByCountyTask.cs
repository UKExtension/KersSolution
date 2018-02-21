using System;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Tasks{
    class SnapSummaryByCountyTask:TaskBase, ITask{

        public SnapSummaryByCountyTask(KERScoreContext context, KERSmainContext mainContext, IDistributedCache cache):base(context, mainContext, cache){

        }
        public object run(string[] arguments){

            var fiscalYearRepo = new FiscalYearRepository( context );
            var repo = new SnapDirectRepository(context, cache, mainContext);
            var str = repo.TotalByCounty(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
            return str;
                      
        }
    }
}
using System;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Tasks{
    class SnapSummaryByEmployeeTask:TaskBase, ITask{

        public SnapSummaryByEmployeeTask(KERScoreContext context, IDistributedCache cache):base(context, cache){

        }
        public object run(string[] arguments){

            var fiscalYearRepo = new FiscalYearRepository( context );
            var repo = new SnapDirectRepository(context, cache);
            var str = repo.TotalByEmployee(fiscalYearRepo.currentFiscalYear(FiscalYearType.SnapEd), true);
            return str;
                      
        }
    }
}
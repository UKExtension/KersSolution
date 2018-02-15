using System;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Kers.Tasks{
    class SnapSummaryByMonthTask:TaskBase, ITask{

        public SnapSummaryByMonthTask(KERScoreContext context, IDistributedCache cache):base(context, cache){

        }
        public void run(string[] arguments){
            var fiscalYearRepo = new FiscalYearRepository( context );

            var repo = new SnapDirectRepository(context, cache);
            

            var str = repo.TotalByMonth(fiscalYearRepo.currentFiscalYear("snapEd"), true);
            Console.WriteLine(str);
        }
    }
}
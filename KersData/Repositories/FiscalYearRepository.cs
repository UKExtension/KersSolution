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
    public class FiscalYearRepository : EntityBaseRepository<FiscalYear>, IFiscalYearRepository
    {

        private KERScoreContext coreContext;
        public FiscalYearRepository(KERScoreContext context)
            : base(context)
        { 
            coreContext = context;
        }

        public FiscalYear currentFiscalYear(string type){
            var current = this.coreContext.
                        FiscalYear.
                        Where(y => y.Start < DateTime.Now && y.End > DateTime.Now && y.Type == type).
                        FirstOrDefault();
            if(current == null){
                current = this.coreContext.FiscalYear.Where( y => y.Name=="2018" && y.Type== type).FirstOrDefault();
            }
            return current;
        }

        public FiscalYear byName(string name, string type){
            return this.coreContext
                        .FiscalYear
                        .Where( y => y.Name == name && y.Type == type)
                        .FirstOrDefault();
        }

        public FiscalYear nextFiscalYear(string type){

            var nextYear = DateTime.Now.AddYears( 1 );
            
            return this.coreContext.
                        FiscalYear.
                        Where(y => y.Start < nextYear && y.End > nextYear && y.Type == type).
                        FirstOrDefault();
            
        }
    }
}
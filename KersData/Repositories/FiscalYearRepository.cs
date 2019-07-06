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

        public FiscalYear currentFiscalYear(string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false){
            
            var today = DateTime.Now;
            
            var currentYear = this.byDate( today, type, includeExtendedTo, afterAvailableAt);
            if(currentYear == null){
                currentYear = this.coreContext.FiscalYear.Where( y => y.Name=="2020" && y.Type== type).FirstOrDefault();
            }
            return currentYear;
        }

        public FiscalYear byName(string name, string type){
            return this.coreContext
                        .FiscalYear
                        .Where( y => y.Name == name && y.Type == type)
                        .FirstOrDefault();
        }

        public FiscalYear byDate(DateTime date, string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false){
           
            date = new DateTime( date.Year, date.Month, date.Day, 8, 0, 0);
            
            var year = this.coreContext.FiscalYear.Where(y => y.Type == type);

            if(includeExtendedTo){
                year = year.Where( y => 

                                (
                                        y.ExtendedTo == null || y.ExtendedTo < y.End 
                                    ? 
                                        new DateTime( y.End.Year, y.End.Month, y.End.Day, 8, 0, 0) 
                                    :
                                        new DateTime( y.ExtendedTo.Year, y.ExtendedTo.Month, y.ExtendedTo.Day, 8, 0, 0)
                                )
                
                                
                                >= 
                                
                                date 

                            );
            }else{
                year = year.Where( y => new DateTime( y.End.Year, y.End.Month, y.End.Day, 8, 0, 0) >= date );
            }
            if(afterAvailableAt){
                year = year.Where( y => 

                                (
                                        y.AvailableAt == null || y.AvailableAt < y.Start 
                                    ? 
                                        new DateTime( y.Start.Year, y.Start.Month, y.Start.Day, 8, 0, 0) 
                                    :
                                        new DateTime( y.AvailableAt.Year, y.AvailableAt.Month, y.AvailableAt.Day, 8, 0, 0)
                                )
                
                                
                                <= 
                                
                                date 

                            );
            }else{
                year = year.Where( y => new DateTime( y.Start.Year, y.Start.Month, y.Start.Day, 8, 0, 0) <= date );
            }

            var theYear = year.OrderBy( y => y.Start).FirstOrDefault();

            return theYear;
        }

        public FiscalYear nextFiscalYear(string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false){



            var CurrentFiscalYear = this.currentFiscalYear(type, includeExtendedTo, afterAvailableAt);


            var dateThatShouldBeInTheNextFiscalYear = CurrentFiscalYear.End.AddMonths(1);
            
            /* 
            var nextYear = DateTime.Now.AddYears( 1 );
            var year = this.coreContext.
                        FiscalYear.
                        Where(y => y.Start < nextYear && y.End > nextYear && y.Type == type).
                        FirstOrDefault();
 */
            var year = this.byDate(dateThatShouldBeInTheNextFiscalYear, type);
            if( year == null ) year = CurrentFiscalYear;
            return year;
            
        }


        public FiscalYear previoiusFiscalYear( string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false ){
            var previousYear = DateTime.Now.AddYears( -1 );
            var year = this.coreContext.
                        FiscalYear.
                        Where(y => y.Start < previousYear && y.End > previousYear && y.Type == type).
                        FirstOrDefault();
            if( year == null){
                year = this.currentFiscalYear(type);
            }
            return year;
        }
    }
}
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Kers.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kers.Models.Contexts.Initializers
{

    

    public static class SeedData
    {
        
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<KERScoreContext>();
            //TransferSnapEdToNextFiscalYear(context);
        }
        private static void TransferSnapEdToNextFiscalYear(KERScoreContext db){
            var nextYear = DateTime.Now.AddYears( 1 );
            
            var nextFiscalYear =  db.FiscalYear.
                        Where(y => y.Start < nextYear && y.End > nextYear && y.Type == FiscalYearType.SnapEd).
                        FirstOrDefault();
            var currentFiscalYear = db.FiscalYear.
                        Where(y => y.Start < DateTime.Now && y.End > DateTime.Now && y.Type == FiscalYearType.SnapEd).
                        FirstOrDefault();
            // Check if fiscal years are defined
            if(nextFiscalYear != null && currentFiscalYear != null){
                // Be sure there are no activity types for the next fiscal year
                if( db.SnapEd_ActivityType.Where(a => a.FiscalYear.Id == nextFiscalYear.Id).FirstOrDefault() == null){
                    var currentFiscalYearActivityTypes = db.SnapEd_ActivityType.Where(a => a.Common_FiscalYearId == currentFiscalYear.Id);
                    foreach( var act in currentFiscalYearActivityTypes){
                        var newActivity = new SnapEd_ActivityType();
                        newActivity.FiscalYear = nextFiscalYear;
                        newActivity.Name = act.Name;
                        newActivity.Measurement = act.Measurement;
                        newActivity.PerProject = act.PerProject;
                        db.Add(newActivity);
                    }
                    var currentFiscalYearProjectTypes = db.SnapEd_ProjectType.Where( p => p.Common_FiscalYearId == currentFiscalYear.Id);
                    foreach( var prj in currentFiscalYearProjectTypes){
                        var newProject = new SnapEd_ProjectType();
                        newProject.FiscalYear = nextFiscalYear;
                        newProject.Name = prj.Name;
                        db.Add(newProject);
                    }
                    //var currentFiscalYearReinforcementItems = db.SnapEd_ReinforcementItem.Where( i => i.Common_FiscalYearId == currentFiscalYear.Id);
                    var items = new string[]{
                        "Dry measuring cups",
                        "Meat thermometers",
                        "Measuring spoons",
                        "Nylon spatula/turner",
                        "Rubber jar openers",
                        "Youth water bottles",
                        "Youth mini footballs",
                        "Gardening gloves",
                        "Farmers market tote bags"
                    };
                    foreach( var itm in items){
                        var newItem = new SnapEd_ReinforcementItem();
                        newItem.FiscalYear = nextFiscalYear;
                        newItem.Name = itm;
                        db.Add(newItem);
                    }

                    db.SaveChanges();

                }
            }
        }



            
            
    }
}
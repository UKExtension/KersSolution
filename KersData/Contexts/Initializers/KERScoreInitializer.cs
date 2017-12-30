using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;


namespace Kers.Models.Contexts
{

    

    public static class KERScoreContextExtensions
    {
        
        public static void Seed(this KERScoreContext db)
        {
            //AddAffirmativeGroupType(db); 
            //AddAffirmativeMakeupDiversityTypeGroup(db);
            //AddAffirmativeSummaryDiversityType(db);
            //SocialConnectionType(db);
            //GeneralLocation(db);
            //Institution(db);
            //Specialty(db);
            
            //ExpenseFundingSource(db);
            //ExpenseMileageRate(db;
            //ExpenseMealRate(db);
            
            //Race(db);
            //Ethnicity(db);
            //ActivityOption(db);
            //ActivityOptionNumber(db);
            //StoryOutcome(db);
            //ActivityDate(db);
            //ExpenseDate(db);
            //CountyData(db);


            //SnapDirectAges(db);
            //SnapDirectAudience(db);
            //SnapDirectDeliverySite(db);
            //SnapDirectSessionType(db);
            //SnapIndirectMethod(db);
            //SnapIndirectReached(db);
            //SnapPolicyAimed(db);
            //SnapPolicyPartner(db);


            //ActivityDetails(db);


            
            //SnapCommitment(db);
            //SnapBudgetAllowance(db);
            //SnapEdCountyBudget(db);
            //IntoMainContact(db);
        }



        private static void IntoMainContact(KERScoreContext db){
            var sel = db.Contact.Where( r => true).Include( c => c.Revisions);
            foreach(var contact in sel){
                if(contact.Revisions != null){
                    var last = contact.Revisions.OrderBy(r => r.Created).Last();
                    contact.MajorProgramId = last.MajorProgramId;
                    contact.ContactDate = last.ContactDate;
                    contact.Audience = last.Male + last.Female;
                    contact.Days = last.Days;
                }
            }
            
            db.SaveChanges();

        }
        private static void SnapCommitment(KERScoreContext db){
            var sel = db.SnapEd_Commitment;
            foreach(var comm in sel){
                comm.KersUser = db.KersUser.Where(k => k.classicReportingProfileId == comm.KersUserId).FirstOrDefault();
            }
            var itm = db.SnapEd_ReinforcementItemChoice;
            foreach( var ic in itm){
                ic.KersUser = db.KersUser.Where(k => k.classicReportingProfileId == ic.zEmpProfileId).FirstOrDefault();
            }
            var sgst = db.SnapEd_ReinforcementItemSuggestion;
            foreach( var sg in sgst){
                sg.KersUser = db.KersUser.Where( k => k.classicReportingProfileId == sg.zEmpProfileId).FirstOrDefault();
            }
            db.SaveChanges();

        }
        private static void SnapEdCountyBudget(KERScoreContext db){
            if (db.SnapCountyBudget.Any())
            {
                return;   // DB has been seeded
            }
            var counties = db.PlanningUnit.Where(p => p.Name.Substring(p.Name.Length - 3) == "CES");
            foreach( var county in counties){
                var budget = new SnapCountyBudget();
                budget.AnnualBudget = 2100;
                budget.PlanningUnit = county;
                budget.FiscalYearId = 1;
                budget.ById = 1;
                budget.Updated = DateTime.Now;
                db.Add(budget);
            }
            db.SaveChanges();
        }
        private static void SnapBudgetAllowance(KERScoreContext db){
            if (db.SnapBudgetAllowance.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapBudgetAllowance{ BudgetDescription = "SNAP Ed NEP Assistant Budget", FiscalYearId = 1, AnnualBudget = 1800},
                    new SnapBudgetAllowance{ BudgetDescription = "SNAP Ed County Budget (separate from NEP Assistant Budget)", FiscalYearId = 1, AnnualBudget = 2100}
            );
            db.SaveChanges();
        }

        private static void ActivityDetails(KERScoreContext db){
            var sel = db.Activity.
                            Where(a => a.Title == null).
                            Include( a => a.Revisions).
                            Take(1000);
            foreach(var act in sel){
                var last = act.Revisions.OrderBy(r => r.Created).Last();
                act.Title = last.Title;
                act.Audience = last.Male + last.Female;
                act.Hours = last.Hours;
                act.MajorProgramId = last.MajorProgramId;

            }
            db.SaveChanges();
        }

        private static void SnapDirectAges(KERScoreContext db){
            if (db.SnapDirectAges.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapDirectAges{ Name = "Less than 5 Years", FiscalYearId = 1, order = 1},
                    new SnapDirectAges{ Name = "5-17 Years", FiscalYearId = 1, order = 2},
                    new SnapDirectAges{ Name = "18-59 Years", FiscalYearId = 1, order = 3},
                    new SnapDirectAges{ Name = "60+ Years", FiscalYearId = 1, order = 4}
            );
            db.SaveChanges();
        }

        private static void SnapDirectAudience(KERScoreContext db){
            if (db.SnapDirectAudience.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapDirectAudience{ Name = "Famers Markets", FiscalYearId = 1, order = 1},
                    new SnapDirectAudience{ Name = "Pre-School", FiscalYearId = 1, order = 2},
                    new SnapDirectAudience{ Name = "Family", FiscalYearId = 1, order = 3},
                    new SnapDirectAudience{ Name = "School Age", FiscalYearId = 1, order = 4},
                    new SnapDirectAudience{ Name = "Limited English", FiscalYearId = 1, order = 5},
                    new SnapDirectAudience{ Name = "Seniors", FiscalYearId = 1, order = 6}
            );
            db.SaveChanges();
        }

        private static void SnapDirectDeliverySite(KERScoreContext db){
            if (db.SnapDirectDeliverySite.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapDirectDeliverySite{ Name = "Adult Education and Job Training Sites", FiscalYearId = 1, order = 1},
                    new SnapDirectDeliverySite{ Name = "Adult Rehabilitation Centers", FiscalYearId = 1, order = 2},
                    new SnapDirectDeliverySite{ Name = "Churches", FiscalYearId = 1, order = 3},
                    new SnapDirectDeliverySite{ Name = "Community Centers", FiscalYearId = 1, order = 4},
                    new SnapDirectDeliverySite{ Name = "Elderly Service Centers", FiscalYearId = 1, order = 5},
                    new SnapDirectDeliverySite{ Name = "Emergency Food Assistance Sites", FiscalYearId = 1, order = 6},
                    new SnapDirectDeliverySite{ Name = "Extension Offices", FiscalYearId = 1, order = 7},
                    new SnapDirectDeliverySite{ Name = "Farmers Markets/Farms", FiscalYearId = 1, order = 8},
                    new SnapDirectDeliverySite{ Name = "Food Stores/Grocery", FiscalYearId = 1, order = 9},
                    new SnapDirectDeliverySite{ Name = "Head Start Programs and Day Care", FiscalYearId = 1, order = 10},
                    new SnapDirectDeliverySite{ Name = "Individual Homes", FiscalYearId = 1, order = 11},
                    new SnapDirectDeliverySite{ Name = "Libraries", FiscalYearId = 1, order = 12},
                    new SnapDirectDeliverySite{ Name = "Other Youth Education Sites (includes Parks and Recreation)", FiscalYearId = 1, order = 13},
                    new SnapDirectDeliverySite{ Name = "Public Housing", FiscalYearId = 1, order = 14},
                    new SnapDirectDeliverySite{ Name = "Public Schools", FiscalYearId = 1, order = 15},
                    new SnapDirectDeliverySite{ Name = "Public/Community Health Centers/Hospitals", FiscalYearId = 1, order = 16},
                    new SnapDirectDeliverySite{ Name = "Shelters", FiscalYearId = 1, order = 17},
                    new SnapDirectDeliverySite{ Name = "SNAP Offices", FiscalYearId = 1, order = 18},
                    new SnapDirectDeliverySite{ Name = "WIC Programs", FiscalYearId = 1, order = 19},
                    new SnapDirectDeliverySite{ Name = "Worksites", FiscalYearId = 1, order = 20},
                    new SnapDirectDeliverySite{ Name = "Other (please specify)", FiscalYearId = 1, order = 21}
            );
            db.SaveChanges();
        }

        private static void SnapDirectSessionType(KERScoreContext db){
            if (db.SnapDirectSessionType.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapDirectSessionType{ Name = "Single Session", FiscalYearId = 1, order = 1},
                    new SnapDirectSessionType{ Name = "Series - 2 to 4 sessions", FiscalYearId = 1, order = 2},
                    new SnapDirectSessionType{ Name = "Series - 5 to 9 sessions", FiscalYearId = 1, order = 3},
                    new SnapDirectSessionType{ Name = "Series - 10 or more sessions", FiscalYearId = 1, order = 4}
            );
            db.SaveChanges();
        }

        private static void SnapIndirectMethod(KERScoreContext db){
            if (db.SnapIndirectMethod.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapIndirectMethod{ Name = "Fact Sheets, Pamphlets, Newsletters", FiscalYearId = 1, order = 1},
                    new SnapIndirectMethod{ Name = "Posters", FiscalYearId = 1, order = 2},
                    new SnapIndirectMethod{ Name = "Calendars", FiscalYearId = 1, order = 3},
                    new SnapIndirectMethod{ Name = "Promotional Material: Nutritional Message", FiscalYearId = 1, order = 4},
                    new SnapIndirectMethod{ Name = "Website", FiscalYearId = 1, order = 5},
                    new SnapIndirectMethod{ Name = "Email Info Distribution", FiscalYearId = 1, order = 6},
                    new SnapIndirectMethod{ Name = "Video CDROM", FiscalYearId = 1, order = 7},
                    new SnapIndirectMethod{ Name = "Other", FiscalYearId = 1, order = 8}
            );
            db.SaveChanges();
        }

        private static void SnapIndirectReached(KERScoreContext db){
            if (db.SnapIndirectReached.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapIndirectReached{ Name = "Nutrition Education Radio Public Service Announcement", FiscalYearId = 1, order = 1},
                    new SnapIndirectReached{ Name = "Nutrition Education TV Public Service Announcement", FiscalYearId = 1, order = 2},
                    new SnapIndirectReached{ Name = "Nutrition Education Articles", FiscalYearId = 1, order = 3},
                    new SnapIndirectReached{ Name = "Grocery Store", FiscalYearId = 1, order = 4},
                    new SnapIndirectReached{ Name = "Community Event Fairs (Participated)", FiscalYearId = 1, order = 5},
                    new SnapIndirectReached{ Name = "Community Event Fairs (Sponsored)", FiscalYearId = 1, order = 6},
                    new SnapIndirectReached{ Name = "Newsletter Audience", FiscalYearId = 1, order = 7},
                    new SnapIndirectReached{ Name = "Social Media", FiscalYearId = 1, order = 8},
                    new SnapIndirectReached{ Name = "Other", FiscalYearId = 1, order = 9}
            );
            db.SaveChanges();
        }


        private static void SnapPolicyAimed(KERScoreContext db){
            if (db.SnapPolicyAimed.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapPolicyAimed{ Name = "Agriculture", FiscalYearId = 1, order = 1},
                    new SnapPolicyAimed{ Name = "Community Design", FiscalYearId = 1, order = 2},
                    new SnapPolicyAimed{ Name = "Community Safety", FiscalYearId = 1, order = 3},
                    new SnapPolicyAimed{ Name = "Early Childhood", FiscalYearId = 1, order = 4},
                    new SnapPolicyAimed{ Name = "Food Insecurity (food banks/pantries)", FiscalYearId = 1, order = 5},
                    new SnapPolicyAimed{ Name = "Food Retail (grocery store)", FiscalYearId = 1, order = 6},
                    new SnapPolicyAimed{ Name = "Food Service/Fast Food", FiscalYearId = 1, order = 7},
                    new SnapPolicyAimed{ Name = "Homes", FiscalYearId = 1, order = 8},
                    new SnapPolicyAimed{ Name = "Parks & Recreational Areas", FiscalYearId = 1, order = 9},
                    new SnapPolicyAimed{ Name = "Schools & Afterschool Programs", FiscalYearId = 1, order = 10},
                    new SnapPolicyAimed{ Name = "Workplaces", FiscalYearId = 1, order = 11}
            );
            db.SaveChanges();
        }

        private static void SnapPolicyPartner(KERScoreContext db){
            if (db.SnapPolicyPartner.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SnapPolicyPartner{ Name = "Boys N Girls Club", FiscalYearId = 1, order = 1},
                    new SnapPolicyPartner{ Name = "Chamber of Commerce", FiscalYearId = 1, order = 2},
                    new SnapPolicyPartner{ Name = "Civic Organizations/groups", FiscalYearId = 1, order = 3},
                    new SnapPolicyPartner{ Name = "Community Industry", FiscalYearId = 1, order = 4},
                    new SnapPolicyPartner{ Name = "Community Volunteers (Master Program & KEHA)", FiscalYearId = 1, order = 5},
                    new SnapPolicyPartner{ Name = "County/City government", FiscalYearId = 1, order = 6},
                    new SnapPolicyPartner{ Name = "Courts/Court Workers", FiscalYearId = 1, order = 7},
                    new SnapPolicyPartner{ Name = "Daycares", FiscalYearId = 1, order = 8},
                    new SnapPolicyPartner{ Name = "Education", FiscalYearId = 1, order = 9},
                    new SnapPolicyPartner{ Name = "Extension Program Council", FiscalYearId = 1, order = 10},
                    new SnapPolicyPartner{ Name = "Faith Based", FiscalYearId = 1, order = 11},
                    new SnapPolicyPartner{ Name = "Family Resource Centers", FiscalYearId = 1, order = 12},
                    new SnapPolicyPartner{ Name = "Farmers", FiscalYearId = 1, order = 13},
                    new SnapPolicyPartner{ Name = "Food Pantries", FiscalYearId = 1, order = 14},
                    new SnapPolicyPartner{ Name = "Foster Family Program", FiscalYearId = 1, order = 15},
                    new SnapPolicyPartner{ Name = "Grocery Stores", FiscalYearId = 1, order = 16},
                    new SnapPolicyPartner{ Name = "Hospitals/clinics", FiscalYearId = 1, order = 17},
                    new SnapPolicyPartner{ Name = "Housing Authority", FiscalYearId = 1, order = 18},
                    new SnapPolicyPartner{ Name = "Local Business", FiscalYearId = 1, order = 19},
                    new SnapPolicyPartner{ Name = "Mental Health/Rehab", FiscalYearId = 1, order = 21},
                    new SnapPolicyPartner{ Name = "Military", FiscalYearId = 1, order = 22},
                    new SnapPolicyPartner{ Name = "Parks & Recreation", FiscalYearId = 1, order = 23},
                    new SnapPolicyPartner{ Name = "Public Health/Health Department", FiscalYearId = 1, order = 24},
                    new SnapPolicyPartner{ Name = "Public Library", FiscalYearId = 1, order = 25},
                    new SnapPolicyPartner{ Name = "State & Federal Government", FiscalYearId = 1, order = 26},
                    new SnapPolicyPartner{ Name = "Utility Company", FiscalYearId = 1, order = 27},
                    new SnapPolicyPartner{ Name = "YMCA", FiscalYearId = 1, order = 28}

            );
            db.SaveChanges();
        }







        private static void CountyData(KERScoreContext db){


            /******************************* */
            // Population
            /******************************* */
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "http://api.census.gov/data/2016/pep/population?get=POP,GEONAME&for=county:*&in=state:21&DATE=9";
            var result = client.GetAsync(uri).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            var ob = JsonConvert.DeserializeObject< List<List<string> >>(data);

            var units = db.PlanningUnit.Where(u => u.District != null).ToList();

            var i = 0;
            foreach( var row in ob){
                if(i > 0){
                    var populCounty = row[1].Substring(0, row[1].Length - 17);
                    var unit = units.Where( u => u.Name.Substring(0, u.Name.Length -11) == populCounty).FirstOrDefault();
                    if(unit != null){
                        unit.Population = Convert.ToInt32(row[0]);
                    }
                }
                i++;
            }
            db.SaveChanges();
        }



        private static void ActivityDate(KERScoreContext db){
            var activities = db.Activity.
                                Where(a => a.Id > 0).
                                Include(a => a.Revisions).
                                Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile);
            foreach( var activity in activities){
                if(activity.ActivityDate == new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)){
                    var lastRevision = activity.Revisions.OrderBy( r => r.Created).Last();
                    activity.ActivityDate = lastRevision.ActivityDate;
                }
                if(activity.PlanningUnitId == null){
                    activity.PlanningUnitId = activity.KersUser.RprtngProfile.PlanningUnitId;
                }
            }        
            db.SaveChanges();
        }

        private static void ExpenseDate(KERScoreContext db){
            
            /*
            var expenses = db.Expense.
                                Where(a => a.Id > 0).
                                Include(a => a.Revisions).
                                Include( a => a.KersUser).ThenInclude( u => u.RprtngProfile);
            foreach( var expense in expenses){
                if(expense.Revisions.Count > 0){
                    if(expense.ExpenseDate == null){
                        var lastRevision = expense.Revisions.OrderBy( r => r.Created).Last();
                        expense.ExpenseDate = expense.Revisions.First().ExpenseDate;
                    }
                    if(expense.PlanningUnitId == null){
                        expense.PlanningUnitId = expense.KersUser.RprtngProfile.PlanningUnitId;
                    }
                }
            }        
            db.SaveChanges();
             */
        }


        


        private static void StoryOutcome(KERScoreContext db){
            if (db.StoryOutcome.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new StoryOutcome{Name = "Initial Outcome", Order = 1},
                    new StoryOutcome{Name = "Intermediate Outcome", Order = 2},
                    new StoryOutcome{Name = "Long-Term Outcome", Order = 3}
                    );          
            db.SaveChanges();
        }

        private static void ActivityOptionNumber(KERScoreContext db){
            if (db.ActivityOptionNumber.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new ActivityOptionNumber{Name = "Number of Adult Volunteers", Order = 1},
                    new ActivityOptionNumber{Name = "Number of Youth Participants (18 and under)", Order = 2},
                    new ActivityOptionNumber{Name = "Number of Indirect Contacts", Order = 3}
                    );          
            db.SaveChanges();
        }

        private static void ActivityOption(KERScoreContext db){
            if (db.ActivityOption.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new ActivityOption{Name = "Not present?", Order = 1},
                    new ActivityOption{Name = "Occurred at night?", Order = 2},
                    new ActivityOption{Name = "Occurred on a weekend?", Order = 3},
                    new ActivityOption{Name = "Not Extension sponsored?", Order = 4},
                    new ActivityOption{Name = "Multistate effort?", Order = 5},
                    new ActivityOption{Name = "MS4 eligible?", Order = 6}
                    );          
            db.SaveChanges();
        }

        private static void Race(KERScoreContext db){
            if (db.Race.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new Race{Name = "White", Order = 1},
                    new Race{Name = "Black", Order = 2},
                    new Race{Name = "Asian", Order = 3},
                    new Race{Name = "American Indian or Alaska Native", Order = 4},
                    new Race{Name = "Hawaiian or Pacific Islander", Order = 5},
                    new Race{Name = "Could not be Determined", Order = 6},
                    new Race{Name = "Other", Order = 7}
                    );          
            db.SaveChanges();
        }

        private static void Ethnicity(KERScoreContext db){
            if (db.Ethnicity.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new Ethnicity{Name = "Non-Hispanic", Order = 1},
                    new Ethnicity{Name = "Hispanic", Order = 2}
                    );          
            db.SaveChanges();
        }

        private static void ExpenseFundingSource(KERScoreContext db){
            if (db.ExpenseFundingSource.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new ExpenseFundingSource{Name = "County (Personal –Reimbursed to Employee)", Order = 1},
                    new ExpenseFundingSource{Name = "County (Credit Card/Check/Prepaid Expense)", Order = 2},
                    new ExpenseFundingSource{Name = "Professional Improvement (Personal – Reimbursed to Employee)", Order = 3},
                    new ExpenseFundingSource{Name = "Professional Improvement (Credit Card/Check/Prepaid Expense)", Order = 4},
                    new ExpenseFundingSource{Name = "State", Order = 5},
                    new ExpenseFundingSource{Name = "Federal", Order = 6}
                    );          
            db.SaveChanges();
        }




        private static void ExpenseMileageRate(KERScoreContext db){
            if (db.ExpenseMileageRate.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2015"),
                            End = DateTime.Parse("12/31/2015"),
                            Description = "", 
                            Rate = 0.575F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2016"),
                            End = DateTime.Parse("12/31/2016"),
                            Description = "", 
                            Rate = 0.54F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2011"),
                            End = DateTime.Parse("12/31/2012"),
                            Description = "", 
                            Rate = 0.54F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2013"),
                            End = DateTime.Parse("12/31/2013"),
                            Description = "", 
                            Rate = 0.565F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2014"),
                            End = DateTime.Parse("12/31/2014"),
                            Description = "", 
                            Rate = 0.56F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 1, 
                            Start = DateTime.Parse("01/01/2017"),
                            End = DateTime.Parse("12/31/9999"),
                            Description = "", 
                            Rate = 0.56F
                        },
                    new ExpenseMileageRate{
                            InstitutionId  = 2, 
                            Start = DateTime.Parse("01/01/2006"),
                            End = DateTime.Parse("12/31/9999"),
                            Description = "", 
                            Rate = 0.45F
                        }
                    );          
            db.SaveChanges();
        }


        private static void ExpenseMealRate(KERScoreContext db){
            if (db.ExpenseMealRate.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new ExpenseMealRate{    InstitutionId  = 1, 
                                            PlanningUnitId = 70,
                                            Description="Standard Rate", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 7F,
                                            LunchRate = 11F,
                                            DinnerRate = 23F,
                                            Order = 1},
                    new ExpenseMealRate{    InstitutionId  = 1,
                                            PlanningUnitId = 173,
                                            Description="Boone", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 8F,
                                            LunchRate = 12F,
                                            DinnerRate = 26F,
                                            Order = 2},
                    new ExpenseMealRate{    InstitutionId  = 1,
                                            PlanningUnitId = 11,
                                            Description="Kenton", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 9F,
                                            LunchRate = 13F,
                                            DinnerRate = 29F,
                                            Order = 3},
                    new ExpenseMealRate{    InstitutionId  = 1,
                                            PlanningUnitId = 73,
                                            Description="Fayette", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 15F,
                                            DinnerRate = 31F,
                                            Order = 4},
                    new ExpenseMealRate{    InstitutionId  = 1,
                                            PlanningUnitId = 25,
                                            Description="Jefferson", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 15F,
                                            DinnerRate = 31F,
                                            Order = 5},
                    new ExpenseMealRate{    InstitutionId  = 2, 
                                            PlanningUnitId = 70,
                                            Description="Standard Rate", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 9F,
                                            LunchRate = 10F,
                                            DinnerRate = 20F,
                                            Order = 6},
                    new ExpenseMealRate{    InstitutionId  = 2,
                                            PlanningUnitId = 173,
                                            Description="Boone", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 12F,
                                            DinnerRate = 22F,
                                            Order = 7},
                    new ExpenseMealRate{    InstitutionId  = 2,
                                            PlanningUnitId = 11,
                                            Description="Kenton", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 12F,
                                            DinnerRate = 22F,
                                            Order = 8},
                    new ExpenseMealRate{    InstitutionId  = 2,
                                            PlanningUnitId = 73,
                                            Description="Fayette", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 14F,
                                            DinnerRate = 25F,
                                            Order = 9},
                    new ExpenseMealRate{    InstitutionId  = 2,
                                            PlanningUnitId = 25,
                                            Description="Jefferson", 
                                            Start = DateTime.Parse("01/01/2011"), 
                                            End = DateTime.Parse("01/01/9999"), 
                                            BreakfastRate = 10F,
                                            LunchRate = 14F,
                                            DinnerRate = 25F,
                                            Order = 10}
                    
                    );          
            db.SaveChanges();
        }




        private static void SocialConnectionType(KERScoreContext db){
            if (db.SocialConnectionType.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new SocialConnectionType { Description="Facebook", Name = "Facebook", Icon = "fa-facebook", Url="https://www.facebook.com/" },
                    new SocialConnectionType { Description="Twitter", Name = "Twitter", Icon = "fa-twitter", Url="https://twitter.com/" },
                    new SocialConnectionType { Description="LinkedIn", Name = "LinkedIn", Icon = "fa-linkedin", Url="https://www.linkedin.com/in/" },
                    new SocialConnectionType { Description="Instagram", Name = "Instagram", Icon = "fa-instagram", Url="" },
                    new SocialConnectionType { Description="Google", Name = "Google", Icon = "fa-google", Url="https://plus.google.com/u/0/" },
                    new SocialConnectionType { Description="Yahoo", Name = "Yahoo", Icon = "fa-yahoo", Url="" },
                    new SocialConnectionType { Description="Youtube", Name = "Youtube", Icon = "fa-youtube", Url="https://www.youtube.com/user/" },
                    new SocialConnectionType { Description="Vimeo", Name = "Vimeo", Icon = "fa-vimeo-square", Url="" },
                    new SocialConnectionType { Description="Flickr", Name = "Flickr", Icon = "fa-flickr", Url="" },
                    new SocialConnectionType { Description="Pinterest", Name = "Pinterest", Icon = "fa-pinterest", Url="" },
                    new SocialConnectionType { Description="Skype", Name = "Skype", Icon = "fa-skype", Url="" }

                    );          
            db.SaveChanges();
        }

        private static void GeneralLocation(KERScoreContext db){
            if (db.GeneralLocation.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new GeneralLocation { Code="LEXCAMPUS", Name = "Lexington Campus", Order = 1 },
                    new GeneralLocation { Code="OFFCAMPUSREC", Name = "Off-Campus - Research and Education Center at Princeton", Order = 1 },
                    new GeneralLocation { Code="OFFCAMPUSRCARS", Name = "Off-Campus - Robinson Center for Appalachian Resource Sustainability (RCARS)", Order = 2 },
                    new GeneralLocation { Code="OFFCAMPUSCESOFFICE", Name = "Off-Campus - County Extension Office", Order = 3 },
                    new GeneralLocation { Code="OFFCAMPUSOTHER", Name = "Off-Campus - other", Order = 4 }
                    );          
            db.SaveChanges();
        }

        private static void Institution(KERScoreContext db){
            if (db.Institution.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new Institution { Code="21000-1862", Name = "University of Kentucky", Order = 1 },
                    new Institution { Code="21000-1890", Name = "Kentucky State University", Order = 2 }
                    );          
            db.SaveChanges();
        }

        private static void Specialty(KERScoreContext db){
            if (db.Specialty.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new Specialty { Code="progANR", Name = "Agriculture and Natural Resources", Description = "Agriculture and Natural Resources" },
                    new Specialty { Code="prog4HYD", Name = "4-H Youth Development", Description = "4-H Youth Development" },
                    new Specialty { Code="progFCS", Name = "Family and Consumer Science", Description = "Family and Consumer Science" },
                    new Specialty { Code="progHORT", Name = "Horticulture", Description = "Horticulture" },
                    new Specialty { Code="progNEP", Name = "Expanded Food and Nutrition Education Program", Description = "Expanded Food and Nutrition Education Program" },
                    new Specialty { Code="snapEd", Name = "Supplemental Nutrition Assistance Program Education", Description = "Supplemental Nutrition Assistance Program Education" },
                    new Specialty { Code="progOther", Name = "Other", Description = "Other" }
                    );          
            db.SaveChanges();
        }

        private static void AddAffirmativeSummaryDiversityType(KERScoreContext db){
            if (db.AffirmativeSummaryDiversityType.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new AffirmativeSummaryDiversityType { Name = "Number of Meetings", Order = 1 },
                    new AffirmativeSummaryDiversityType { Name = "Average Number Attending Each Meeting", Order = 2 },
                    new AffirmativeSummaryDiversityType { Name = "Females *", Order = 3 },
                    new AffirmativeSummaryDiversityType { Name = "Males *", Order = 4 },
                    new AffirmativeSummaryDiversityType { Name = "Racial Minorities Present *", Order = 5 }
                    );          
            db.SaveChanges();
        }
        private static void AddAffirmativeMakeupDiversityTypeGroup(KERScoreContext db){
            if (db.AffirmativeMakeupDiversityTypeGroup.Any())
            {
                return;   // DB has been seeded
            }
            var raceGroupMembers = new List<AffirmativeMakeupDiversityType>();
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "White", Order = 1, Type="number" }).Entity);
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Black", Order = 2, Type="number" }).Entity);
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Asian/Pacific Islander", Order = 3, Type="number" }).Entity);
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "American Indian or Alaska Native", Order = 4 , Type="number"}).Entity);
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Other", Order = 5, Type="number" }).Entity);
            raceGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Race Not Determined", Order = 6, Type="number" }).Entity);

            var ethnicityGroupMembers = new List<AffirmativeMakeupDiversityType>();
            ethnicityGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Hispanic", Order = 1, Type="number" }).Entity);

            var genderGroupMembers = new List<AffirmativeMakeupDiversityType>();
            genderGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Females", Order = 1, Type="number" }).Entity);
            genderGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Males", Order = 2, Type="number" }).Entity);

            var noneGroupMembers = new List<AffirmativeMakeupDiversityType>();
            noneGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Number of Meetings This Year", Order = 1, Type="number" }).Entity);
            noneGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Average Attendance", Order = 2, Type="number" }).Entity);
            noneGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Date Bylaws Updated", Order = 3, Type="date" }).Entity);
            noneGroupMembers.Add(db.AffirmativeMakeupDiversityType.Add( new AffirmativeMakeupDiversityType{ Name = "Council Rotation Policy in Place", Order = 4 , Type="boolean"}).Entity);
            

            db.AddRange(
                new AffirmativeMakeupDiversityTypeGroup{ 
                                Name = "MEMBERSHIP BY RACE", 
                                Order = 1, 
                                Render = true, 
                                Types = raceGroupMembers
                                
                                },
                new AffirmativeMakeupDiversityTypeGroup{ 
                                Name = "ETHNICITY", 
                                Order = 2, 
                                Render = true, 
                                Types = ethnicityGroupMembers
                                
                                },
                new AffirmativeMakeupDiversityTypeGroup{ 
                                Name = "GENDER", 
                                Order = 3, 
                                Render = true, 
                                Types = genderGroupMembers
                                
                                },
                new AffirmativeMakeupDiversityTypeGroup{ 
                                Name = "NONE", 
                                Order = 4, 
                                Render = false, 
                                Types = noneGroupMembers
                                
                                }
            );
            db.SaveChanges();
        }
        private static void AddAffirmativeGroupType(KERScoreContext db)
        {
            if (db.AffirmativeAdvisoryGroupType.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new AffirmativeAdvisoryGroupType { Name = "County Extension Council", Order = 1 },
                    new AffirmativeAdvisoryGroupType { Name = "District Board", Order = 2 },
                    new AffirmativeAdvisoryGroupType { Name = "Ag Advancement Council Board", Order = 3 },
                    new AffirmativeAdvisoryGroupType { Name = "FCS Council", Order = 4 },
                    new AffirmativeAdvisoryGroupType { Name = "4-H Council", Order = 5 },
                    new AffirmativeAdvisoryGroupType { Name = "Homemakers Council", Order = 6 },
                    new AffirmativeAdvisoryGroupType { Name = "Horticultural Council", Order = 7 },
                    new AffirmativeAdvisoryGroupType { Name = "CED Council", Order = 8 },
                    new AffirmativeAdvisoryGroupType { Name = "Fine Arts Council", Order = 9 },
                    new AffirmativeAdvisoryGroupType { Name = "Expansion and Review Committee", Order = 10 },
                    new AffirmativeAdvisoryGroupType { Name = "Client Protection Committee", Order = 11 },
                    new AffirmativeAdvisoryGroupType { Name = "Other", Order = 12 }
                    );          
            db.SaveChanges();
        }



            
            
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Data;
using Kers.Models.Contexts;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Kers.Models.Repositories
{
    public class SnapCommitmentRepository : SnapBaseRepository, ISnapCommitmentRepository
    {

        private KERScoreContext context;
        private IDistributedCache _cache;
        public SnapCommitmentRepository(KERScoreContext context, IDistributedCache _cache)
            : base(context, _cache)
        { 
            this.context = context;
            this._cache = _cache;
        }


        public async Task<string> CommitmentSummary(FiscalYear fiscalYear, bool refreshCache = false){


            string result;
            var cacheKey = CacheKeys.SnapCommitmentSummary + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("District");
                keys.Add("PlanningUnit");
                keys.Add("Name");
                keys.Add("Title");
                keys.Add("Program(s)");
                keys.Add("HoursReportedLastFY");
                keys.Add("HoursCommittedLastFY");
                keys.Add("HoursCommittedThisFY");

                result = string.Join(",", keys.ToArray()) + "\n";

                var previousFiscalYear = await this.context.FiscalYear
                                                .Where( f => f.Start < fiscalYear.Start && f.Type == FiscalYearType.SnapEd)
                                                .OrderByDescending( f => f.Start)
                                                .FirstOrDefaultAsync();
                if(previousFiscalYear == null){
                    throw new Exception("No Previous Fiscal Year Found in Commitment Summary Report.");
                }
                var byUser = SnapData(previousFiscalYear)
                                .GroupBy( d=> d.User.Id)
                                .Select(
                                    k => new {
                                        Revisions = k.Select( a => a.Revision),
                                        User = k.Select( a => a.User).First()
                                    }
                                );
                var Rows = new List<CommitmentSummaryViewModel>();
                var userIds = new List<int>();
                foreach( var usr in byUser){
                    userIds.Add(usr.User.Id);
                    var rowData = new CommitmentSummaryViewModel();
                    rowData.FY = fiscalYear.Name;
                    
                    if(usr.User.RprtngProfile.PlanningUnit.District != null){
                        rowData.District = usr.User.RprtngProfile.PlanningUnit.District.Name;
                    }else{
                        rowData.District = "";
                    }
                    rowData.PlanningUnit = usr.User.RprtngProfile.PlanningUnit.Name;
                    rowData.Name = usr.User.RprtngProfile.Name;
                    rowData.Title = usr.User.ExtensionPosition.Code;
                    var spclt = "";
                    foreach( var s in usr.User.Specialties){
                        spclt += " " + (s.Specialty.Code.Substring( 0, 4) == "prog" ? s.Specialty.Code.Substring(4) : s.Specialty.Code);
                    }
                    rowData.Programs = spclt;
                    var reported = usr.Revisions.Sum( s => s.Hours ).ToString();
                    rowData.HoursReportedLastFY = (reported == "" ? "0" :reported);

                    var prev = context.SnapEd_Commitment
                                .Where( c => c.KersUserId1 == usr.User.Id
                                            && 
                                            c.FiscalYear == previousFiscalYear
                                            &&
                                            c.SnapEd_ActivityType.Measurement == "Hour");

                    var previous = prev.Sum( s => s.Amount).ToString();
                    rowData.HoursCommittedLastFY = (previous == ""? "0" : previous );
                    var curnt = context.SnapEd_Commitment
                                .Where( c => c.KersUserId1 == usr.User.Id
                                            && 
                                            c.FiscalYear == fiscalYear
                                            &&
                                            c.SnapEd_ActivityType.Measurement == "Hour");

                    var current = curnt.Sum( s => s.Amount).ToString();
                    rowData.HoursCommittedThisFY = (current == "" ? "0" : current);
                    Rows.Add(rowData);
                }

                // Find people with commitment but no reported hours
                var rest = context.SnapEd_Commitment.Where(
                                                        c =>                    
                                                            !userIds.Contains(c.KersUserId1??0)
                                                            && 
                                                            c.FiscalYear == fiscalYear
                                                            &&
                                                            c.SnapEd_ActivityType.Measurement == "Hour"
                                                            &&
                                                            c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                ).GroupBy( d=> d.KersUser.Id)
                                .Select(
                                    k => new {
                                        Commitments = k.Select( a => a),
                                        User = k.Select( a => a.KersUser).First()
                                    }
                                );
                foreach( var usr in rest){
                    var userData = await context.KersUser.Where( u => u.Id == usr.User.Id )
                                .Include( u => u.RprtngProfile).ThenInclude( r => r.PlanningUnit ).ThenInclude( u => u.District)
                                .Include( u => u.Specialties ).ThenInclude( s => s.Specialty)
                                .Include( u => u.ExtensionPosition)
                                .FirstOrDefaultAsync();
                    var rowData = new CommitmentSummaryViewModel();
                    rowData.FY = fiscalYear.Name;
                    if(userData.RprtngProfile.PlanningUnit.District != null){
                        rowData.District = userData.RprtngProfile.PlanningUnit.District.Name;
                    }else{
                        rowData.District = "";
                    }
                    rowData.PlanningUnit = userData.RprtngProfile.PlanningUnit.Name;
                    rowData.Name = userData.RprtngProfile.Name;
                    rowData.Title = userData.ExtensionPosition.Code;
                    var spclt = "";
                    foreach( var s in userData.Specialties){
                        spclt += " " + (s.Specialty.Code.Substring( 0, 4) == "prog" ? s.Specialty.Code.Substring(4) : s.Specialty.Code);
                    }
                    rowData.Programs = spclt;
                    rowData.HoursReportedLastFY = "0";
                    var prev = context.SnapEd_Commitment
                                .Where( c => c.KersUserId1 == usr.User.Id
                                            && 
                                            c.FiscalYear == previousFiscalYear
                                            &&
                                            c.SnapEd_ActivityType.Measurement == "Hour");

                    var previous = prev.Sum( s => s.Amount).ToString();
                    rowData.HoursCommittedLastFY = (previous == ""? "0" : previous );
                    rowData.HoursCommittedThisFY = usr.Commitments.Sum( c => c.Amount).ToString();
                    Rows.Add(rowData);
                }
                


                foreach( var Row in Rows.OrderBy( r => r.District).ThenBy( r => r.PlanningUnit).ThenBy( r => r.Name )){
                    var textRow = "";
                    textRow += Row.FY + ",";
                    textRow += Row.District + ",";
                    textRow += Row.PlanningUnit + ",";
                    textRow += string.Concat( "\"", Row.Name, "\"") + ",";
                    textRow += string.Concat( "\"", Row.Title, "\"") + ",";
                    textRow += string.Concat( "\"", Row.Programs, "\"") + ",";
                    textRow += Row.HoursReportedLastFY + ",";
                    textRow += Row.HoursCommittedLastFY + ",";
                    textRow += Row.HoursCommittedThisFY;
                    result += textRow + "\n";
                }

                
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }
        public async Task<string> CommitmentHoursDetail(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentHoursDetail + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("District");
                keys.Add("PlanningUnit");
                keys.Add("Name");
                keys.Add("Title");
                keys.Add("Program(s)");
                
                var projects = await context.SnapEd_ProjectType.Where(t => t.FiscalYear == fiscalYear).ToListAsync();
                var activitiesPerProject = await context.SnapEd_ActivityType.Where( p => p.PerProject == 1 && p.FiscalYear == fiscalYear).ToListAsync();

                foreach( var project in projects){
                    foreach( var type in activitiesPerProject){
                        keys.Add( string.Concat( "\"", project.Name + type.Name, "\"") );
                    }
                }
                var activitiesNotPerProject = context.SnapEd_ActivityType.Where( p => p.PerProject != 1 && p.FiscalYear == fiscalYear);
                foreach( var type in activitiesNotPerProject){
                    keys.Add( string.Concat( "\"", type.Name, "\""));
                }
                keys.Add( "TotalCommitmentHours" );
                result = string.Join(",", keys.ToArray()) + "\n";

                var commitment = await context.SnapEd_Commitment
                                    .Where( c => 
                                                    c.FiscalYear == fiscalYear
                                                    &&
                                                    c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                    )
                                    .GroupBy( c => c.KersUser)
                                    .Select( c => new {
                                        User = c.Key,
                                        commitments = c.Select( s => s )
                                    })
                                    .ToListAsync();
                foreach( var usr in commitment){
                    var user = await context.KersUser.Where( u => u.Id == usr.User.Id)
                                .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit ).ThenInclude( u => u.District)
                                .Include( u => u.ExtensionPosition)
                                .Include( u => u.Specialties).ThenInclude( s => s.Specialty)
                                .FirstOrDefaultAsync();
                                
                    var row = fiscalYear.Name + ",";
                    if(user.RprtngProfile.PlanningUnit.District != null){
                        row += user.RprtngProfile.PlanningUnit.District.Name + ",";
                    }else{
                        row +=  ",";
                    }
                    
                    row += user.RprtngProfile.PlanningUnit.Name + ",";
                    row += string.Concat( "\"", user.RprtngProfile.Name, "\"") + ",";
                    row += user.ExtensionPosition.Code + ",";
                    var spclt = "";
                    if(user.Specialties != null){
                        foreach( var s in user.Specialties){
                            spclt += " " + (s.Specialty.Code.Substring( 0, 4) == "prog" ? s.Specialty.Code.Substring(4) : s.Specialty.Code);
                        }
                    }
                    row += string.Concat( "\"", spclt, "\"") + ",";
                    var sumHours = 0;
                    foreach( var project in projects){
                        foreach( var type in activitiesPerProject){
                            var cmtm = usr.commitments
                                                .Where( c => c.SnapEd_ProjectTypeId == project.Id && c.SnapEd_ActivityTypeId == type.Id )
                                                .FirstOrDefault();
                            if( cmtm != null){
                                if( type.Measurement == "Hour"){
                                    sumHours += cmtm.Amount??0;
                                }
                                row += cmtm.Amount.ToString() + ",";
                            }else{
                                row +=  "0,";
                            }
                            
                        }
                    }
                    foreach( var type in activitiesNotPerProject){
                        var cmtm = usr.commitments
                                                .Where( c => c.SnapEd_ActivityTypeId == type.Id )
                                                .FirstOrDefault();
                        if( cmtm != null){
                            if( type.Measurement == "Hour"){
                                sumHours += cmtm.Amount??0;
                            }
                            row += cmtm.Amount.ToString() + ",";
                        }else{
                            row += "0,";
                        }
                        
                    }
                    row += sumHours.ToString() + ",";
                    result += row + "\n";
                }
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }

        public async Task<string> AgentsWithoutCommitment(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapAgentsWithoutCommitment + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();

                keys.Add("District");
                keys.Add("PlanningUnit");
                keys.Add("Name");
                keys.Add("Title");
                keys.Add("Program(s)");
                result = string.Join(",", keys.ToArray()) + "\n";


                var agents = await context.KersUser
                                                        .Where( u => 
                                                                u.RprtngProfile.enabled
                                                                &&
                                                                u.ExtensionPosition.Code == "AGENT"
                                                                &&
                                                                u.RprtngProfile.Institution.Code == "21000-1862"
                                                                &&
                                                                context.SnapEd_Commitment.Where( c => c.FiscalYear == fiscalYear && c.KersUser == u).Count() == 0
                                                                )
                                                        .Include( u => u.RprtngProfile ).ThenInclude( r => r.PlanningUnit ).ThenInclude( u => u.District)
                                                        .Include( u => u.ExtensionPosition)
                                                        .Include( u => u.Specialties).ThenInclude( s => s.Specialty)
                                                        .ToListAsync();
                foreach( var user in agents){
                    
                                
                    var row = "";

                    if(user.RprtngProfile.PlanningUnit.District != null){
                        row += user.RprtngProfile.PlanningUnit.District.Name + ",";
                    }else{
                        row +=  ",";
                    }
                    
                    row += user.RprtngProfile.PlanningUnit.Name + ",";
                    row += string.Concat( "\"", user.RprtngProfile.Name, "\"") + ",";
                    row += user.ExtensionPosition.Code + ",";
                    var spclt = "";
                    if(user.Specialties != null){
                        foreach( var s in user.Specialties){
                            spclt += " " + (s.Specialty.Code.Substring( 0, 4) == "prog" ? s.Specialty.Code.Substring(4) : s.Specialty.Code);
                        }
                    }
                    row += string.Concat( "\"", spclt, "\"") + ",";


                    result += row + "\n";
                } 
                
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }




        public async Task<string> SummaryByPlanningUnit(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentSummaryByPlanningUnit + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("PlanningUnit");
                
                var projects = await context.SnapEd_ProjectType.Where(t => t.FiscalYear == fiscalYear).ToListAsync();
                var activitiesPerProject = await context.SnapEd_ActivityType.Where( p => p.PerProject == 1 && p.FiscalYear == fiscalYear).ToListAsync();

                foreach( var project in projects){
                    foreach( var type in activitiesPerProject){
                        keys.Add( string.Concat( "\"", project.Name + type.Name, "\"") );
                    }
                }
                var activitiesNotPerProject = context.SnapEd_ActivityType.Where( p => p.PerProject != 1 && p.FiscalYear == fiscalYear);
                foreach( var type in activitiesNotPerProject){
                    keys.Add( string.Concat( "\"", type.Name, "\""));
                }
                keys.Add( "TotalCommitmentHours" );
                result = string.Join(",", keys.ToArray()) + "\n";

                var commitment = await context.SnapEd_Commitment
                                    .Where( c => 
                                                c.FiscalYear == fiscalYear
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                            )
                                    .GroupBy( c => c.KersUser.RprtngProfile.PlanningUnit)
                                    .Select( c => new {
                                        Unit = c.Key,
                                        commitments = c.Select( s => s )
                                    })
                                    .OrderBy(s => s.Unit.Name)
                                    .ToListAsync();
                foreach( var unit in commitment){
                    
                    var row = fiscalYear.Name + ",";
                    row += unit.Unit.Name + ",";
                    

                    var sumHours = 0;
                    foreach( var project in projects){
                        foreach( var type in activitiesPerProject){
                            var cmtm = unit.commitments
                                                .Where( c => c.SnapEd_ProjectTypeId == project.Id && c.SnapEd_ActivityTypeId == type.Id );
                            if( cmtm != null){
                                if( type.Measurement == "Hour"){
                                    sumHours += cmtm.Sum( s => s.Amount)??0;
                                }
                                row += cmtm.Sum( s => s.Amount).ToString() + ",";
                            }else{
                                row +=  "0,";
                            }
                            
                        }
                    }
                    foreach( var type in activitiesNotPerProject){
                        var cmtm = unit.commitments
                                                .Where( c => c.SnapEd_ActivityTypeId == type.Id );
                        if( cmtm != null){
                            if( type.Measurement == "Hour"){
                                sumHours += cmtm.Sum( s => s.Amount)??0;
                            }
                            row += cmtm.Sum( s => s.Amount).ToString() + ",";
                        }else{
                            row += "0,";
                        }
                        
                    }
                    row += sumHours.ToString() + ",";
                    result += row + "\n";
                }
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }

        public async Task<string> SummaryByPlanningUnitNotNEPAssistants(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentSummaryByPlanningUnit + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("FY");
                keys.Add("PlanningUnit");
                
                var projects = await context.SnapEd_ProjectType.Where(t => t.FiscalYear == fiscalYear).ToListAsync();
                var activitiesPerProject = await context.SnapEd_ActivityType.Where( p => p.PerProject == 1 && p.FiscalYear == fiscalYear).ToListAsync();

                foreach( var project in projects){
                    foreach( var type in activitiesPerProject){
                        keys.Add( string.Concat( "\"", project.Name + type.Name, "\"") );
                    }
                }
                var activitiesNotPerProject = context.SnapEd_ActivityType.Where( p => p.PerProject != 1 && p.FiscalYear == fiscalYear);
                foreach( var type in activitiesNotPerProject){
                    keys.Add( string.Concat( "\"", type.Name, "\""));
                }
                keys.Add( "TotalCommitmentHours" );
                result = string.Join(",", keys.ToArray()) + "\n";

                var commitment = await context.SnapEd_Commitment
                                    .Where( c => 
                                                c.FiscalYear == fiscalYear
                                                &&
                                                c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                &&
                                                c.KersUser.Specialties.Where( s => 
                                                                                s.Specialty.Name == "Expanded Food and Nutrition Education Program"
                                                                                ||
                                                                                s.Specialty.Name == "Supplemental Nutrition Assistance Program Education"
                                                                            ).Count() == 0
                                            )
                                    .GroupBy( c => c.KersUser.RprtngProfile.PlanningUnit )
                                    .Select( c => new {
                                        Unit = c.Key,
                                        commitments = c.Select( s => s )
                                    })
                                    .OrderBy(s => s.Unit.Name)
                                    .ToListAsync();
                foreach( var unit in commitment){
                    
                    var row = fiscalYear.Name + ",";
                    row += unit.Unit.Name + ",";
                    

                    var sumHours = 0;
                    foreach( var project in projects){
                        foreach( var type in activitiesPerProject){
                            var cmtm = unit.commitments
                                                .Where( c => c.SnapEd_ProjectTypeId == project.Id && c.SnapEd_ActivityTypeId == type.Id );
                            if( cmtm != null){
                                if( type.Measurement == "Hour"){
                                    sumHours += cmtm.Sum( s => s.Amount)??0;
                                }
                                row += cmtm.Sum( s => s.Amount).ToString() + ",";
                            }else{
                                row +=  "0,";
                            }
                            
                        }
                    }
                    foreach( var type in activitiesNotPerProject){
                        var cmtm = unit.commitments
                                                .Where( c => c.SnapEd_ActivityTypeId == type.Id );
                        if( cmtm != null){
                            if( type.Measurement == "Hour"){
                                sumHours += cmtm.Sum( s => s.Amount)??0;
                            }
                            row += cmtm.Sum( s => s.Amount).ToString() + ",";
                        }else{
                            row += "0,";
                        }
                        
                    }
                    row += sumHours.ToString() + ",";
                    result += row + "\n";
                }
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }

        public async Task<string> ReinforcementItems(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentReinforcementItems + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("ItemCount");
                keys.Add("ItemName");
                result = string.Join(",", keys.ToArray()) + "\n";
                var items = context.SnapEd_ReinforcementItemChoice
                                .Where( i => 
                                            i.FiscalYear == fiscalYear
                                            &&
                                            i.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                        )
                                .GroupBy( i => i.SnapEd_ReinforcementItem )
                                .Select( i => new {
                                    item = i.Key,
                                    amount = i.Select( s => s).Count()
                                })
                                .OrderBy( i => i.item.Name);
                foreach( var item in items){
                    var row = item.item.Name + ",";
                    row += item.amount.ToString();
                    result += row + "\n";
                }
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }


        public async Task<string> ReinforcementItemsPerCounty(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentReinforcementItemsPerCounty + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();
                keys.Add("District");
                keys.Add("PlanningUnit");

                var ReinforcementItems = context.SnapEd_ReinforcementItem.Where( a => a.FiscalYear == fiscalYear);
                foreach( var item in ReinforcementItems){
                    keys.Add( string.Concat( "\"", item.Name, "\"") );
                }

                result = string.Join(",", keys.ToArray()) + "\n";

                var counties = context.PlanningUnit.Where( p => p.District != null && p.Name.Substring(p.Name.Count() - 3) == "CES").Include(p => p.District).OrderBy( p => p.Name );

                foreach( var county in counties){
                    var row = county.District.Name + ",";
                    row += county.Name + ",";
                    foreach( var item in ReinforcementItems){
                        var areItemsSelected = context.SnapEd_ReinforcementItemChoice
                                                        .Where( c => 
                                                                    c.KersUser.RprtngProfile.PlanningUnit == county 
                                                                    && 
                                                                    c.SnapEd_ReinforcementItem == item
                                                                    &&
                                                                    c.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                    )
                                                        .Any();
                        if( areItemsSelected ){
                            row += "x" + ",";
                        }else{
                            row += ",";
                        }
                    }
                    
                    result += row + "\n";
                }


                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }



        public async Task<string> SuggestedIncentiveItems(FiscalYear fiscalYear, bool refreshCache = false){
            string result;
            var cacheKey = CacheKeys.SnapCommitmentSuggestedIncentiveItems + fiscalYear.Name;
            var cacheString = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheString) && !refreshCache ){
                result = cacheString;
            }else{
                var keys = new List<string>();

                keys.Add("District");
                keys.Add("PlanningUnit");
                keys.Add("Name");
                keys.Add("Title");
                keys.Add("Program(s)");
                keys.Add("SuggestedIncentiveItems");
                result = string.Join(",", keys.ToArray()) + "\n";


                var suggestions = await context.SnapEd_ReinforcementItemSuggestion
                                                        .Where( u => 
                                                                u.FiscalYear == fiscalYear
                                                                &&
                                                                u.Suggestion != ""
                                                                &&
                                                                u.KersUser.RprtngProfile.Institution.Code == "21000-1862"
                                                                )
                                                        .Include( u => u.KersUser.RprtngProfile ).ThenInclude( r => r.PlanningUnit ).ThenInclude( u => u.District)
                                                        .Include( u => u.KersUser.ExtensionPosition)
                                                        .Include( u => u.KersUser.Specialties).ThenInclude( s => s.Specialty)
                                                        .ToListAsync();
                foreach( var suggestion in suggestions){
                    
                                
                    var row = "";

                    if(suggestion.KersUser.RprtngProfile.PlanningUnit.District != null){
                        row += suggestion.KersUser.RprtngProfile.PlanningUnit.District.Name + ",";
                    }else{
                        row +=  ",";
                    }
                    
                    row += suggestion.KersUser.RprtngProfile.PlanningUnit.Name + ",";
                    row += string.Concat( "\"", suggestion.KersUser.RprtngProfile.Name, "\"") + ",";
                    row += suggestion.KersUser.ExtensionPosition.Code + ",";
                    var spclt = "";
                    if(suggestion.KersUser.Specialties != null){
                        foreach( var s in suggestion.KersUser.Specialties){
                            spclt += " " + (s.Specialty.Code.Substring( 0, 4) == "prog" ? s.Specialty.Code.Substring(4) : s.Specialty.Code);
                        }
                    }
                    row += string.Concat( "\"", spclt, "\"") + ",";

                    row += string.Concat( "\"", suggestion.Suggestion, "\"") + ",";
                    result += row + "\n";
                } 
                
                await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays( 2 )
                    });
            }
            return result;
        }


    }




    public class CommitmentSummaryViewModel{

        public string FY;
        public string District;
        public string PlanningUnit;
        public string Name;
        public string Title;
        public string Programs;
        public string HoursReportedLastFY;
        public string HoursCommittedLastFY;
        public string HoursCommittedThisFY;


    }


}
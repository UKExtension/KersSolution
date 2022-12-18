using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities;
using Kers.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Kers.Models.Data;
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.ViewModels;
using Kers.Models.Entities.UKCAReporting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;



namespace Kers.Models.Abstract
{
    public interface IzEmpRptProfileRepository : IEntityBaseRepository<zEmpRptProfile> { }
    //public interface IzActivityRepository : IEntityBaseRepository<zActivity> { }
    //public interface IzzGeneralLocationRepository : IEntityBaseRepository<zzGeneralLocation> { }
    public interface  IzEmpRoleTypeRepository : IEntityBaseRepository<zEmpRoleType> { }
    public interface  ITrainingRepository {
        List<zInServiceTrainingCatalog>  csv2list(string fileUrl = "database/trainingsData.csv");
        List<Training> InServicesToTrainings(List<zInServiceTrainingCatalog> services);
        Training ServiceToTraining( zInServiceTrainingCatalog service);
        List<TrainingEnrollment> trainingsPerPersonPerYear( int userId, int year);
     }
    public interface  IInitiativeRepository : IEntityBaseRepository<StrategicInitiative> {
        Task<List<ProgramIndicatorSumViewModel>> IndicatorSumPerMajorProgram(int MajorProgramId );
    }
    public interface IKersUserRepository : IEntityBaseRepository<KersUser>{ 
        KersUser findByProfileID(int ProfileId);
        KersUser createUserFromProfile(zEmpRptProfile profile);

        List<zEmpRoleType> roles(int id);

        zEmpRoleType roleForId(int id);
        Task<List<KesrUserBriefViewModel>> Search( SearchCriteriaViewModel criteria, bool refreshCache = false );
    }

    public interface  INavSectionRepository : IEntityBaseRepository<NavSection> { 

        IQueryable<NavSection> AllIncludingQuery(params Expression<Func<NavSection, object>>[] includeProperties);
        NavGroup groupWithId(int id);
        NavGroup groupWithIdWithItems(int id);
        void deleteEntity(IEntityBase entity);
        NavItem itemWithId(int id);
    }

    public interface IPlanOfWorkRevisionRepository : IEntityBaseRepository<PlanOfWorkRevision> {}
    public interface ILogRepository: IEntityBaseRepository<Log>{}
    public interface IExpenseRepository: IEntityBaseRepository<Expense>{
        List<ExpenseSummary> Summaries(KersUser user, int year, int month);
        List<ExpenseRevision> PerMonth(KersUser user, int year, int month, string order);
        IQueryable<Expense> MileagePerMonth(KersUser user, int year, int month, bool withFundingSource = true, bool ForCurrentCounty = false);
        List<ExpenseSummary> SummariesPerFiscalYear(KersUser user, FiscalYear fiscalYear);
        List<ExpenseSummary> SummariesPerPeriod(KersUser user, DateTime start, DateTime end);
        float Breakfast(ExpenseRevision expense);
        float Lunch(ExpenseRevision expense);
        float Dinner(ExpenseRevision expense);
        float MileageRate(KersUser user, int year, int month);
    }
    public interface IActivityRepository: IEntityBaseRepository<Activity>{
        List<ActivityRevision> PerMonth(KersUser user, int year, int month, string order);
        List<PerUnitActivities> ProcessUnitActivities(List<ActivityUnitResult> activities, IDistributedCache _cache);
        List<PerProgramActivities> ProcessMajorProgramActivities(List<ActivityMajorProgramResult> activities, IDistributedCache _cache);
        //List<PerPersonActivities> ProcessPersonActivities(List<ActivityPersonResult> activities, IDistributedCache _cache);
        List<int> LastActivityRevisionIds( FiscalYear fiscalYear, IDistributedCache _cache);
        //TableViewModel ReportsStateAll(FiscalYear fiscalYear, bool refreshCache = false);
        Task<TableViewModel> ContactsByCountyByMajorProgram(FiscalYear fiscalYear, bool refreshCache = false);
        //Task<TableViewModel> StateByMajorProgram(FiscalYear fiscalYear, int type = 0, bool refreshCache = false);
        Task<List<ProgramDataViewModel>> TopProgramsPerMonth(int year = 0, int month = 0, int amount = 5, int PlanningUnitId = 0, bool refreshCache = false);
        Task<List<ProgramDataViewModel>> TopProgramsPerFiscalYear(FiscalYear FiscalYear, int amount = 5, int PlanningUnitId = 0, bool refreshCache = false);
        
        List<string> ReportHeaderRow(   
                                        List<Race> races = null,
                                        List<Ethnicity> ethnicities = null,
                                        List<ActivityOption> options = null, 
                                        List<ActivityOptionNumber> optionNumbers = null,
                                        List<SnapDirectAges> ages = null,
                                        List<SnapDirectAudience> audience = null,
                                        List<SnapIndirectMethod> method = null,
                                        List<SnapIndirectReached> reached = null,
                                        List<SnapPolicyAimed> aimed = null,
                                        List<SnapPolicyPartner> partners = null);
        
        List<string> ReportRow(     int id, 
                                    Activity activity = null, 
                                    List<Race> races = null,
                                    List<Ethnicity> ethnicities = null,
                                    List<ActivityOption> options = null, 
                                    List<ActivityOptionNumber> optionNumbers = null,
                                    List<SnapDirectAges> ages = null,
                                    List<SnapDirectAudience> audience = null,
                                    List<SnapIndirectMethod> method = null,
                                    List<SnapIndirectReached> reached = null,
                                    List<SnapPolicyAimed> aimed = null,
                                    List<SnapPolicyPartner> partners = null
                                );
    
    
    }

    public interface IContactRepository: IEntityBaseRepository<Contact>{
        //List<PerUnitActivities> ProcessUnitContacts(List<ContactUnitResult> contacts, List<PerUnitActivities> result, IDistributedCache _cache);
        //List<PerProgramActivities> ProcessMajorProgramContacts(List<ContactMajorProgramResult> contacts, List<PerProgramActivities> result, IDistributedCache _cache);
        //List<PerPersonActivities> ProcessPersonContacts(List<ContactPersonResult> contacts, List<PerPersonActivities> result, IDistributedCache _cache);
        //Task<TableViewModel> Data(FiscalYear fiscalYear, int type = 0, int id = 0, bool refreshCache = false );
        Task<TableViewModel> DataByEmployee(FiscalYear fiscalYear, int type = 0, int id = 0, bool refreshCache = false, int cacheDaysSpan = 0 );
        Task<TableViewModel> DataByMajorProgram(FiscalYear fiscalYear, int type = 0, int id = 0, bool refreshCache = false, int cacheDaysSpan = 0 );
        Task<StatsViewModel> StatsPerMonth( int year = 0, int month = 0, int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false);
        
        /*****************************************************************/
        // Generate Contacts Reports Groupped by Employee or Major Program
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        // grouppedBy: 0 Employee, 1 MajorProgram
        /*******************************************************************/
        Task<List<PerGroupActivities>> GetActivitiesAndContactsAsync( DateTime start, DateTime end, int filter = 0, int grouppedBy = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 );
        Task<List<PerGroupSummary>> GetActivitiesAndContactsSummaryAsync( DateTime start, DateTime end, int filter = 0, int grouppedBy = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 );
        /***********************************************************************************************/
        // Generate Contacts Reports Groupped by Employee or Major Program
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All
        // Returns List with Indexes: 0 Total Hours, 1 Contacts, 2 Multistate Hours, 3 Number of Activities
        /***********************************************************************************************/
        Task<List<float>> GetPerPeriodSummaries( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 );
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 5 Major Program, 6 Employee
        Task<List<int>> LastContactRevisionIds( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 );
        // filter: 0 District, 1 Planning Unit, 2 KSU, 3 UK, 4 All, 5 Major Program, 6 Employee
        Task<List<int>> LastActivityRevisionIds( DateTime start, DateTime end, int filter = 0, int id = 0, bool refreshCache = false, int keepCacheInDays = 0 );
    
    }
    public interface IHelpContentRepository: IEntityBaseRepository<HelpContent>{}
    public interface IMessageRepository: IEntityBaseRepository<Message>{
        List<Message> ProcessMessageQueue(IConfiguration configuration, IWebHostEnvironment environment);
        bool ScheduleTrainingMessage(string type, Training training, KersUser To, DateTimeOffset? ScheduledFor = null);
        public bool ScheduleLadderMessage(string type, LadderApplication application, KersUser To);
    }
    public interface IFiscalYearRepository: IEntityBaseRepository<FiscalYear>{
        FiscalYear currentFiscalYear(string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false);
        FiscalYear nextFiscalYear(string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false);
        FiscalYear previoiusFiscalYear( string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false );
        FiscalYear byName(string name, string type);
        FiscalYear byDate(DateTime date, string type, Boolean includeExtendedTo = false, Boolean afterAvailableAt = false);
    }
    public interface IAffirmativeActionPlanRevisionRepository: IEntityBaseRepository<AffirmativeActionPlanRevision>{}
    public interface ISnapDirectRepository{
        string TotalByMonth(FiscalYear fiscalYear, Boolean refreshCache = false);
        string TotalByCounty(FiscalYear fiscalYear, Boolean refreshCache = false);
        string TotalByEmployee(FiscalYear fiscalYear, bool refreshCache = false);
        string PersonalHourDetails(FiscalYear fiscalYear, bool refreshCache = false);
        string SitesPerPersonPerMonth(FiscalYear fiscalYear, bool refreshCache = false);
        string SpecificSiteNamesByMonth(FiscalYear fiscalYear, Boolean refreshCache = false);
        string NumberofDeliverySitesbyTypeofSetting(FiscalYear fiscalYear, Boolean refreshCache = false);
        string MethodsUsedRecordCount(FiscalYear fiscalYear, Boolean refreshCache = false);
        string IndividualContactTotals(FiscalYear fiscalYear, Boolean refreshCache = false);
        string EstimatedSizeofAudiencesReached(FiscalYear fiscalYear, Boolean refreshCache = false);
        string IndirectByEmployee(FiscalYear fiscalYear, bool refreshCache = false);
        string SessionTypebyMonth(FiscalYear fiscalYear, Boolean refreshCache = false);
        string AudienceAgeCategory(FiscalYear fiscalYear, Boolean refreshCache = false);
        
    }

    public interface ISnapPolicyRepository{
        string AimedTowardsImprovement(FiscalYear fiscalYear, bool refreshCache = false);
        string PartnerCategory(FiscalYear fiscalYear, bool refreshCache = false);
        string AgentCommunityEventDetail(FiscalYear fiscalYear, bool refreshCache = false);
        string PartnersOfACounty(int countyId, FiscalYear fiscalYear, bool refreshCache = false);
    }

    public interface ISnapFinancesRepository{
        string CopiesSummarybyCountyAgents(FiscalYear fiscalYear, bool refreshCache = false );
        string CopiesSummarybyCountyNotAgents(FiscalYear fiscalYear, bool refreshCache = false);
        string CopiesDetailAgents(FiscalYear fiscalYear, bool refreshCache = false);
        string CopiesDetailNotAgents(FiscalYear fiscalYear, bool refreshCache = false);
        string ReimbursementNepAssistants(FiscalYear fiscalYear, bool refreshCache = false);
        string ReimbursementCounty(FiscalYear fiscalYear, bool refreshCache = false);
    }
    public interface ISnapInDirectRepository: IEntityBaseRepository<SnapIndirect>{}
    public interface ISnapCommitmentRepository{
        Task<string> CommitmentSummary(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> CommitmentHoursDetail(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> AgentsWithoutCommitment(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> SummaryByPlanningUnit(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> SummaryByPlanningUnitNotNEPAssistants(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> ReinforcementItems(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> ReinforcementItemsPerCounty(FiscalYear fiscalYear, bool refreshCache = false);
        Task<string> SuggestedIncentiveItems(FiscalYear fiscalYear, bool refreshCache = false);
    }
    public interface IStoryRepository: IEntityBaseRepository<Story>{
        List<int> LastStoryRevisionIds( FiscalYear fiscalYear, int filter = 4, int id = 0);
        List<StoryRevision> LastStoryRevisions( FiscalYear fiscalYear);
        Task<List<StoryViewModel>> LastStoriesWithImages(FiscalYear fiscalYear = null, int filter = 4, int id = 0, int amount = 6, bool refreshCache = false, int keepCacheInDays = 0);
        Task<List<StoryViewModel>> LastStoriesWithoutImages(FiscalYear fiscalYear = null, int filter = 4, int id = 0, int amount = 6, bool refreshCache = false, int keepCacheInDays = 0);
        Task<List<StoryViewModel>> LastStories(FiscalYear fiscalYear = null, int amount = 4, int PlanningUnitId = 0, int MajorProgramId = 0, bool refreshCache = false );
        Task<List<StoryViewModel>> LastStoriesByUser( int userId, FiscalYear fiscalYear = null, int amount = 4, bool refreshCache = false );
    }
}
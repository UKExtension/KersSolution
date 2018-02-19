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

namespace Kers.Models.Abstract
{
    public interface IzEmpRptProfileRepository : IEntityBaseRepository<zEmpRptProfile> { }
    //public interface IzActivityRepository : IEntityBaseRepository<zActivity> { }
    //public interface IzzGeneralLocationRepository : IEntityBaseRepository<zzGeneralLocation> { }
    public interface  IzEmpRoleTypeRepository : IEntityBaseRepository<zEmpRoleType> { }
    public interface  IInitiativeRepository : IEntityBaseRepository<StrategicInitiative> { }
    public interface IKersUserRepository : IEntityBaseRepository<KersUser>{ 
        KersUser findByProfileID(int ProfileId);
        KersUser createUserFromProfile(zEmpRptProfile profile);

        List<zEmpRoleType> roles(int id);

        zEmpRoleType roleForId(int id);
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
        List<ExpenseSummary> SummariesPerFiscalYear(KersUser user, FiscalYear fiscalYear);
        float Breakfast(ExpenseRevision expense);
        float Lunch(ExpenseRevision expense);
        float Dinner(ExpenseRevision expense);
        float MileageRate(KersUser user, int year, int month);
    }
    public interface IActivityRepository: IEntityBaseRepository<Activity>{
        List<ActivityRevision> PerMonth(KersUser user, int year, int month, string order);
        List<PerUnitActivities> ProcessUnitActivities(List<ActivityUnitResult> activities, IDistributedCache _cache);
        List<PerProgramActivities> ProcessMajorProgramActivities(List<ActivityMajorProgramResult> activities, IDistributedCache _cache);
        List<PerPersonActivities> ProcessPersonActivities(List<ActivityPersonResult> activities, IDistributedCache _cache);
        List<int> LastActivityRevisionIds( FiscalYear fiscalYear, IDistributedCache _cache);
    }

    public interface IContactRepository: IEntityBaseRepository<Contact>{
        List<PerUnitActivities> ProcessUnitContacts(List<ContactUnitResult> contacts, List<PerUnitActivities> result, IDistributedCache _cache);
        List<PerProgramActivities> ProcessMajorProgramContacts(List<ContactMajorProgramResult> contacts, List<PerProgramActivities> result, IDistributedCache _cache);
        List<PerPersonActivities> ProcessPersonContacts(List<ContactPersonResult> contacts, List<PerPersonActivities> result, IDistributedCache _cache);
    }
    public interface IHelpContentRepository: IEntityBaseRepository<HelpContent>{}
    public interface IFiscalYearRepository: IEntityBaseRepository<FiscalYear>{
        FiscalYear currentFiscalYear(string type);
        FiscalYear nextFiscalYear(string type);
    }
    public interface IAffirmativeActionPlanRevisionRepository: IEntityBaseRepository<AffirmativeActionPlanRevision>{}
    public interface ISnapDirectRepository{
        string TotalByMonth(FiscalYear fiscalYear, Boolean refreshCache = false);
        string TotalByCounty(FiscalYear fiscalYear, Boolean refreshCache = false);
        string TotalByEmployee(FiscalYear fiscalYear, bool refreshCache = false);
    }

    public interface ISnapPolicyRepository{
        string AimedTowardsImprovement(FiscalYear fiscalYear, bool refreshCache = false);
        string PartnerCategory(FiscalYear fiscalYear, bool refreshCache = false);
    }
    public interface ISnapInDirectRepository: IEntityBaseRepository<SnapIndirect>{}
}
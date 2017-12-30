using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.KERS_SNAPED2017;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kers.Models.Contexts
{

    public partial class KERS_SNAPED2017Context : DbContext
    {
        public KERS_SNAPED2017Context(DbContextOptions<KERS_SNAPED2017Context> options) : base(options)
        {
        }


        public virtual DbSet<zSnapEdActivity> zSnapEdActivities { get; set; }
        public virtual DbSet<zzSnapEdSessionType> zzSnapEdSessionTypes { get; set; }

        public virtual DbSet<zzSnapEdDeliverySite> zzSnapEdDeliverySites { get; set; }
        /*
        public virtual DbSet<zSnapEdBudgetReimbursementsCounty> zSnapEdBudgetReimbursementsCounties { get; set; }
        public virtual DbSet<zSnapEdBudgetReimbursementsNepAssistant> zSnapEdBudgetReimbursementsNepAssistants { get; set; }
        public virtual DbSet<zSnapEdCommitmentWorksheet> zSnapEdCommitmentWorksheets { get; set; }
        public virtual DbSet<zzSnapEdBudgetAllowance> zzSnapEdBudgetAllowances { get; set; }
        public virtual DbSet<zzSnapEdCommunityAimedTowardImprovementInType> zzSnapEdCommunityAimedTowardImprovementInTypes { get; set; }
        public virtual DbSet<zzSnapEdCommunityFocusType> zzSnapEdCommunityFocusTypes { get; set; }
        public virtual DbSet<zzSnapEdCommunityPartnerType> zzSnapEdCommunityPartnerTypes { get; set; }
        public virtual DbSet<zzSnapEdContactCategory> zzSnapEdContactCategories { get; set; }
        public virtual DbSet<zzSnapEdCountyBudget> zzSnapEdCountyBudgets { get; set; }
        public virtual DbSet<zzSnapEdDailyHour> zzSnapEdDailyHours { get; set; }
        
        public virtual DbSet<zzSnapEdMonth> zzSnapEdMonths { get; set; }
        
 */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}

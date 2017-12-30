using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace Kers.Models.Contexts
{

    public class KERSmainContext : DbContext
    {
        
        public KERSmainContext(DbContextOptions<KERSmainContext> options) : base(options)
        {
        }

/*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./KERSmain.db");
        }
*/
        public virtual DbSet<SAP_HR_ACTIVE> SAP_HR_ACTIVE { get; set; }
        public virtual DbSet<zEmpRptProfile> zEmpRptProfiles { get; set; }
        public virtual DbSet<zExtServiceLog> zExtServiceLogs { get; set; }
        public virtual DbSet<zzCESdistrict> zzCESdistricts { get; set; }
        public virtual DbSet<zzCESregion> zzCESregions { get; set; }
        public virtual DbSet<zzCESregion_district> zzCESregion_district { get; set; }
        public virtual DbSet<zzExtensionPosition> zzExtensionPositions { get; set; }
        public virtual DbSet<zzGeneralLocation> zzGeneralLocations { get; set; }
        public virtual DbSet<zzHour> zzHours { get; set; }
        public virtual DbSet<zzInstitution> zzInstitutions { get; set; }
        public virtual DbSet<zzLookup> zzLookups { get; set; }
        public virtual DbSet<zzLsvAutoUpdateInventory> zzLsvAutoUpdateInventories { get; set; }
        public virtual DbSet<zzLsvHardSubscriber> zzLsvHardSubscribers { get; set; }
        public virtual DbSet<zzLsvHardSubscribersBAK> zzLsvHardSubscribersBAKs { get; set; }
        public virtual DbSet<zzLsvMembership> zzLsvMemberships { get; set; }
        public virtual DbSet<zzPac> zzPacs { get; set; }
        public virtual DbSet<zzPlanningUnit> zzPlanningUnits { get; set; }
        public virtual DbSet<zzSpeedSort> zzSpeedSorts { get; set; }
        public virtual DbSet<zzUScounty> zzUScounties { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder
                .Entity<zzCESregion_district>()
                .HasKey(zzCESregion_district => new { zzCESregion_district.rID, zzCESregion_district.dID });
        
        
            modelBuilder.Entity<zEmpRptProfile>()
                .HasOne(s => s.zzExtensionPosition)
                .WithMany(c => c.zEmpRptProfiles)
                .HasForeignKey(s => s.positionID)
                .HasPrincipalKey(c => c.posCode);


            modelBuilder.Entity<zEmpRptProfile>()
                .HasOne(s => s.zzPlaningUnit)
                .WithMany(c => c.zEmpRptProfiles)
                .HasForeignKey(s => s.planningUnitID)
                .HasPrincipalKey(c => c.planningUnitID);


            modelBuilder.Entity<zEmpRptProfile>()
                .HasOne(s => s.zzGeneralLocation)
                .WithMany(c => c.zEmpRptProfiles)
                .HasForeignKey(s => s.locationID)
                .HasPrincipalKey(c => c.locationID);

            
            
        }
    }
}

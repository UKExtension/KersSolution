using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.UKCAReporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace Kers.Models.Contexts
{

    public class KERSreportingContext : DbContext
    {
        
        public KERSreportingContext(DbContextOptions<KERSreportingContext> options) : base(options)
        {
        }

/*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./KERSmain.db");
        }
*/
        
        public virtual DbSet<zInServiceTrainingCatalog> zInServiceTrainingCatalog { get; set; }
        public virtual DbSet<zInServiceTrainingEnrollment> zInServiceTrainingEnrollment {get;set;}
        public virtual DbSet<zCesEvent> zCesEvent {get;set;}
        public virtual DbSet<zCesCountyEvent> zCesCountyEvent {get; set;}
        public virtual DbSet<zCesTaxExemptEntity> zCesTaxExemptEntity {get;set;}
        public virtual DbSet<zCesTaxExemptHowFundsHandledLookup> zCesTaxExemptHowFundsHandledLookup {get;set;}
        public virtual DbSet<zCesTaxExemptFinancialYearLookup> zCesTaxExemptFinancialYearLookup {get;set;}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            
            
        }
    }
}

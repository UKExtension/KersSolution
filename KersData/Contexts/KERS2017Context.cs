using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.KERS2017;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Kers.Models.Contexts
{

    public partial class KERS2017Context : DbContext
    {
        
        public KERS2017Context(DbContextOptions<KERS2017Context> options)
            : base(options)
        {
            
        }

        public virtual DbSet<zMap> zMaps { get; set; }
        public virtual DbSet<zProgramPlan> zProgramPlans { get; set; }
        public virtual DbSet<zzPacs> zzPacs { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
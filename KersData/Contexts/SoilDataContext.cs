using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.SoilData;




namespace Kers.Models.Contexts
{

    public class SoilDataContext : DbContext
    {
        
        public SoilDataContext(DbContextOptions<SoilDataContext> options) : base(options)
        {
        }


        public virtual DbSet<CountyCode> CountyCodes { get; set; }
        public virtual DbSet<CountyNote> CountyNotes { get; set; }
        public virtual DbSet<FarmerAddress> FarmerAddress { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             

            
            
        }
    }
}

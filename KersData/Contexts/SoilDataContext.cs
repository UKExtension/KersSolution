using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.SoilData;




namespace Kers.Models.Contexts
{

    public class SoilDataContext : DbContext
    {
        
        public SoilDataContext(DbContextOptions<SoilDataContext> options) : base(options){
        }


        public virtual DbSet<CountyCode> CountyCodes { get; set; }
        public virtual DbSet<CountyNote> CountyNotes { get; set; }
        public virtual DbSet<FarmerAddress> FarmerAddress { get; set; }
        public virtual DbSet<FarmerForReport> FarmerForReport { get; set; }
        public virtual DbSet<TestResults> TestResults { get; set; }
        public virtual DbSet<SoilReport> SoilReport { get; set; }
        public virtual DbSet<SoilReportBundle> SoilReportBundle { get; set; }
        public virtual DbSet<FormTypeSignees> FormTypeSignees {get;set;}
        public virtual DbSet<SoilReportStatus> SoilReportStatus {get;set;}
        public virtual DbSet<SoilReportStatusChange> SoilReportStatusChange {get; set;}
        public virtual DbSet<TypeForm> TypeForm {get;set;}
        public virtual DbSet<SampleInfoBundle> SampleInfoBundle {get;set;}
        public virtual DbSet<SampleAttributeType> SampleAttributeType {get;set;}
        public virtual DbSet<SampleAttribute> SampleAttribute {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){

            modelBuilder.Entity<SampleAttribute>().HasData(
                new SampleAttribute{Id=1,Name="Barley", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=2,Name="Barley/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=3,Name="Burley Tobacco", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=4,Name="Canola", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=5,Name="Canola/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=6,Name="Corn", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=7,Name="Dark Tobacco", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=8,Name="Forage Crops (multiple)", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=9,Name="Forage Sorghum", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=10,Name="Grain Crops (multiple)", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=11,Name="Grain Sorghum", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=12,Name="Hemp", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=13,Name="Millet", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=14,Name="Oats", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=15,Name="Oats/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=16,Name="Rye", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=17,Name="Rye/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=18,Name="Small Grains", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=19,Name="Small Grains/Corn", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=20,Name="Small Grains/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=21,Name="Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=22,Name="Sunflower", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=23,Name="Tobacco Beds", SampleAttributeTypeId =  1 },
                new SampleAttribute{Id=24,Name="Triticale", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=25,Name="Triticale/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=26,Name="Wheat", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=27,Name="Wheat/Corn", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=28,Name="Wheat/Soybeans", SampleAttributeTypeId = 1 },
                new SampleAttribute{Id=29,Name="Other", SampleAttributeTypeId = 1 },

                new SampleAttribute{Id=30,Name="Barley", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=31,Name="Barley/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=32,Name="Burley Tobacco", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=33,Name="Canola", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=34,Name="Canola/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=35,Name="Corn", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=36,Name="Dark Tobacco", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=37,Name="Forage Crops (multiple)", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=38,Name="Forage Sorghum", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=39,Name="Grain Crops (multiple)", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=40,Name="Grain Sorghum", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=41,Name="Hemp", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=42,Name="Millet", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=43,Name="Oats", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=44,Name="Oats/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=45,Name="Rye", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=46,Name="Rye/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=47,Name="Small Grains", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=48,Name="Small Grains/Corn", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=49,Name="Small Grains/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=50,Name="Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=51,Name="Sunflower", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=52,Name="Tobacco Beds", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=53,Name="Triticale", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=54,Name="Triticale/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=55,Name="Wheat", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=56,Name="Wheat/Corn", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=57,Name="Wheat/Soybeans", SampleAttributeTypeId = 2 },
                new SampleAttribute{Id=58,Name="Other", SampleAttributeTypeId = 2 },

                new SampleAttribute{Id=59,Name="Conventional Tillage", SampleAttributeTypeId = 3 },
                new SampleAttribute{Id=60,Name="No Tillage", SampleAttributeTypeId = 3 },
                new SampleAttribute{Id=61,Name="Doublecrop-Conventional", SampleAttributeTypeId = 3 },
                new SampleAttribute{Id=62,Name="Doublecrop-No Till", SampleAttributeTypeId = 3 },

                new SampleAttribute{Id=63,Name="Conventional Tillage", SampleAttributeTypeId = 4 },
                new SampleAttribute{Id=64,Name="No Tillage", SampleAttributeTypeId = 4 },
                new SampleAttribute{Id=65,Name="Hay or Pasture < 4yrs.", SampleAttributeTypeId = 4 },
                new SampleAttribute{Id=66,Name="Hay or Pasture > 4yrs.", SampleAttributeTypeId = 4 },
                new SampleAttribute{Id=67,Name="Doublecrop-Conventional", SampleAttributeTypeId = 4 },
                new SampleAttribute{Id=68,Name="Doublecrop-No Till", SampleAttributeTypeId = 4 },

                new SampleAttribute{Id=69,Name="Grain", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=70,Name="Cover Crop", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=71,Name="Silage", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=72,Name="Tobacco", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=73,Name="Silage-Grain (double crop)", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=74,Name="Grain-Grain (double crop)", SampleAttributeTypeId = 5 },
                new SampleAttribute{Id=75,Name="Silage-Silage (double crop)", SampleAttributeTypeId = 5 },

                new SampleAttribute{Id=76,Name="Well Drained", SampleAttributeTypeId = 6 },
                new SampleAttribute{Id=77,Name="Moderately Well", SampleAttributeTypeId = 6 },
                new SampleAttribute{Id=78,Name="Somewhat Poorly", SampleAttributeTypeId = 6 },
                new SampleAttribute{Id=79,Name="Poorly Drained", SampleAttributeTypeId = 6 },
                new SampleAttribute{Id=80,Name="Poorly, but tiled", SampleAttributeTypeId = 6 },

                new SampleAttribute{Id=81,Name="Alfalfa", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=82,Name="Alfalfa/Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=83,Name="Bermudagrass, common", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=84,Name="Bermudagrass, improved", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=85,Name="Birdsfoot Trefoil", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=86,Name="Bluegrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=87,Name="Bluegrass/White Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=88,Name="Bluestem", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=89,Name="Buffer or Filter Strip", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=90,Name="Clover/Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=91,Name="Cool Season Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=92,Name="Crownvetch", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=93,Name="Fescue", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=94,Name="Fescue/Lespedeza (multiple)", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=95,Name="Fescue/White Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=96,Name="Indiangrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=97,Name="Lespedeza", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=98,Name="Lespedeza/Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=99,Name="Native Grassland Restoration", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=100, Name = "Orchardgrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=101, Name = "Orchardgrass/Red Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=102, Name = "Orchardgrass/White Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=103, Name = "Other", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=104, Name = "Red Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=105, Name = "Red Clover/Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=106, Name = "Side-oats grama", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=107, Name = "Sorghum Sudangrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=108, Name = "Sudangrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=109, Name = "Switchgrass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=110, Name = "Timothy", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=111, Name = "Timothy/Red Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=112, Name = "Warm Season Annual Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=113, Name = "Warm Season Native Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=114, Name = "White Clover", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=115, Name = "White Clover/Grass", SampleAttributeTypeId = 17 },
                new SampleAttribute{Id=116, Name = "Wildlife Food Plot", SampleAttributeTypeId = 17 },

                new SampleAttribute{Id=117, Name = "New Seeding", SampleAttributeTypeId = 18 },
                new SampleAttribute{Id=118, Name = "Renovation", SampleAttributeTypeId = 18 },
                new SampleAttribute{Id=119, Name = "Annual Top Dressing", SampleAttributeTypeId = 18 },

                new SampleAttribute{Id=120, Name = "Pasture", SampleAttributeTypeId = 19 },
                new SampleAttribute{Id=121, Name = "Cover Crop", SampleAttributeTypeId = 19 },
                new SampleAttribute{Id=122, Name = "Hay", SampleAttributeTypeId = 19 },
                new SampleAttribute{Id=123, Name = "Seed Production", SampleAttributeTypeId = 19 },
                new SampleAttribute{Id=124, Name = "Horse Pasture", SampleAttributeTypeId = 19 },

                new SampleAttribute{Id=125, Name = "Apples", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=126, Name = "Azalea/Rhododendron", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=127, Name = "Blueberries", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=128, Name = "Brambles", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=129, Name = "Broadleaved Evergreen Tree or Shrub", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=130, Name = "Deciduous Shrub", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=131, Name = "Deciduous Tree", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=132, Name = "Flower Garden", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=133, Name = "Grape Arbor", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=134, Name = "Ground Covers", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=135, Name = "Needled Evergreen Tree or Shrub", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=136, Name = "Other", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=137, Name = "Peaches", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=138, Name = "Roses", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=139, Name = "Strawberries", SampleAttributeTypeId = 7 },
                new SampleAttribute{Id=140, Name = "Vegetable Garden", SampleAttributeTypeId = 7 },

                new SampleAttribute{Id=141, Name = "New Planting or Seeding", SampleAttributeTypeId = 8 },
                new SampleAttribute{Id=142, Name = "Plant Maintenance", SampleAttributeTypeId = 8 },

                new SampleAttribute{Id=143, Name = "Mostly Sunny Location", SampleAttributeTypeId = 9 },
                new SampleAttribute{Id=144, Name = "Mostly Shady Location", SampleAttributeTypeId = 9 },

                new SampleAttribute{Id=145, Name = "Bermudagrass", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=146, Name = "Creeping Bentgrass", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=147, Name = "Fine Fescue", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=148, Name = "Kentucky Bluegrass", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=149, Name = "Perennial Ryegrass", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=150, Name = "Tall Fescue", SampleAttributeTypeId = 20 },
                new SampleAttribute{Id=151, Name = "Zoysiagrass", SampleAttributeTypeId = 20 },

                new SampleAttribute{Id=152, Name = "Home Lawn", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=153, Name = "Golf Green", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=154, Name = "Golf Tee", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=155, Name = "Golf Fairway", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=156, Name = "Sod Production", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=157, Name = "Athletic Field", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=158, Name = "General Turf", SampleAttributeTypeId = 21 },
                new SampleAttribute{Id=159, Name = "Other Location", SampleAttributeTypeId = 21 },

                new SampleAttribute{Id=160, Name = "New Planting or Seeding", SampleAttributeTypeId = 22 },
                new SampleAttribute{Id=161, Name = "Plant Maintenance", SampleAttributeTypeId = 22 },

                new SampleAttribute{Id=162, Name = "Mostly Sunny Location", SampleAttributeTypeId = 23 },
                new SampleAttribute{Id=163, Name = "Mostly Shady Location", SampleAttributeTypeId = 23 },

                new SampleAttribute{Id=164, Name = "Annuals", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=165, Name = "Apples", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=166, Name = "Asparagus", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=167, Name = "Azaleas", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=168, Name = "Beans (snap,dry,lima,etc.)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=169, Name = "Blueberries", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=170, Name = "Brambles", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=171, Name = "Bulbs", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=172, Name = "Cherries, Tart", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=173, Name = "Cole Crops (broccoli, etc.)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=174, Name = "Conifers (not pines or junipers)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=175, Name = "Conifers, junipers", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=176, Name = "Conifers, pines", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=177, Name = "Cool Peas", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=178, Name = "Corn, Sweet", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=179, Name = "Cucumbers", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=180, Name = "Currants and Gooseberries", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=181, Name = "Deciduous Shrubs", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=182, Name = "Deciduous Trees", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=183, Name = "Eggplant", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=184, Name = "Evergreen Shrubs, Broadleaved", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=185, Name = "Evergreen Trees, Broadleaved", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=186, Name = "Grapes", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=187, Name = "Greens (collards, kale, etc.)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=188, Name = "Hollies", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=189, Name = "Muskmelons (cantaloupes)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=190, Name = "Okra", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=191, Name = "Onions (green & bulb)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=192, Name = "Other", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=193, Name = "Peaches", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=194, Name = "Pears", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=195, Name = "Pecans", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=196, Name = "Peppers (bell & pimento)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=197, Name = "Perrenials (not bulbs)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=198, Name = "Plums", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=199, Name = "Potatoes", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=200, Name = "Rhododendrons", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=201, Name = "Rhubarb", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=202, Name = "Root Crops (beets, carrots,etc.)", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=203, Name = "Southern Peas", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=204, Name = "Squash & Pumpkins", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=205, Name = "Strawberries", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=206, Name = "Sweet Potatoes", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=207, Name = "Tomatoes", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=208, Name = "Walnuts", SampleAttributeTypeId = 10 },
                new SampleAttribute{Id=209, Name = "Watermelons", SampleAttributeTypeId = 10 },

                new SampleAttribute{Id=210, Name = "Annuals", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=211, Name = "Apples", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=212, Name = "Asparagus", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=213, Name = "Azaleas", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=214, Name = "Beans (snap,dry,lima,etc.)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=215, Name = "Blueberries", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=216, Name = "Brambles", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=217, Name = "Bulbs", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=218, Name = "Cherries, Tart", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=219, Name = "Cole Crops (broccoli, etc.)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=220, Name = "Conifers (not pines or junipers)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=221, Name = "Conifers, junipers", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=222, Name = "Conifers, pines", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=223, Name = "Cool Peas", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=224, Name = "Corn, Sweet", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=225, Name = "Cucumbers", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=226, Name = "Currants and Gooseberries", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=227, Name = "Deciduous Shrubs", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=228, Name = "Deciduous Trees", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=229, Name = "Eggplant", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=230, Name = "Evergreen Shrubs, Broadleaved", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=231, Name = "Evergreen Trees, Broadleaved", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=232, Name = "Grapes", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=233, Name = "Greens (collards, kale, etc.)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=234, Name = "Hollies", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=235, Name = "Muskmelons (cantaloupes)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=236, Name = "Okra", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=237, Name = "Onions (green & bulb)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=238, Name = "Other", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=239, Name = "Peaches", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=240, Name = "Pears", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=241, Name = "Pecans", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=242, Name = "Peppers (bell & pimento)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=243, Name = "Perrenials (not bulbs)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=244, Name = "Plums", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=245, Name = "Potatoes", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=246, Name = "Rhododendrons", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=247, Name = "Rhubarb", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=248, Name = "Root Crops (beets, carrots,etc.)", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=249, Name = "Southern Peas", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=250, Name = "Squash & Pumpkins", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=251, Name = "Strawberries", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=252, Name = "Sweet Potatoes", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=253, Name = "Tomatoes", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=254, Name = "Walnuts", SampleAttributeTypeId = 11 },
                new SampleAttribute{Id=255, Name = "Watermelons", SampleAttributeTypeId = 11 },

                new SampleAttribute{Id=256, Name = "Bare sod", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=257, Name = "Black plastic mulch", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=258, Name = "Conventional Tillage", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=259, Name = "Limited Tillage with no weed control", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=260, Name = "Limited Tillage with weed control", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=261, Name = "No tillage", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=262, Name = "Organic mulch", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=263, Name = "Plants grown through black plastic mulch", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=264, Name = "Sod Nursery", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=265, Name = "Sod orchard", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=266, Name = "Tilled Nursery with weed control", SampleAttributeTypeId = 12 },
                new SampleAttribute{Id=267, Name = "Sampled before planting", SampleAttributeTypeId = 13 },
                new SampleAttribute{Id=268, Name = "Irrigated", SampleAttributeTypeId = 14},
                new SampleAttribute{Id=269, Name = "Not Irrigated", SampleAttributeTypeId = 14},
                new SampleAttribute{Id=270, Name = "Moderately Well Drained", SampleAttributeTypeId = 15},
                new SampleAttribute{Id=271, Name = "Poorly Drained", SampleAttributeTypeId = 15},
                new SampleAttribute{Id=272, Name = "Somewhat Poorly Drained", SampleAttributeTypeId = 15},
                new SampleAttribute{Id=273, Name = "Well Drained", SampleAttributeTypeId = 15}
            );











            modelBuilder.Entity<SampleAttributeType>().HasData(
                new SampleAttributeType{ Id = 1, Name = "Primary Crop", TypeFormId = 1, Order = 1 },
                new SampleAttributeType{ Id = 2, Name = "Previous Crop", TypeFormId = 1, Order = 3 },
                new SampleAttributeType{ Id = 3, Name = "Primary Management", TypeFormId = 1, Order = 4 },
                new SampleAttributeType{ Id = 4, Name = "Previous Management", TypeFormId = 1, Order = 6 },
                new SampleAttributeType{ Id = 5, Name = "Primary Use", TypeFormId = 1, Order = 7 },
                new SampleAttributeType{ Id = 6, Name = "Soil Drainage", TypeFormId = 1, Order = 11 },

                new SampleAttributeType{ Id = 7, Name = "Plant", TypeFormId = 2, Order = 1 },
                new SampleAttributeType{ Id = 8, Name = "Maintenance", TypeFormId = 2, Order = 3 },
                new SampleAttributeType{ Id = 9, Name = "Sunny or Shady", TypeFormId = 2, Order = 4 },

                new SampleAttributeType{ Id = 10, Name = "Primary Crop", TypeFormId = 3, Order = 1 },
                new SampleAttributeType{ Id = 11, Name = "Previous Crop", TypeFormId = 3, Order = 2 },
                new SampleAttributeType{ Id = 12, Name = "Primary Management", TypeFormId = 3, Order = 3 },
                new SampleAttributeType{ Id = 13, Name = "Previous Management", TypeFormId = 3, Order = 4 },
                new SampleAttributeType{ Id = 14, Name = "Sampling Time", TypeFormId = 3, Order = 5 },
                new SampleAttributeType{ Id = 15, Name = "Irrigation", TypeFormId = 3, Order = 6 },
                new SampleAttributeType{ Id = 16, Name = "Soil Drainage", TypeFormId = 3, Order = 7 },

                new SampleAttributeType{ Id = 17, Name = "Primary Crop", TypeFormId = 4, Order = 1 },
                new SampleAttributeType{ Id = 18, Name = "Primary Management", TypeFormId = 4, Order = 4 },
                new SampleAttributeType{ Id = 19, Name = "Primary Use", TypeFormId = 4, Order = 7 },

                new SampleAttributeType{ Id = 20, Name = "Plant", TypeFormId = 5, Order = 1 },
                new SampleAttributeType{ Id = 21, Name = "Location", TypeFormId = 5, Order = 2 },
                new SampleAttributeType{ Id = 22, Name = "Maintenance", TypeFormId = 5, Order = 3 },
                new SampleAttributeType{ Id = 23, Name = "Sunny or Shady", TypeFormId = 5, Order = 4 }
            );










            modelBuilder.Entity<CountyCode>().HasData(
                new CountyCode{Id=1, Name = "ADAIR", Code = "001", CountyID = 1},
                new CountyCode{Id=2, Name = "ALLEN", Code = "003", CountyID = 3},
                new CountyCode{Id=3, Name = "ANDERSON", Code = "005", CountyID = 5},
                new CountyCode{Id=4, Name = "BALLARD", Code = "007", CountyID = 7},
                new CountyCode{Id=5, Name = "BARREN", Code = "009", CountyID = 9},
                new CountyCode{Id=6, Name = "BATH", Code = "011", CountyID = 11},
                new CountyCode{Id=7, Name = "BELL", Code = "013", CountyID = 13},
                new CountyCode{Id=8, Name = "BOONE", Code = "015", CountyID = 15},
                new CountyCode{Id=9, Name = "BOURBON", Code = "017", CountyID = 17},
                new CountyCode{Id=10, Name = "BOYD", Code = "019", CountyID = 19},
                new CountyCode{Id=11, Name = "BOYLE", Code = "021", CountyID = 21},
                new CountyCode{Id=12, Name = "BRACKEN", Code = "023", CountyID = 23},
                new CountyCode{Id=13, Name = "BREATHITT", Code = "025", CountyID = 25},
                new CountyCode{Id=14, Name = "BRECKINRIDGE", Code = "027", CountyID = 27},
                new CountyCode{Id=15, Name = "BULLITT", Code = "029", CountyID = 29},
                new CountyCode{Id=16, Name = "BUTLER", Code = "031", CountyID = 31},
                new CountyCode{Id=17, Name = "CALDWELL", Code = "033", CountyID = 33},
                new CountyCode{Id=18, Name = "CALLOWAY", Code = "035", CountyID = 35},
                new CountyCode{Id=19, Name = "CAMPBELL", Code = "037", CountyID = 37},
                new CountyCode{Id=20, Name = "CARLISLE", Code = "039", CountyID = 39},
                new CountyCode{Id=21, Name = "CARROLL", Code = "041", CountyID = 41},
                new CountyCode{Id=22, Name = "CARTER", Code = "043", CountyID = 43},
                new CountyCode{Id=23, Name = "CASEY", Code = "045", CountyID = 45},
                new CountyCode{Id=24, Name = "CHRISTIAN", Code = "047", CountyID = 47},
                new CountyCode{Id=25, Name = "CLARK", Code = "049", CountyID = 49},
                new CountyCode{Id=26, Name = "CLAY", Code = "051", CountyID = 51},
                new CountyCode{Id=27, Name = "CLINTON", Code = "053", CountyID = 53},
                new CountyCode{Id=28, Name = "CRITTENDEN", Code = "055", CountyID = 55},
                new CountyCode{Id=29, Name = "CUMBERLAND", Code = "057", CountyID = 57},
                new CountyCode{Id=30, Name = "DAVIESS", Code = "059", CountyID = 59},
                new CountyCode{Id=31, Name = "EDMONSON", Code = "061", CountyID = 61},
                new CountyCode{Id=32, Name = "ELLIOTT", Code = "063", CountyID = 63},
                new CountyCode{Id=33, Name = "ESTILL", Code = "065", CountyID = 65},
                new CountyCode{Id=34, Name = "FAYETTE", Code = "067", CountyID = 67},
                new CountyCode{Id=35, Name = "FLEMING", Code = "069", CountyID = 69},
                new CountyCode{Id=36, Name = "FLOYD", Code = "071", CountyID = 71},
                new CountyCode{Id=37, Name = "FRANKLIN", Code = "073", CountyID = 73},
                new CountyCode{Id=38, Name = "FULTON", Code = "075", CountyID = 75},
                new CountyCode{Id=39, Name = "GALLATIN", Code = "077", CountyID = 77},
                new CountyCode{Id=40, Name = "GARRARD", Code = "079", CountyID = 79},
                new CountyCode{Id=41, Name = "GRANT", Code = "081", CountyID = 81},
                new CountyCode{Id=42, Name = "GRAVES", Code = "083", CountyID = 83},
                new CountyCode{Id=43, Name = "GRAYSON", Code = "085", CountyID = 85},
                new CountyCode{Id=44, Name = "GREEN", Code = "087", CountyID = 87},
                new CountyCode{Id=45, Name = "GREENUP", Code = "089", CountyID = 89},
                new CountyCode{Id=46, Name = "HANCOCK", Code = "091", CountyID = 91},
                new CountyCode{Id=47, Name = "HARDIN", Code = "093", CountyID = 93},
                new CountyCode{Id=48, Name = "HARLAN", Code = "095", CountyID = 95},
                new CountyCode{Id=49, Name = "HARRISON", Code = "097", CountyID = 97},
                new CountyCode{Id=50, Name = "HART", Code = "099", CountyID = 99},
                new CountyCode{Id=51, Name = "HENDERSON", Code = "101", CountyID = 101},
                new CountyCode{Id=52, Name = "HENRY", Code = "103", CountyID = 103},
                new CountyCode{Id=53, Name = "HICKMAN", Code = "105", CountyID = 105},
                new CountyCode{Id=54, Name = "HOPKINS", Code = "107", CountyID = 107},
                new CountyCode{Id=55, Name = "JACKSON", Code = "109", CountyID = 109},
                new CountyCode{Id=56, Name = "JEFFERSON", Code = "111", CountyID = 111},
                new CountyCode{Id=57, Name = "JESSAMINE", Code = "113", CountyID = 113},
                new CountyCode{Id=58, Name = "JOHNSON", Code = "115", CountyID = 115},
                new CountyCode{Id=59, Name = "KENTON", Code = "117", CountyID = 117},
                new CountyCode{Id=60, Name = "KNOTT", Code = "119", CountyID = 119},
                new CountyCode{Id=61, Name = "KNOX", Code = "121", CountyID = 121},
                new CountyCode{Id=62, Name = "LARUE", Code = "123", CountyID = 123},
                new CountyCode{Id=63, Name = "LAUREL", Code = "125", CountyID = 125},
                new CountyCode{Id=64, Name = "LAWRENCE", Code = "127", CountyID = 127},
                new CountyCode{Id=65, Name = "LEE", Code = "129", CountyID = 129},
                new CountyCode{Id=66, Name = "LESLIE", Code = "131", CountyID = 131},
                new CountyCode{Id=67, Name = "LETCHER", Code = "133", CountyID = 133},
                new CountyCode{Id=68, Name = "LEWIS", Code = "135", CountyID = 135},
                new CountyCode{Id=69, Name = "LINCOLN", Code = "137", CountyID = 137},
                new CountyCode{Id=70, Name = "LIVINGSTON", Code = "139", CountyID = 139},
                new CountyCode{Id=71, Name = "LOGAN", Code = "141", CountyID = 141},
                new CountyCode{Id=72, Name = "LYON", Code = "143", CountyID = 143},
                new CountyCode{Id=73, Name = "MCCRACKEN", Code = "145", CountyID = 145},
                new CountyCode{Id=74, Name = "MCCREARY", Code = "147", CountyID = 147},
                new CountyCode{Id=75, Name = "MCLEAN", Code = "149", CountyID = 149},
                new CountyCode{Id=76, Name = "MADISON", Code = "151", CountyID = 151},
                new CountyCode{Id=77, Name = "MAGOFFIN", Code = "153", CountyID = 153},
                new CountyCode{Id=78, Name = "MARION", Code = "155", CountyID = 155},
                new CountyCode{Id=79, Name = "MARSHALL", Code = "157", CountyID = 157},
                new CountyCode{Id=80, Name = "MARTIN", Code = "159", CountyID = 159},
                new CountyCode{Id=81, Name = "MASON", Code = "161", CountyID = 161},
                new CountyCode{Id=82, Name = "MEADE", Code = "163", CountyID = 163},
                new CountyCode{Id=83, Name = "MENIFEE", Code = "165", CountyID = 165},
                new CountyCode{Id=84, Name = "MERCER", Code = "167", CountyID = 167},
                new CountyCode{Id=85, Name = "METCALFE", Code = "169", CountyID = 169},
                new CountyCode{Id=86, Name = "MONROE", Code = "171", CountyID = 171},
                new CountyCode{Id=87, Name = "MONTGOMERY", Code = "173", CountyID = 173},
                new CountyCode{Id=88, Name = "MORGAN", Code = "175", CountyID = 175},
                new CountyCode{Id=89, Name = "MUHLENBERG", Code = "177", CountyID = 177},
                new CountyCode{Id=90, Name = "NELSON", Code = "179", CountyID = 179},
                new CountyCode{Id=91, Name = "NICHOLAS", Code = "181", CountyID = 181},
                new CountyCode{Id=92, Name = "OHIO", Code = "183", CountyID = 183},
                new CountyCode{Id=93, Name = "OLDHAM", Code = "185", CountyID = 185},
                new CountyCode{Id=94, Name = "OWEN", Code = "187", CountyID = 187},
                new CountyCode{Id=95, Name = "OWSLEY", Code = "189", CountyID = 189},
                new CountyCode{Id=96, Name = "PENDLETON", Code = "191", CountyID = 191},
                new CountyCode{Id=97, Name = "PERRY", Code = "193", CountyID = 193},
                new CountyCode{Id=98, Name = "PIKE", Code = "195", CountyID = 195},
                new CountyCode{Id=99, Name = "POWELL", Code = "197", CountyID = 197},
                new CountyCode{Id=100, Name = "PULASKI", Code = "199", CountyID = 199},
                new CountyCode{Id=101, Name = "ROBERTSON", Code = "201", CountyID = 201},
                new CountyCode{Id=102, Name = "ROCKCASTLE", Code = "203", CountyID = 203},
                new CountyCode{Id=103, Name = "ROWAN", Code = "205", CountyID = 205},
                new CountyCode{Id=104, Name = "RUSSELL", Code = "207", CountyID = 207},
                new CountyCode{Id=105, Name = "SCOTT", Code = "209", CountyID = 209},
                new CountyCode{Id=106, Name = "SHELBY", Code = "211", CountyID = 211},
                new CountyCode{Id=107, Name = "SIMPSON", Code = "213", CountyID = 213},
                new CountyCode{Id=108, Name = "SPENCER", Code = "215", CountyID = 215},
                new CountyCode{Id=109, Name = "TAYLOR", Code = "217", CountyID = 217},
                new CountyCode{Id=110, Name = "TODD", Code = "219", CountyID = 219},
                new CountyCode{Id=111, Name = "TRIGG", Code = "221", CountyID = 221},
                new CountyCode{Id=112, Name = "TRIMBLE", Code = "223", CountyID = 223},
                new CountyCode{Id=113, Name = "UNION", Code = "225", CountyID = 225},
                new CountyCode{Id=114, Name = "WARREN", Code = "227", CountyID = 227},
                new CountyCode{Id=115, Name = "WASHINGTON", Code = "229", CountyID = 229},
                new CountyCode{Id=116, Name = "WAYNE", Code = "231", CountyID = 231},
                new CountyCode{Id=117, Name = "WEBSTER", Code = "233", CountyID = 233},
                new CountyCode{Id=118, Name = "WHITLEY", Code = "235", CountyID = 235},
                new CountyCode{Id=119, Name = "WOLFE", Code = "237", CountyID = 237},
                new CountyCode{Id=120, Name = "WOODFORD", Code = "239", CountyID = 239}
            );

            modelBuilder.Entity<FarmerAddress>().HasData(
                    new FarmerAddress{
                        Id = 1,
                        CountyCodeId = 19,
                        First = "Robert",
                        Last = "Lauer",
                        Address = "12439 Shaw Goetz Road",
                        City = "California",
                        St = "KY",
                        Status = "N",
                        WorkNumber = "(859) 257-2708",
                        HomeNumber = "859.635.3069",
                        FarmerID = "2",
                        Zip = "41007",
                        EmailAddress = "Lauer@email.com",
                        Latitude = "38.020492",
                        Longitude = "-84.510185"
                    },
                    new FarmerAddress{
                        Id = 2,
                        CountyCodeId = 19,
                        First = "Allan",
                        Last = "Smith",
                        Address = "12424 Flagg Spring Pike",
                        City = "California",
                        St = "KY",
                        Status = "N",
                        FarmerID = "3",
                        Zip = "41007",
                        EmailAddress = "Seiter@email.com"
                    },
                    new FarmerAddress{
                        Id = 3,
                        CountyCodeId = 19,
                        First = "Bill",
                        Last = "Miller",
                        Address = "401 Visalia Road",
                        City = "Alexandria",
                        St = "KY",
                        HomeNumber = "(859) 635-2070",
                        Status = "N",
                        FarmerID = "4",
                        Zip = "41001"
                    },
                    new FarmerAddress{
                        Id = 4,
                        CountyCodeId = 34,
                        First = "Tom",
                        Last = "Jones",
                        Title = "III",
                        Address = "1 Main St",
                        City = "Lexington",
                        St = "KY",
                        HomeNumber = "859 222 1111",
                        FarmerID = "1429",
                        Zip = "40502",
                        EmailAddress = "tjones@email.com",
                        Latitude = "38.1111111",
                        Longitude = "-83.22222222"
                    }
            );

            modelBuilder.Entity<CountyNote>().HasData(
                new CountyNote{
                        Id = 1,
                        CountyCodeId = 1,
                        Name = "_Alfalfa",
                        Note = "LIME:  The soil pH is low [good] [high].  Apply lime as indicated above."+
                                "[Lime application is not needed this year].P (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0)."+
                                "Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth]"+
                                "[August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).\n" +
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
                    },
                    new CountyNote{
                        Id = 2,
                        CountyCodeId = 1,
                        Name = "_Corn",
                        Note = "LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].\n"+
                                "FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0)."+
                                "Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] "+
                                "[August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below)."+
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service."
                    },
                    new CountyNote{
                        Id = 3,
                        CountyCodeId = 1,
                        Name = "_Hay/Pasture-Grass Only",
                        Note = "Note LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].\n\n"+
                                "FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  "+
                                "Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).\n\n"+
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
                    },
                    new CountyNote{
                        Id = 4,
                        CountyCodeId = 1,
                        Name = "_Soybeans",
                        Note = "Note LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].\n\n"+
                                "FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  "+
                                "Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to "+
                                "August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).\n\n"+
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
                    },
                    new CountyNote{
                        Id = 5,
                        CountyCodeId = 2,
                        Name = "LAWN",
                        Note = "LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].\n\n"+
                                "FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0)."+
                                " Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 "+
                                "to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).\n\n"+
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
                    },
                    new CountyNote{
                        Id = 6,
                        CountyCodeId = 2,
                        Name = "marijuana",
                        Note = "Note This will make you high\n"
                    },
                    new CountyNote{
                        Id = 7,
                        CountyCodeId = 2,
                        Name = "MUSKMELON (black plastic)",
                        Note = "LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].\n\n"+
                                "FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  "+
                                "Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to "+
                                "August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).\n\n"+
                                "If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service.\n\n"
                    }
            );
            modelBuilder.Entity<TypeForm>().HasData(
                new TypeForm{
                    Id = 1,
                    Name = "Agriculture",
                    Code = "A",
                    Note = ""
                },
                new TypeForm{
                    Id = 2,
                    Name = "Home lawn and garden",
                    Code = "H",
                    Note = ""
                },
                new TypeForm{
                    Id = 3,
                    Name = "Commercial horticulture",
                    Code = "C",
                    Note = ""
                }
            );
 /* 
            modelBuilder.Entity<SoilReportBundle>()
                .HasMany<SoilReport>( r => r.Reports)
                .WithOne( t => t.SoilReportBundle)
                .IsRequired(false);

            modelBuilder.Entity<SoilReport>()
                .HasOne<FarmerForReport>()
                .WithOne( t => t.SoilReport)
                .HasForeignKey<FarmerForReport>( r => r.FarmerID)
                .HasPrincipalKey<SoilReport>( m => m.FarmerID);
 */

        }
    }
}

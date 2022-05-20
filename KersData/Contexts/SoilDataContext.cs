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
                new SampleAttribute{1,"Barley", 1 },
                new SampleAttribute{2,"Barley/Soybeans", 1 },
                new SampleAttribute{3,"Burley Tobacco", 1 },
                new SampleAttribute{4,"Canola", 1 },
                new SampleAttribute{5,"Canola/Soybeans", 1 },
                new SampleAttribute{6,"Corn", 1 },
                new SampleAttribute{7,"Dark Tobacco", 1 },
                new SampleAttribute{8,"Forage Crops (multiple)", 1 },
                new SampleAttribute{9,"Forage Sorghum", 1 },
                new SampleAttribute{10,"Grain Crops (multiple)", 1 },
                new SampleAttribute{11,"Grain Sorghum", 1 },
                new SampleAttribute{12,"Hemp", 1 },
                new SampleAttribute{13,"Millet", 1 },
                new SampleAttribute{14,"Oats", 1 },
                new SampleAttribute{15,"Oats/Soybeans", 1 },
                new SampleAttribute{16,"Rye", 1 },
                new SampleAttribute{17,"Rye/Soybeans", 1 },
                new SampleAttribute{18,"Small Grains", 1 },
                new SampleAttribute{19,"Small Grains/Corn", 1 },
                new SampleAttribute{20,"Small Grains/Soybeans", 1 },
                new SampleAttribute{21,"Soybeans", 1 },
                new SampleAttribute{22,"Sunflower", 1 },
                new SampleAttribute{23,"Tobacco Beds", 1 },
                new SampleAttribute{24,"Triticale", 1 },
                new SampleAttribute{25,"Triticale/Soybeans", 1 },
                new SampleAttribute{26,"Wheat", 1 },
                new SampleAttribute{27,"Wheat/Corn", 1 },
                new SampleAttribute{28,"Wheat/Soybeans", 1 },
                new SampleAttribute{29,"Other", 1 },
                
                new SampleAttribute{30,"Barley", 2 },
                new SampleAttribute{31,"Barley/Soybeans", 2 },
                new SampleAttribute{32,"Burley Tobacco", 2 },
                new SampleAttribute{33,"Canola", 2 },
                new SampleAttribute{34,"Canola/Soybeans", 2 },
                new SampleAttribute{35,"Corn", 2 },
                new SampleAttribute{36,"Dark Tobacco", 2 },
                new SampleAttribute{37,"Forage Crops (multiple)", 2 },
                new SampleAttribute{38,"Forage Sorghum", 2 },
                new SampleAttribute{39,"Grain Crops (multiple)", 2 },
                new SampleAttribute{40,"Grain Sorghum", 2 },
                new SampleAttribute{41,"Hemp", 2 },
                new SampleAttribute{42,"Millet", 2 },
                new SampleAttribute{43,"Oats", 2 },
                new SampleAttribute{44,"Oats/Soybeans", 2 },
                new SampleAttribute{45,"Rye", 2 },
                new SampleAttribute{46,"Rye/Soybeans", 2 },
                new SampleAttribute{47,"Small Grains", 2 },
                new SampleAttribute{48,"Small Grains/Corn", 2 },
                new SampleAttribute{49,"Small Grains/Soybeans", 2 },
                new SampleAttribute{50,"Soybeans", 2 },
                new SampleAttribute{51,"Sunflower", 2 },
                new SampleAttribute{52,"Tobacco Beds", 2 },
                new SampleAttribute{53,"Triticale", 2 },
                new SampleAttribute{54,"Triticale/Soybeans", 2 },
                new SampleAttribute{55,"Wheat", 2 },
                new SampleAttribute{56,"Wheat/Corn", 2 },
                new SampleAttribute{57,"Wheat/Soybeans", 2 },
                new SampleAttribute{58,"Other", 2 },


                new SampleAttribute{59,"Conventional Tillage", 3 },
                new SampleAttribute{60,"No Tillage", 3 },
                new SampleAttribute{61,"Doublecrop-Conventional", 3 },
                new SampleAttribute{62,"Doublecrop-No Till", 3 },

                new SampleAttribute{63,"Conventional Tillage", 4 },
                new SampleAttribute{64,"No Tillage", 4 },
                new SampleAttribute{65,"Hay or Pasture < 4yrs.", 4 },
                new SampleAttribute{66,"Hay or Pasture > 4yrs.", 4 },
                new SampleAttribute{67,"Doublecrop-Conventional", 4 },
                new SampleAttribute{68,"Doublecrop-No Till", 4 },

                new SampleAttribute{69,"Grain", 5 },
                new SampleAttribute{70,"Cover Crop", 5 },
                new SampleAttribute{71,"Silage", 5 },
                new SampleAttribute{72,"Tobacco", 5 },
                new SampleAttribute{73,"Silage-Grain (double crop)", 5 },
                new SampleAttribute{74,"Grain-Grain (double crop)", 5 },
                new SampleAttribute{75,"Silage-Silage (double crop)", 5 },

                new SampleAttribute{76,"Well Drained", 6 },
                new SampleAttribute{77,"Moderately Well", 6 },
                new SampleAttribute{78,"Somewhat Poorly", 6 },
                new SampleAttribute{79,"Poorly Drained", 6 },
                new SampleAttribute{80,"Poorly, but tiled", 6 },

                new SampleAttribute{81,"Alfalfa", 17 },
                new SampleAttribute{82,"Alfalfa/Grass", 17 },
                new SampleAttribute{83,"Bermudagrass, common", 17 },
                new SampleAttribute{84,"Bermudagrass, improved", 17 },
                new SampleAttribute{85,"Birdsfoot Trefoil", 17 },
                new SampleAttribute{86,"Bluegrass", 17 },
                new SampleAttribute{87,"Bluegrass/White Clover", 17 },
                new SampleAttribute{88,"Bluestem", 17 },
                new SampleAttribute{89,"Buffer or Filter Strip", 17 },
                new SampleAttribute{90,"Clover/Grass", 17 },
                new SampleAttribute{91,"Cool Season Grass", 17 },
                new SampleAttribute{92,"Crownvetch", 17 },
                new SampleAttribute{93,"Fescue", 17 },
                new SampleAttribute{94,"Fescue/Lespedeza (multiple)", 17 },
                new SampleAttribute{95,"Fescue/White Clover", 17 },
                new SampleAttribute{96,"Indiangrass", 17 },
                new SampleAttribute{97,"Lespedeza", 17 },
                new SampleAttribute{98,"Lespedeza/Grass", 17 },
                new SampleAttribute{99,"Native Grassland Restoration", 17 },
                new SampleAttribute{100,"Orchardgrass", 17 },
                new SampleAttribute{101,"Orchardgrass/Red Clover", 17 },
                new SampleAttribute{102,"Orchardgrass/White Clover", 17 },
                new SampleAttribute{103,"Other", 17 },
                new SampleAttribute{104,"Red Clover", 17 },
                new SampleAttribute{105,"Red Clover/Grass", 17 },
                new SampleAttribute{106,"Side-oats grama", 17 },
                new SampleAttribute{107,"Sorghum Sudangrass", 17 },
                new SampleAttribute{108,"Sudangrass", 17 },
                new SampleAttribute{109,"Switchgrass", 17 },
                new SampleAttribute{110,"Timothy", 17 },
                new SampleAttribute{111,"Timothy/Red Clover", 17 },
                new SampleAttribute{112,"Warm Season Annual Grass", 17 },
                new SampleAttribute{113,"Warm Season Native Grass", 17 },
                new SampleAttribute{114,"White Clover", 17 },
                new SampleAttribute{115,"White Clover/Grass", 17 },
                new SampleAttribute{116,"Wildlife Food Plot", 17 },


                new SampleAttribute{117,"New Seeding", 18 },
                new SampleAttribute{118,"Renovation", 18 },
                new SampleAttribute{119,"Annual Top Dressing", 18 },

                new SampleAttribute{120,"Pasture", 19 },
                new SampleAttribute{121,"Cover Crop", 19 },
                new SampleAttribute{122,"Hay", 19 },
                new SampleAttribute{123,"Seed Production", 19 },
                new SampleAttribute{124,"Horse Pasture", 19 },



                new SampleAttribute{125,"Apples", 7 },
                new SampleAttribute{126,"Azalea/Rhododendron", 7 },
                new SampleAttribute{127,"Blueberries", 7 },
                new SampleAttribute{128,"Brambles", 7 },
                new SampleAttribute{129,"Broadleaved Evergreen Tree or Shrub", 7 },
                new SampleAttribute{130,"Deciduous Shrub", 7 },
                new SampleAttribute{131,"Deciduous Tree", 7 },
                new SampleAttribute{132,"Flower Garden", 7 },
                new SampleAttribute{133,"Grape Arbor", 7 },
                new SampleAttribute{134,"Ground Covers", 7 },
                new SampleAttribute{135,"Needled Evergreen Tree or Shrub", 7 },
                new SampleAttribute{136,"Other", 7 },
                new SampleAttribute{137,"Peaches", 7 },
                new SampleAttribute{138,"Roses", 7 },
                new SampleAttribute{139,"Strawberries", 7 },
                new SampleAttribute{140,"Vegetable Garden", 7 },

                new SampleAttribute{141,"New Planting or Seeding", 8 },
                new SampleAttribute{142,"Plant Maintenance", 8 },


                new SampleAttribute{143,"Mostly Sunny Location", 9 },
                new SampleAttribute{144,"Mostly Shady Location", 9 },

                new SampleAttribute{145,"Bermudagrass", 20 },
                new SampleAttribute{146,"Creeping Bentgrass", 20 },
                new SampleAttribute{147,"Fine Fescue", 20 },
                new SampleAttribute{148,"Kentucky Bluegrass", 20 },
                new SampleAttribute{149,"Perennial Ryegrass", 20 },
                new SampleAttribute{150,"Tall Fescue", 20 },
                new SampleAttribute{151,"Zoysiagrass", 20 },

                new SampleAttribute{152,"Home Lawn", 21 },
                new SampleAttribute{153,"Golf Green", 21 },
                new SampleAttribute{154,"Golf Tee", 21 },
                new SampleAttribute{155,"Golf Fairway", 21 },
                new SampleAttribute{156,"Sod Production", 21 },
                new SampleAttribute{157,"Athletic Field", 21 },
                new SampleAttribute{158,"General Turf", 21 },
                new SampleAttribute{159,"Other Location", 21 },

                new SampleAttribute{160,"New Planting or Seeding", 22 },
                new SampleAttribute{161,"Plant Maintenance", 22 },

                new SampleAttribute{162,"Mostly Sunny Location", 23 },
                new SampleAttribute{163,"Mostly Shady Location", 23 },

                new SampleAttribute{164,"Annuals", 10 },
                new SampleAttribute{165,"Apples", 10 },
                new SampleAttribute{166,"Asparagus", 10 },
                new SampleAttribute{167,"Azaleas", 10 },
                new SampleAttribute{168,"Beans (snap,dry,lima,etc.)", 10 },
                new SampleAttribute{169,"Blueberries", 10 },
                new SampleAttribute{170,"Brambles", 10 },
                new SampleAttribute{171,"Bulbs", 10 },
                new SampleAttribute{172,"Cherries, Tart", 10 },
                new SampleAttribute{173,"Cole Crops (broccoli, etc.)", 10 },
                new SampleAttribute{174,"Conifers (not pines or junipers)", 10 },
                new SampleAttribute{175,"Conifers, junipers", 10 },
                new SampleAttribute{176,"Conifers, pines", 10 },
                new SampleAttribute{177,"Cool Peas", 10 },
                new SampleAttribute{178,"Corn, Sweet", 10 },
                new SampleAttribute{179,"Cucumbers", 10 },
                new SampleAttribute{180,"Currants and Gooseberries", 10 },
                new SampleAttribute{181,"Deciduous Shrubs", 10 },
                new SampleAttribute{182,"Deciduous Trees", 10 },
                new SampleAttribute{183,"Eggplant", 10 },
                new SampleAttribute{184,"Evergreen Shrubs, Broadleaved", 10 },
                new SampleAttribute{185,"Evergreen Trees, Broadleaved", 10 },
                new SampleAttribute{186,"Grapes", 10 },
                new SampleAttribute{187,"Greens (collards, kale, etc.)", 10 },
                new SampleAttribute{188,"Hollies", 10 },
                new SampleAttribute{189,"Muskmelons (cantaloupes)", 10 },
                new SampleAttribute{190,"Okra", 10 },
                new SampleAttribute{191,"Onions (green & bulb)", 10 },
                new SampleAttribute{192,"Other", 10 },
                new SampleAttribute{193,"Peaches", 10 },
                new SampleAttribute{194,"Pears", 10 },
                new SampleAttribute{195,"Pecans", 10 },
                new SampleAttribute{196,"Peppers (bell & pimento)", 10 },
                new SampleAttribute{197,"Perrenials (not bulbs)", 10 },
                new SampleAttribute{198,"Plums", 10 },
                new SampleAttribute{199,"Potatoes", 10 },
                new SampleAttribute{200,"Rhododendrons", 10 },
                new SampleAttribute{201,"Rhubarb", 10 },
                new SampleAttribute{202,"Root Crops (beets, carrots,etc.)", 10 },
                new SampleAttribute{203,"Southern Peas", 10 },
                new SampleAttribute{204,"Squash & Pumpkins", 10 },
                new SampleAttribute{205,"Strawberries", 10 },
                new SampleAttribute{206,"Sweet Potatoes", 10 },
                new SampleAttribute{207,"Tomatoes", 10 },
                new SampleAttribute{208,"Walnuts", 10 },
                new SampleAttribute{209,"Watermelons", 10 },





                new SampleAttribute{210,"Annuals", 11 },
                new SampleAttribute{211,"Apples", 11 },
                new SampleAttribute{212,"Asparagus", 11 },
                new SampleAttribute{213,"Azaleas", 11 },
                new SampleAttribute{214,"Beans (snap,dry,lima,etc.)", 11 },
                new SampleAttribute{215,"Blueberries", 11 },
                new SampleAttribute{216,"Brambles", 11 },
                new SampleAttribute{217,"Bulbs", 11 },
                new SampleAttribute{218,"Cherries, Tart", 11 },
                new SampleAttribute{219,"Cole Crops (broccoli, etc.)", 11 },
                new SampleAttribute{220,"Conifers (not pines or junipers)", 11 },
                new SampleAttribute{221,"Conifers, junipers", 11 },
                new SampleAttribute{222,"Conifers, pines", 11 },
                new SampleAttribute{223,"Cool Peas", 11 },
                new SampleAttribute{224,"Corn, Sweet", 11 },
                new SampleAttribute{225,"Cucumbers", 11 },
                new SampleAttribute{226,"Currants and Gooseberries", 11 },
                new SampleAttribute{227,"Deciduous Shrubs", 11 },
                new SampleAttribute{228,"Deciduous Trees", 11 },
                new SampleAttribute{229,"Eggplant", 11 },
                new SampleAttribute{230,"Evergreen Shrubs, Broadleaved", 11 },
                new SampleAttribute{231,"Evergreen Trees, Broadleaved", 11 },
                new SampleAttribute{232,"Grapes", 11 },
                new SampleAttribute{233,"Greens (collards, kale, etc.)", 11 },
                new SampleAttribute{234,"Hollies", 11 },
                new SampleAttribute{235,"Muskmelons (cantaloupes)", 11 },
                new SampleAttribute{236,"Okra", 11 },
                new SampleAttribute{237,"Onions (green & bulb)", 11 },
                new SampleAttribute{238,"Other", 11 },
                new SampleAttribute{239,"Peaches", 11 },
                new SampleAttribute{240,"Pears", 11 },
                new SampleAttribute{241,"Pecans", 11 },
                new SampleAttribute{242,"Peppers (bell & pimento)", 11 },
                new SampleAttribute{243,"Perrenials (not bulbs)", 11 },
                new SampleAttribute{244,"Plums", 11 },
                new SampleAttribute{245,"Potatoes", 11 },
                new SampleAttribute{246,"Rhododendrons", 11 },
                new SampleAttribute{247,"Rhubarb", 11 },
                new SampleAttribute{248,"Root Crops (beets, carrots,etc.)", 11 },
                new SampleAttribute{249,"Southern Peas", 11 },
                new SampleAttribute{250,"Squash & Pumpkins", 11 },
                new SampleAttribute{251,"Strawberries", 11 },
                new SampleAttribute{252,"Sweet Potatoes", 11 },
                new SampleAttribute{253,"Tomatoes", 11 },
                new SampleAttribute{254,"Walnuts", 11 },
                new SampleAttribute{255,"Watermelons", 11 },





                new SampleAttribute{256,"Bare sod", 12 },
                new SampleAttribute{257,"Black plastic mulch", 12 },
                new SampleAttribute{258,"Conventional Tillage", 12 },
                new SampleAttribute{259,"Limited Tillage with no weed control", 12 },
                new SampleAttribute{260,"Limited Tillage with weed control", 12 },
                new SampleAttribute{261,"No tillage", 12 },
                new SampleAttribute{262,"Organic mulch", 12 },
                new SampleAttribute{263,"Plants grown through black plastic mulch", 12 },
                new SampleAttribute{264,"Sod Nursery", 12 },
                new SampleAttribute{265,"Sod orchard", 12 },
                new SampleAttribute{266,"Tilled Nursery with weed control", 12 },

                new SampleAttribute{267,"Sampled before planting", 13 },

                new SampleAttribute{268,"Irrigated", 14 },
                new SampleAttribute{269,"Not Irrigated", 14 },


                new SampleAttribute{270,"Moderately Well Drained", 15 },
                new SampleAttribute{271,"Poorly Drained", 15 },
                new SampleAttribute{272,"Somewhat Poorly Drained", 15 },
                new SampleAttribute{273,"Well Drained", 15 }
            );











            modelBuilder.Entity<SampleAttributeType>().HasData(
                new SampleAttributeType{1,"Primary Crop", 1, 1 },
                new SampleAttributeType{2,"Previous Crop", 1, 3 },
                new SampleAttributeType{3,"Primary Management", 1, 4 },
                new SampleAttributeType{4,"Previous Management", 1, 6 },
                new SampleAttributeType{5,"Primary Use", 1, 7 },
                new SampleAttributeType{6,"Soil Drainage", 1, 11 },

                new SampleAttributeType{7,"Plant", 2, 1 },
                new SampleAttributeType{8,"Maintenance", 2, 3 },
                new SampleAttributeType{9,"Sunny or Shady", 2, 4 },

                new SampleAttributeType{10,"Primary Crop", 3, 1 },
                new SampleAttributeType{11,"Previous Crop", 3, 2 },
                new SampleAttributeType{12,"Primary Management", 3, 3 },
                new SampleAttributeType{13,"Previous Management", 3, 4 },
                new SampleAttributeType{14,"Sampling Time", 3, 5 },
                new SampleAttributeType{15,"Irrigation", 3, 6 },
                new SampleAttributeType{16,"Soil Drainage", 3, 7 },

                new SampleAttributeType{17,"Primary Crop", 4, 1 },
                new SampleAttributeType{18,"Primary Management", 4, 4 },
                new SampleAttributeType{19,"Primary Use", 4, 7 },

                new SampleAttributeType{20,"Plant", 5, 1 },
                new SampleAttributeType{21,"Location", 5, 2 },
                new SampleAttributeType{22,"Maintenance", 5, 3 },
                new SampleAttributeType{23,"Sunny or Shady", 5, 4 }
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

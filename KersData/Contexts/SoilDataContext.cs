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
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
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
        
        
        }
    }
}

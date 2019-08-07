using System.Linq;
using Kers.Models.Entities.SoilData;


namespace Kers.Models.Contexts
{
    public static class SoilDataContextExtensions
    {
        
        public static void Seed(this SoilDataContext db)
        {
            seedCounties(db);
        }

        public static void seedCounties(SoilDataContext db){
            if (db.CountyCodes.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new CountyCode{Name = "ADAIR", Code = "001", CountyID = 1},
                    new CountyCode{Name = "ALLEN", Code = "003", CountyID = 3},
                    new CountyCode{Name = "ANDERSON", Code = "005", CountyID = 5},
                    new CountyCode{Name = "BALLARD", Code = "007", CountyID = 7},
                    new CountyCode{Name = "BARREN", Code = "009", CountyID = 9},
                    new CountyCode{Name = "BATH", Code = "011", CountyID = 11},
                    new CountyCode{Name = "BELL", Code = "013", CountyID = 13},
                    new CountyCode{Name = "BOONE", Code = "015", CountyID = 15},
                    new CountyCode{Name = "BOURBON", Code = "017", CountyID = 17},
                    new CountyCode{Name = "BOYD", Code = "019", CountyID = 19},
                    new CountyCode{Name = "BOYLE", Code = "021", CountyID = 21},
                    new CountyCode{Name = "BRACKEN", Code = "023", CountyID = 23},
                    new CountyCode{Name = "BREATHITT", Code = "025", CountyID = 25},
                    new CountyCode{Name = "BRECKINRIDGE", Code = "027", CountyID = 27},
                    new CountyCode{Name = "BULLITT", Code = "029", CountyID = 29},
                    new CountyCode{Name = "BUTLER", Code = "031", CountyID = 31},
                    new CountyCode{Name = "CALDWELL", Code = "033", CountyID = 33},
                    new CountyCode{Name = "CALLOWAY", Code = "035", CountyID = 35},
                    new CountyCode{Name = "CAMPBELL", Code = "037", CountyID = 37},
                    new CountyCode{Name = "CARLISLE", Code = "039", CountyID = 39},
                    new CountyCode{Name = "CARROLL", Code = "041", CountyID = 41},
                    new CountyCode{Name = "CARTER", Code = "043", CountyID = 43},
                    new CountyCode{Name = "CASEY", Code = "045", CountyID = 45},
                    new CountyCode{Name = "CHRISTIAN", Code = "047", CountyID = 47},
                    new CountyCode{Name = "CLARK", Code = "049", CountyID = 49},
                    new CountyCode{Name = "CLAY", Code = "051", CountyID = 51},
                    new CountyCode{Name = "CLINTON", Code = "053", CountyID = 53},
                    new CountyCode{Name = "CRITTENDEN", Code = "055", CountyID = 55},
                    new CountyCode{Name = "CUMBERLAND", Code = "057", CountyID = 57},
                    new CountyCode{Name = "DAVIESS", Code = "059", CountyID = 59},
                    new CountyCode{Name = "EDMONSON", Code = "061", CountyID = 61},
                    new CountyCode{Name = "ELLIOTT", Code = "063", CountyID = 63},
                    new CountyCode{Name = "ESTILL", Code = "065", CountyID = 65},
                    new CountyCode{Name = "FAYETTE", Code = "067", CountyID = 67},
                    new CountyCode{Name = "FLEMING", Code = "069", CountyID = 69},
                    new CountyCode{Name = "FLOYD", Code = "071", CountyID = 71},
                    new CountyCode{Name = "FRANKLIN", Code = "073", CountyID = 73},
                    new CountyCode{Name = "FULTON", Code = "075", CountyID = 75},
                    new CountyCode{Name = "GALLATIN", Code = "077", CountyID = 77},
                    new CountyCode{Name = "GARRARD", Code = "079", CountyID = 79},
                    new CountyCode{Name = "GRANT", Code = "081", CountyID = 81},
                    new CountyCode{Name = "GRAVES", Code = "083", CountyID = 83},
                    new CountyCode{Name = "GRAYSON", Code = "085", CountyID = 85},
                    new CountyCode{Name = "GREEN", Code = "087", CountyID = 87},
                    new CountyCode{Name = "GREENUP", Code = "089", CountyID = 89},
                    new CountyCode{Name = "HANCOCK", Code = "091", CountyID = 91},
                    new CountyCode{Name = "HARDIN", Code = "093", CountyID = 93},
                    new CountyCode{Name = "HARLAN", Code = "095", CountyID = 95},
                    new CountyCode{Name = "HARRISON", Code = "097", CountyID = 97},
                    new CountyCode{Name = "HART", Code = "099", CountyID = 99},
                    new CountyCode{Name = "HENDERSON", Code = "101", CountyID = 101},
                    new CountyCode{Name = "HENRY", Code = "103", CountyID = 103},
                    new CountyCode{Name = "HICKMAN", Code = "105", CountyID = 105},
                    new CountyCode{Name = "HOPKINS", Code = "107", CountyID = 107},
                    new CountyCode{Name = "JACKSON", Code = "109", CountyID = 109},
                    new CountyCode{Name = "JEFFERSON", Code = "111", CountyID = 111},
                    new CountyCode{Name = "JESSAMINE", Code = "113", CountyID = 113},
                    new CountyCode{Name = "JOHNSON", Code = "115", CountyID = 115},
                    new CountyCode{Name = "KENTON", Code = "117", CountyID = 117},
                    new CountyCode{Name = "KNOTT", Code = "119", CountyID = 119},
                    new CountyCode{Name = "KNOX", Code = "121", CountyID = 121},
                    new CountyCode{Name = "LARUE", Code = "123", CountyID = 123},
                    new CountyCode{Name = "LAUREL", Code = "125", CountyID = 125},
                    new CountyCode{Name = "LAWRENCE", Code = "127", CountyID = 127},
                    new CountyCode{Name = "LEE", Code = "129", CountyID = 129},
                    new CountyCode{Name = "LESLIE", Code = "131", CountyID = 131},
                    new CountyCode{Name = "LETCHER", Code = "133", CountyID = 133},
                    new CountyCode{Name = "LEWIS", Code = "135", CountyID = 135},
                    new CountyCode{Name = "LINCOLN", Code = "137", CountyID = 137},
                    new CountyCode{Name = "LIVINGSTON", Code = "139", CountyID = 139},
                    new CountyCode{Name = "LOGAN", Code = "141", CountyID = 141},
                    new CountyCode{Name = "LYON", Code = "143", CountyID = 143},
                    new CountyCode{Name = "MC CRACKEN", Code = "145", CountyID = 145},
                    new CountyCode{Name = "MC CREARY", Code = "147", CountyID = 147},
                    new CountyCode{Name = "MC LEAN", Code = "149", CountyID = 149},
                    new CountyCode{Name = "MADISON", Code = "151", CountyID = 151},
                    new CountyCode{Name = "MAGOFFIN", Code = "153", CountyID = 153},
                    new CountyCode{Name = "MARION", Code = "155", CountyID = 155},
                    new CountyCode{Name = "MARSHALL", Code = "157", CountyID = 157},
                    new CountyCode{Name = "MARTIN", Code = "159", CountyID = 159},
                    new CountyCode{Name = "MASON", Code = "161", CountyID = 161},
                    new CountyCode{Name = "MEADE", Code = "163", CountyID = 163},
                    new CountyCode{Name = "MENIFEE", Code = "165", CountyID = 165},
                    new CountyCode{Name = "MERCER", Code = "167", CountyID = 167},
                    new CountyCode{Name = "METCALFE", Code = "169", CountyID = 169},
                    new CountyCode{Name = "MONROE", Code = "171", CountyID = 171},
                    new CountyCode{Name = "MONTGOMERY", Code = "173", CountyID = 173},
                    new CountyCode{Name = "MORGAN", Code = "175", CountyID = 175},
                    new CountyCode{Name = "MUHLENBERG", Code = "177", CountyID = 177},
                    new CountyCode{Name = "NELSON", Code = "179", CountyID = 179},
                    new CountyCode{Name = "NICHOLAS", Code = "181", CountyID = 181},
                    new CountyCode{Name = "OHIO", Code = "183", CountyID = 183},
                    new CountyCode{Name = "OLDHAM", Code = "185", CountyID = 185},
                    new CountyCode{Name = "OWEN", Code = "187", CountyID = 187},
                    new CountyCode{Name = "OWSLEY", Code = "189", CountyID = 189},
                    new CountyCode{Name = "PENDLETON", Code = "191", CountyID = 191},
                    new CountyCode{Name = "PERRY", Code = "193", CountyID = 193},
                    new CountyCode{Name = "PIKE", Code = "195", CountyID = 195},
                    new CountyCode{Name = "POWELL", Code = "197", CountyID = 197},
                    new CountyCode{Name = "PULASKI", Code = "199", CountyID = 199},
                    new CountyCode{Name = "ROBERTSON", Code = "201", CountyID = 201},
                    new CountyCode{Name = "ROCKCASTLE", Code = "203", CountyID = 203},
                    new CountyCode{Name = "ROWAN", Code = "205", CountyID = 205},
                    new CountyCode{Name = "RUSSELL", Code = "207", CountyID = 207},
                    new CountyCode{Name = "SCOTT", Code = "209", CountyID = 209},
                    new CountyCode{Name = "SHELBY", Code = "211", CountyID = 211},
                    new CountyCode{Name = "SIMPSON", Code = "213", CountyID = 213},
                    new CountyCode{Name = "SPENCER", Code = "215", CountyID = 215},
                    new CountyCode{Name = "TAYLOR", Code = "217", CountyID = 217},
                    new CountyCode{Name = "TODD", Code = "219", CountyID = 219},
                    new CountyCode{Name = "TRIGG", Code = "221", CountyID = 221},
                    new CountyCode{Name = "TRIMBLE", Code = "223", CountyID = 223},
                    new CountyCode{Name = "UNION", Code = "225", CountyID = 225},
                    new CountyCode{Name = "WARREN", Code = "227", CountyID = 227},
                    new CountyCode{Name = "WASHINGTON", Code = "229", CountyID = 229},
                    new CountyCode{Name = "WAYNE", Code = "231", CountyID = 231},
                    new CountyCode{Name = "WEBSTER", Code = "233", CountyID = 233},
                    new CountyCode{Name = "WHITLEY", Code = "235", CountyID = 235},
                    new CountyCode{Name = "WOLFE", Code = "237", CountyID = 237},
                    new CountyCode{Name = "WOODFORD", Code = "239", CountyID = 239}
                );          
            db.SaveChanges();
        }


        private static void seedAddresses(SoilDataContext db){
            if (db.FarmerAddress.Any())
            {
                return;   // DB has been seeded
            }
            var CountyCode = db.CountyCodes.Where( c => c.Code == "037").FirstOrDefault();
            if(CountyCode != null){
                db.AddRange(
                    new FarmerAddress{
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
                        First = "Bill",
                        Last = "Miller",
                        Address = "401 Visalia Road",
                        City = "Alexandria",
                        St = "KY",
                        HomeNumber = "(859) 635-2070",
                        Status = "N",
                        FarmerID = "4",
                        Zip = "41001"
                    }
                    
                ); 
            }

            var anotherCountyCode = db.CountyCodes.Where( c => c.Code == "067").FirstOrDefault();
            if(anotherCountyCode != null){
                db.Add(
                    new FarmerAddress{
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
            }     
            db.SaveChanges();
        }

        private static void seedNotes(SoilDataContext db){
            if (db.CountyNotes.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new CountyNote{
                        
                    }
                    
                    );          
            db.SaveChanges();
        }


/* 





1	001	_Alfalfa	"Note LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].P (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).
If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
1	001	_Corn	"LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].

FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).

If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
1	001	_Hay/Pasture-Grass Only	"Note LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].

FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).

If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
1	001	_Soybeans	"Note LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].

FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).

If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
3	003	BEANS (BPM- Black Plastic)	Note
3	003	LAWN	"LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].

FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).

If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
3	003	marijuana	Note This will make you high
3	003	MUSKMELON (black plastic)	"LIME:  The soil pH is low [good] [high].  Apply lime as indicated above.  [Lime application is not needed this year].

FERTILIZER:  Apply ___ lbs per acre DAP (18-46-0), ___lbs per acre muriate of potash (0-0-60), and ___lbs per acre urea (46-0-0).  Urea should be applied between February 15 and March 15 [May 1 and May 15 after initial harvest to stimulate regrowth] [August 1 to August 15 when the goal is for stockpiling hayfields or pastures] (see additional comment below).

If you have any questions regarding these soil test result, feel free to contact me at the Clark County Extension Service. "
1	001	PEPPERS (BPM- Black Plastic)	Note
1	001	POTATOES (COMMERCIAL HORT.)	"LIME:  The soil pH is good.  Lime application is not required this year.

PRE-PLANT N FERTILIZER APPLICATION w/ FERTIGATION AS NEEDED LATE IN THE SEASON:  Apply 165 lbs per acre (3.75 lbs per 1000 sq ft) calicum nitrate (15.5-0-0), incorporate into the soil prior to laying plastic, and fertigate with a weekly light dose of calcium nitrate late in the season as needed to maintain late season vine and fruit development.  Consider weather conditions, condition of the vines, and extent of disease pressure when deciding if/how much to fertigate.

FERTIGATION W/O APPLYING PRE-PLANT N FERTILIZER: Follow only when N fertilizer is not applied pre-plant.  Apply 25 lbs per acre (0.5 lbs per each 1000 sq ft) greenhouse grade calicum nitrate (15.5-0-0) thru the drip system each.  Begin fertigating about two week after plants develop their true leaves. 

If you have any questions regarding this soil test report, feel free to contact me at the Clark County Extension Service."
1	001	SQUASH (BPM- Black Plastic)	mowing at times, and some herbicide applied for weed control), apply 2 applications of fertilizer.  Apply the first application between September and October.  Apply the 2nd application between November and December.  See additional comment regarding lawn care maintenance levels for additional explanation.
1	001	SQUASH and PUMPKINS (conventional till)	LIME:  The soil pH is good.  Lime application is not required this year.  [The soil pH is low.  Apply __ T/A (__ lbs per each 1000 sq ft) of bagged lime.]













        private static void zSnapEdSessionType(KERS_SNAPED2017Context db){
            if (db.zzSnapEdSessionTypes.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new zzSnapEdSessionType{rID=100001, FY = 2017, orderID = 1, snapDirectSessionTypeName = "Single Session"},
                    new zzSnapEdSessionType{rID=100002, FY = 2017, orderID = 2, snapDirectSessionTypeName = "Series - 2 to 4 sessions"},
                    new zzSnapEdSessionType{rID=100003, FY = 2017, orderID = 3, snapDirectSessionTypeName = "Series - 5 to 9 sessions"},
                    new zzSnapEdSessionType{rID=100004, FY = 2017, orderID = 4, snapDirectSessionTypeName = "Series - 10 or more sessions"}
                    );          
            db.SaveChanges();
        }

        private static void zzSnapEdDeliverySite(KERS_SNAPED2017Context db){
            if (db.zzSnapEdDeliverySites.Any())
            {
                return;   // DB has been seeded
            }
            db.AddRange(
                    new zzSnapEdDeliverySite{rID=1001, FY = 2017, orderID = 1, snapDirectDeliverySiteName = "Adult Education and Job Training Sites"},
                    new zzSnapEdDeliverySite{rID=1002, FY = 2017, orderID = 2, snapDirectDeliverySiteName = "Adult Rehabilitation Centers"},
                    new zzSnapEdDeliverySite{rID=1003, FY = 2017, orderID = 3, snapDirectDeliverySiteName = "Churches"},
                    new zzSnapEdDeliverySite{rID=1004, FY = 2017, orderID = 4, snapDirectDeliverySiteName = "Community Centers"},
                    new zzSnapEdDeliverySite{rID=1005, FY = 2017, orderID = 5, snapDirectDeliverySiteName = "Elderly Service Centers"},
                    new zzSnapEdDeliverySite{rID=1006, FY = 2017, orderID = 6, snapDirectDeliverySiteName = "Emergency Food Assistance Sites"},
                    new zzSnapEdDeliverySite{rID=1007, FY = 2017, orderID = 7, snapDirectDeliverySiteName = "Extension Offices"},
                    new zzSnapEdDeliverySite{rID=1008, FY = 2017, orderID = 8, snapDirectDeliverySiteName = "Farmers Markets/Farms"},
                    new zzSnapEdDeliverySite{rID=1009, FY = 2017, orderID = 9, snapDirectDeliverySiteName = "Food Stores/Grocery"},
                    new zzSnapEdDeliverySite{rID=1010, FY = 2017, orderID = 10, snapDirectDeliverySiteName = "Head Start Programs and Day Care"},
                    new zzSnapEdDeliverySite{rID=1011, FY = 2017, orderID = 11, snapDirectDeliverySiteName = "Individual Homes"},
                    new zzSnapEdDeliverySite{rID=1012, FY = 2017, orderID = 12, snapDirectDeliverySiteName = "Libraries"},
                    new zzSnapEdDeliverySite{rID=1013, FY = 2017, orderID = 13, snapDirectDeliverySiteName = "Other Youth Education Sites (includes Parks and Recreation)"},
                    new zzSnapEdDeliverySite{rID=1014, FY = 2017, orderID = 14, snapDirectDeliverySiteName = "Public Housing"},
                    new zzSnapEdDeliverySite{rID=1015, FY = 2017, orderID = 15, snapDirectDeliverySiteName = "Public Schools"},
                    new zzSnapEdDeliverySite{rID=1016, FY = 2017, orderID = 16, snapDirectDeliverySiteName = "Public/Community Health Centers/Hospitals"},
                    new zzSnapEdDeliverySite{rID=1017, FY = 2017, orderID = 17, snapDirectDeliverySiteName = "Shelters"},
                    new zzSnapEdDeliverySite{rID=1018, FY = 2017, orderID = 18, snapDirectDeliverySiteName = "SNAP Offices"},
                    new zzSnapEdDeliverySite{rID=1019, FY = 2017, orderID = 19, snapDirectDeliverySiteName = "WIC Programs"},
                    new zzSnapEdDeliverySite{rID=1020, FY = 2017, orderID = 20, snapDirectDeliverySiteName = "Worksites"},
                    new zzSnapEdDeliverySite{rID=1021, FY = 2017, orderID = 21, snapDirectDeliverySiteName = "Other (please specify)"}

                );          
            db.SaveChanges();
        } */

    }
}
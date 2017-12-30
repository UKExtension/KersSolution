using Kers.Models.Entities.KERS_SNAPED2017;
using Kers.Models.Contexts;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kers.Models.Contexts
{
    public static class KERS_SNAPED2017ContextExtensions
    {
        
        public static void Seed(this KERS_SNAPED2017Context db)
        {

            //zSnapEdSessionType(db);
            //zzSnapEdDeliverySite(db);
        }


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
        }

    }
}
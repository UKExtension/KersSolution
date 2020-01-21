using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class FarmerAddress : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CountyCode CountyCode {get;set;}
        public int CountyCodeId {get;set;}
        public string First {get;set;}
        public string Mi {get;set;}
        public string Last {get;set;}
        public string Title {get;set;}
        public string Modifier {get;set;}
        public string Company {get;set;}
        public string Address {get;set;}
        public string City {get;set;}
        public string St {get;set;}
        public string Status {get;set;}
        public string WorkNumber {get;set;}
        public string DuplicateHouseHold {get;set;}
        public string HomeNumber {get;set;}
        public string Fax {get;set;}
        public string FarmerID {get;set;}
        public string Zip {get;set;}
        public string EmailAddress {get;set;}
        public string Latitude {get;set;}
        public string Longitude {get;set;}
        public string Altitude {get;set;}
        [Column("Historic_FarmerID")]
        public string HistoricFarmerId {get; set;}
        public string FarmerData {get;set;}
        public List<SoilReportBundle> Reports {get;set;}

    }
}
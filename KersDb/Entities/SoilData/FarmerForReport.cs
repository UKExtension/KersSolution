using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class FarmerForReport : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("fmrReportID")]
        public int Id { get; set; }
        [Column("LAB_NUM")]
        public string LabNum {get;set;}
        public string FarmerID {get;set;}
        public string First {get;set;}
        public string Mi {get;set;}
        public string Last {get;set;}
        public string Title {get;set;}
        public string Modifier {get;set;}
        public string Company {get;set;}
        public string Address {get;set;}
        public string City {get;set;}
        [Column("ST")]
        public string St {get;set;}
        [Column("STATUS")]
        public string Status {get;set;}
        public string WorkNumber {get;set;}
        public string DuplicateHouseHold {get;set;}
        public string HomeNumber {get;set;}
        public string Fax {get;set;}
        [Column("ZIP")]
        public string Zip {get;set;}
        [Column("emailaddress")]
        public string EmailAddress {get;set;}
        public string Latitude {get;set;}
        public string Longitude {get;set;}
        public string Altitude {get;set;}
        public string FarmerData {get;set;}
    }
}
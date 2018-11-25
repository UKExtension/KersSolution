using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class CountyVehicle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public int? AddedById {get;set;}
        public KersUser AddedBy {get;set;}
        public string Make {get;set;}
        public string Model {get;set;}
        public string Year {get;set;}
        public string LicenseTag {get;set;}
        public float Odometer {get;set;}
        public string Color {get;set;}
        public bool Enabled {get;set;}
        [Column(TypeName="date")]
        public DateTime DatePurchased {get;set;}
        [Column(TypeName="date")]
        public DateTime DateDispossed {get;set;}
        public DateTimeOffset CreatedDateTime {get;set;}
        public DateTimeOffset LastModifiedDateTime {get;set;}
    }
}
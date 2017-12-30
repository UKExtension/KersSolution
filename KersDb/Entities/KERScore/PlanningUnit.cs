using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class PlanningUnit : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? order { get; set; }
        public string Name { get; set; }
        [Column(TypeName="varchar(50)")]
        public string Code { get; set; }
        public string Description {get;set;}
        public string FullName{get; set;}
        public string Address{get;set;}
        public string Zip {get; set;}
        public string City {get; set; }
        public string Phone {get; set; }
        public string WebSite {get; set;}
        public string Email {get; set;}
        public bool ReportsExtension {get; set;}
        public int? DistrictId {get; set;}
        public District District {get; set;}
        public Region Region {get; set;}
        public int Population {get;set;}
        [Column(TypeName="text")]
        public string GeoFeature {get;set;}
        public int? FIPSCode {get;set;}
    }
}
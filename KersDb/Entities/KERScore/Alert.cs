using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Alert:IEntityBase, IEntityCredentials
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public string Message {get; set;}
        public string UrlRoute {get; set;}
        public int AlertType {get;set;}
        public DateTime Start {get;set;}
        public DateTime End {get;set;}
        public string MoreInfoUrl {get;set;}
        public int? EmployeePositionId {get; set;}
        public int? zEmpRoleTypeId {get; set;}
        public int? isContyStaff {get;set;}
        public ExtensionRegion ExtensionRegion {get;set;}
        public int? ExtensionRegionId {get;set;}
        public ExtensionArea ExtensionArea {get;set;}
        public int? ExtensionAreaId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public int? PlanningUnitId {get;set;}
        public bool Active {get;set;}
        public KersUser CreatedBy {get;set;}
        public DateTime Created {get;set;}
        public DateTime LastUpdated {get;set;}
        
    }
}
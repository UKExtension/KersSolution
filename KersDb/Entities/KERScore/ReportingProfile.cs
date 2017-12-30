using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ReportingProfile : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LinkBlueId {get; set;}
        public string PersonId {get; set;}
        public string Name {get; set;}
        public string Email {get;set;}
        public string EmailAlias {get; set;}
        public int GeneralLocationId {get; set;}
        public GeneralLocation GeneralLocation {get; set;}
        public int PlanningUnitId {get; set;}
        public PlanningUnit PlanningUnit {get; set;}
        public int InstitutionId {get;set;}
        public Institution Institution {get;set;}
        public bool enabled {get;set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore{ 
    public partial class CountyEventPlanningUnit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CountyEventId {get;set;}
        public CountyEvent CountyEvent {get;set;}
        public int PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public bool IsHost {get;set;}
    } 
}
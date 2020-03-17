using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class CongressionalDistrictUnit : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? order { get; set; }
        public PlanningUnit PlanningUnit { get; set; }
        public int PlanningUnitId {get;set;}
        public bool IsMultiDistrict {get;set;}        
    }
}
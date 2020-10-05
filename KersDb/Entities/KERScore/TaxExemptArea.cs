using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
 
    public partial class TaxExemptArea
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TaxExemptId {get;set;}
        public TaxExempt TaxExempt {get;set;}
        public int UnitId {get;set;}
        public PlanningUnit Unit {get;set;}
        public int DistrictId {get;set;}
        public District District {get;set;}
        public int RegionId {get;set;}
        public ExtensionRegion Region {get;set;}
        public int AreaId {get;set;}
        public ExtensionArea Area {get;set;}
    } 
}
namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ExtensionEventLocationConnection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KersUser KersUser{get;set;}
        public int? KersUserId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public int? PlanningUnitId {get;set;}
        public bool active {get;set;}
        public int SelectedCount {get;set;}
        public ExtensionEventLocation ExtensionEventLocation {get;set;}
        public int ExtensionEventLocationId {get;set;}
    }
}

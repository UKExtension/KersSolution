namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    /// <summary>
    /// The type Location.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventLocationConnection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ExtensionEvent ExtensionEvent {get;set;}
        public int? ExtensionEventId {get;set;}
        public KersUser KersUser{get;set;}
        public int? KersUserId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public int? PlanningUnitId {get;set;}
        public bool active {get;set;}
        public ExtensionEventLocation ExtensionEventLocation {get;set;}
        public int ExtensionEventLocationId {get;set;}
    }
}

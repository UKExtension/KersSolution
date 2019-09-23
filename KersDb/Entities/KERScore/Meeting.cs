namespace Kers.Models.Entities.KERScore
{
    public class Meeting : ExtensionEvent
    {
        public string mLocation { get; set; }
        public string mContact { get; set; }
        public int? mClassicId {get;set;}
    }
}
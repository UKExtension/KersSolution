using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class CountyCode : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code {get;set;}
        public string Name {get;set;}
        public int CountyID {get;set;}
        public int PlanningUnitId {get;set;}
        public string ReportEmail {get;set;}
        public string InvoiceEmail {get;set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Entities.SoilData
{

    public partial class FormTypeSignees : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CountyCode PlanningUnit {get;set;}
        public TypeForm TypeForm {get;set;}
        public string Signee {get;set;}
        public string Title {get;set;}
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Entities.SoilData
{

    public partial class SoilReportStatus : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name {get;set;}
        public string Description {get;set;}
        public string CssClass {get;set;}
    }
}
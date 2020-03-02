using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class Areas : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string County {get;set;}
        public int District {get;set;} 
        public string RegionArea {get;set;}
        public int Congressional {get;set;}
        public int IsMulty {get;set;}
    }
}
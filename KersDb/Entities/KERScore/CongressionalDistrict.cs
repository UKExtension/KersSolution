using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class CongressionalDistrict : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? order { get; set; }
        public string Name { get; set; }
        public List<CongressionalDistrictUnit> Units {get;set;}
        
    }
}
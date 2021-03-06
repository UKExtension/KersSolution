using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class District : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? order { get; set; }
        public string Name { get; set; }
        public string AreaName { get; set; }
        public string Description {get;set;}
        public KersUser Admin {get; set;}
        public KersUser Assistant {get;set;}
    }
}
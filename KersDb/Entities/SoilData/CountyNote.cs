using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class CountyNote : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CountyCode CountyCode {get;set;}
        public string Name {get;set;}
        [Column(TypeName = "nvarchar(MAX)")]
        public string Note {get;set;}
    }
}
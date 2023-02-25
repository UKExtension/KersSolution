using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class CountyAutoCoSamNum : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CountyCode CountyCode {get;set;}
        public int CountyCodeId {get;set;}
        public bool AutoSampleNumber {get;set;}
        public int LastSampleNumber {get;set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class SampleAttributeSampleInfoBundle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SampleInfoBundle SampleInfoBundle {get;set;}
        public int SampleInfoBundleId {get;set;}
        public int SampleAttributeId {get;set;}
        public SampleAttribute SampleAttribute {get;set;}
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class SampleInfoBundle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SoilReportBundle SoilReportBundle {get;set;}
        public int SoilReportBundleId {get;set;}
        public int TypeFormId {get;set;}
        public TypeForm TypeForm {get;set;}
        public Purpose Purpose {get;set;}
        public int PurposeId {get;set;}
    }
}
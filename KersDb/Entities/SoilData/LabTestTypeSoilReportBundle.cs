using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class LabTestTypeSoilReportBundle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SoilReportBundle SoilReportBundle {get;set;}
        public int SoilReportBundleId {get;set;}
        public LabTestType LabTestType {get;set;}
        public int LabTestTypeId {get;set;}
    }
}
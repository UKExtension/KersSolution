using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class SampleInterfaceButtonStatus : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SampleInterfaceButton SampleInterfaceButton {get;set;}
        public int SampleInterfaceButtonId {get;set;}
        public SoilReportStatus SoilReportStatus {get;set;}
        public int SoilReportStatusId {get;set;}
    }
}
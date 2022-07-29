using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class OptionalTestSoilReportBundle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OptionalTestId {get;set;}
        public OptionalTest OptionalTest{get;set;}
        public int SoilReportBundleId {get;set;}
        public SoilReportBundle SoilReportBundle {get;set;} 
    }
}
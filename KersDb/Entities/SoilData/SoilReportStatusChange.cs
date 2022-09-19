using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Entities.SoilData
{

    public partial class SoilReportStatusChange : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public SoilReportStatus SoilReportStatus {get;set;}
        public int SoilReportStatusId {get;set;}
        public SoilReportBundle SoilReportBundle {get;set;}
        public int SoilReportBundleId {get;set;}
        public DateTime Created {get;set;}
        public int? KersUserId {get;set;}
    }
}
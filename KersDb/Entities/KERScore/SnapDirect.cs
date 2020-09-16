using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapDirect : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string SiteName {get;set;}
        public int SnapDirectDeliverySiteId {get;set;}
        public SnapDirectDeliverySite SnapDirectDeliverySite {get;set;}
        public int SnapDirectSessionTypeId {get;set;}
        public SnapDirectSessionType SnapDirectSessionType {get;set;}
        public SnapDirectSessionLength SnapDirectSessionLength {get;set;}
        public int? SnapDirectSessionLengthId {get;set;}
        public List<SnapDirectAgesAudienceValue> SnapDirectAgesAudienceValues {get;set;}

    }
}

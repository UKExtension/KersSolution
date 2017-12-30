using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class AffirmativeMakeupValue : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public AffirmativeActionPlanRevision Revision { get; set; }
        public int DiversityTypeId {get; set;}
        public AffirmativeMakeupDiversityType DiversityType {get; set;}
        public int GroupTypeId {get;set;}
        public AffirmativeAdvisoryGroupType GroupType {get;set;}
        public string Value {get;set;}
        
        
    }
}
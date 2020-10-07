using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class MileageSegment : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LocationId {get; set;}
        public int ProgramCategoryId {get;set;}
        public ProgramCategory ProgramCategory {get;set;}
        public ExtensionEventLocation Location {get; set;}
        public String BusinessPurpose {get; set;}
        public int FundingSource {get;set;}
        public ExpenseFundingSource FundingSource {get;set;}
        public float Mileage {get;set;}
        public int order {get;set;}
    }
}

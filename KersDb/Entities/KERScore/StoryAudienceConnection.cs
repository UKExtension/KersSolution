using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class StoryAudienceConnection : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public StoryAudienceType StoryAudienceType {get;set;}
        public int StoryAudienceTypeId {get;set;}
        public StoryRevision StoryRevision {get;set;}
        public int StoryRevisionId {get;set;}
    }
}
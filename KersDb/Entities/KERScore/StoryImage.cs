using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class StoryImage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StoryRevisionId {get;set;}
        public StoryRevision StoryRevision { get; set; }
        public int UploadImageId {get;set;}
        public UploadImage UploadImage {get;set;}
        public DateTime Created { get; set; }
    }
}
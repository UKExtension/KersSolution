using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class LadderImage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LadderApplicationId {get;set;}
        public LadderApplication LadderApplication { get; set; }
        public int UploadImageId {get;set;}
        public UploadImage UploadImage {get;set;}
        public DateTime Created { get; set; }
    }
}
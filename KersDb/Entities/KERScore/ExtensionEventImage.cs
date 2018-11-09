using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ExtensionEventImage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ExtensionEventId {get;set;}
        public ExtensionEvent ExtensionEvent { get; set; }
        public int UploadImageId {get;set;}
        public UploadImage UploadImage {get;set;}
        public DateTime Created { get; set; }
    }
}
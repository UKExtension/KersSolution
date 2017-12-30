using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class UploadFile : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Size {get;set;}
        public string Type {get;set;}
        public byte[] Content { get; set; }
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
        public KersUser By {get;set;}
    }
}
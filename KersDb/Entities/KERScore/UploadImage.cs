using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class UploadImage : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Width {get;set;}
        public int Height {get;set;}
        [Column(TypeName = "text")]
        public string ExIf {get;set;}
        public UploadFile UploadFile {get;set;}
    }
}
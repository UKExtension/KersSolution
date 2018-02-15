using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class TaskOperation : IEntityBase
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ClassName {get;set;}
        public string Description {get;set;}
        public string Arguments {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}
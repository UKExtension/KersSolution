using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class MessageTemplate : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code {get;set;}
        public string Subject {get; set;}
        public string BodyHtml {get;set;}
        public string BodyText {get;set;}
        public KersUser CreatedBy {get; set;}
        public KersUser UpdatedBy {get; set;}
        public DateTimeOffset Created {get; set;}
        public DateTimeOffset Updated {get; set;}
    }
}
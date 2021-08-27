using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Abstract;

namespace Kers.Models.Entities.KERScore
{
    public partial class ContactOptionNumberValue : IEntityBase, IOptionNumberValue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ActivityOptionNumberId {get;set;}
        public ActivityOptionNumber ActivityOptionNumber {get;set;}
        public int Value {get;set;}
        public ContactRevision ContactRevision {get;set;}
    }
}
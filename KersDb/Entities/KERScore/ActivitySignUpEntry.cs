using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class ActivitySignUpEntry : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name {get; set;}
        public int Gender {get;set;}
        public Race Race {get;set;}
        public int RaceId {get;set;}
        public Ethnicity Ethnicity {get;set;}
        public int EthnicityId {get;set;}
        public Activity Activity {get;set;}
        public int ActivityId {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}
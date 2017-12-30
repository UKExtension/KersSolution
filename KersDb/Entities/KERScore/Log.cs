using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    [Table("Log")]
    public partial class Log : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Level {get;set;}
        public DateTime Time {get; set;}
        public string Type {get; set;}
        [Column(TypeName = "text")]
        public string Description {get; set;}
        public string ObjectType {get; set; }
        
        [Column(TypeName = "text")]
        public string Object {get; set;}
        public string Ip {get; set;}
        public string Agent {get; set;}
        public KersUser User {get;set;}

    }
}

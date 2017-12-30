using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    [Table("Common_FiscalYear")]
    public partial class FiscalYear : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Start {get; set;}
        public DateTime End {get; set;}
        public DateTime AvailableAt {get; set;}
        public DateTime ExtendedTo {get;set;}
        public String Type {get;set;}
        public String Name {get; set;}
    }
}

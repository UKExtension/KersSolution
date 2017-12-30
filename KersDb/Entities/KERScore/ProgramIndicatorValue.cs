using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class ProgramIndicatorValue : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public int ProgramIndicatorId {get;set;}
        public ProgramIndicator ProgramIndicator {get;set;}
        public int Value {get; set;}

    }
}

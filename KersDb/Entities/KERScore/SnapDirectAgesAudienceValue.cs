using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapDirectAgesAudienceValue : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int SnapDirectAgesId { get; set; }
        public SnapDirectAges SnapDirectAges { get; set; }
        public int SnapDirectAudienceId {get;set;}
        public SnapDirectAudience SnapDirectAudience {get;set;}
        public int Value {get;set;}
    }
}

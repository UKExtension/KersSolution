using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapIndirectReachedValue : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int SnapIndirectReachedId {get;set;}
        public SnapIndirectReached SnapIndirectReached {get;set;}
        public int Value {get;set;}

    }
}

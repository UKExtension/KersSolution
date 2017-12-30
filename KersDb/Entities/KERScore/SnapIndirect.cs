using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapIndirect : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public List<SnapIndirectMethodSelection> SnapIndirectMethodSelections {get;set;}
        public List<SnapIndirectReachedValue> SnapIndirectReachedValues {get;set;}

    }
}

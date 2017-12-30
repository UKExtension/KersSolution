using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapIndirectMethodSelection : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int SnapIndirectMethodId {get;set;}
        public SnapIndirectMethod SnapIndirectMethod {get;set;}

    }
}

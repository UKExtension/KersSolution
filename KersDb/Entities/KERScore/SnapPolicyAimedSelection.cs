using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapPolicyAimedSelection : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int SnapPolicyAimedId {get;set;}
        public SnapPolicyAimed SnapPolicyAimed {get;set;}
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapPolicy : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "text")]
        public string Purpose {get;set;}
        [Column(TypeName = "text")]
        public string Result {get;set;}
        public List<SnapPolicyAimedSelection> SnapPolicyAimedSelections {get;set;}
        public List<SnapPolicyPartnerValue> SnapPolicyPartnerValue {get;set;}

    }
}

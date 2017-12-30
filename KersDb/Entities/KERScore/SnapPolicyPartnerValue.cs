using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapPolicyPartnerValue : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int SnapPolicyPartnerId {get;set;}
        public SnapPolicyPartner SnapPolicyPartner {get;set;}
        public int Value {get;set;}

    }
}

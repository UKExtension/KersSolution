using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class SocialConnection : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SocialConnectionTypeId {get; set;}
        public SocialConnectionType SocialConnectionType { get; set; }
        public PersonalProfile PersonalProfile { get; set; }
        public string Identifier {get;set;}
    }
}
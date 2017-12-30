using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class KersUserSpecialty : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SpecialtyId {get; set;}
        public Specialty Specialty{ get; set; }
        public int KersUserId {get; set;}
        //public KersUser KersUser { get; set; }
    }
}
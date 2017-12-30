using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ProfileTimeZone : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Retrieved as described here:
        // http://stackoverflow.com/questions/11580423/what-is-the-best-way-to-store-timezone-information-in-my-db
        public string TimeZoneId { get; set; }
        public PersonalProfile PersonalProfile {get; set;}
    }
}
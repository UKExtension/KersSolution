using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class PersonalProfile : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName {get; set;}
        public string LastName {get; set;}
        public string ProfessionalTitle {get;set;}
        public string OfficePhone {get; set;}
        public string MobilePhone {get; set;}
        // Retrieved as described here:
        // http://stackoverflow.com/questions/11580423/what-is-the-best-way-to-store-timezone-information-in-my-db
        public string TimeZoneId { get; set; }
        public string OfficeAddress {get;set;}
        [Column(TypeName="text")]
        public string Bio {get; set;}
        public List<InterestProfile> Interests {get;set;}
        public List<SocialConnection> SocialConnections {get;set;}
        public int? UploadImageId {get;set;}
        public UploadImage UploadImage {get;set;}
        public KersUser User;

    }
}

using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class KersUser : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int classicReportingProfileId {get; set;}
        public PersonalProfile PersonalProfile { get; set; }
        public ReportingProfile RprtngProfile {get; set;}
        public int ExtensionPositionId { get; set; }
        public ExtensionPosition ExtensionPosition{ get; set; }
        public List<zEmpProfileRole> Roles {get;set;}
        public List<KersUserSpecialty> Specialties {get; set;}
        public DateTime Created { get; set; }
        public DateTime Updated {get; set;}
        public DateTime LastLogin {get;set;}
        public List<ExtensionEvent> Events {get;set;}
        public List<Training> SubmittedTrainins {get;set;}
        public List<Training> ApprovedTrainings {get;set;}

    }
}

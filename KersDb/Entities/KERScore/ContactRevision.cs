using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ContactRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ContactId {get;set;}
        public DateTime Created {get;set;}
        public DateTime ContactDate {get;set;}
        public float Days{get;set;}
        public float Multistate {get;set;}
        public int MajorProgramId {get;set;}
        public MajorProgram MajorProgram {get;set;}
        public List<ContactRaceEthnicityValue> ContactRaceEthnicityValues {get;set;}
        public int Female {get;set;}
        public int Male {get;set;}
        public List<ContactOptionNumberValue> ContactOptionNumbers {get;set;}
    }
}
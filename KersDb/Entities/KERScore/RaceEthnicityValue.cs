using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Abstract;

namespace Kers.Models.Entities.KERScore
{

    public partial class RaceEthnicityValue : IEntityBase, IRaceEthnicityValue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RaceId {get;set;}
        public Race Race { get; set; }
        public int EthnicityId { get; set; }
        public Ethnicity Ethnicity {get;set;}
        public int Amount {get;set;}
    }
}
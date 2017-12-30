using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;


namespace Kers.Models.Abstract
{
    public interface IRaceEthnicityValue{
        int Id {get;set;}
        int RaceId {get;set;}
        Race Race {get;set;}
        int EthnicityId { get; set; }
        Ethnicity Ethnicity {get;set;}
        int Amount {get;set;}

    }
}
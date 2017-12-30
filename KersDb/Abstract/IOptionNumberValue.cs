using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;


namespace Kers.Models.Abstract
{
    public interface IOptionNumberValue{
        int Id {get;set;}
        int ActivityOptionNumberId {get;set;}
        ActivityOptionNumber ActivityOptionNumber {get;set;}
        int Value {get;set;}

    }
}
using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{
    public class SearchCriteriaViewModel{
        public int Skip {get;set;}
        public int Take {get;set;}
        public string OrderBy {get;set;}
        public string SearchString {get;set;}
    }
}
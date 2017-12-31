using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{

    public class TableViewModel
    {
        public List<string> Header {get;set;}
        public List<List<string>> Rows {get;set;}
        public List<String> Foother {get;set;}

    }
}
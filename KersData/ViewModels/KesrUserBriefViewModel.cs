using System;
using System.Collections.Generic;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.ViewModels
{
    public class KesrUserBriefViewModel{
        public int Id {get;set;}
        public string Name {get;set;}
        public string Position {get;set;}
        public string Title {get;set;}
        public string Image {get;set;}
        public string Phone {get;set;}
        public string PlanningUnitName {get;set;}
        public int PlanningUnitId {get;set;}
    }
}
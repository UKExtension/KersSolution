using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class AlertRoute:IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public String Name {get; set;}
        public String UrlRoute {get; set;}
        public bool Active {get;set;}
        
    }
}
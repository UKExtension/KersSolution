using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapDirectDeliverySiteCategory : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int order { get; set; }
        public string Name {get;set;}
        public List<SnapDirectDeliverySite> SnapDirectDeliverySites {get;set;}
        public Boolean Active {get;set;}
    }
}

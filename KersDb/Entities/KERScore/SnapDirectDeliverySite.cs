using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERScore
{

    public partial class SnapDirectDeliverySite : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public int FiscalYearId {get;set;}
        public FiscalYear FiscalYear{ get; set; }
        public int order { get; set; }
        public string Name {get;set;}
        public Boolean Active {get;set;}
    }
}

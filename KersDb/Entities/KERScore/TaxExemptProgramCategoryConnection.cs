using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
 
    public partial class TaxExemptProgramCategoryConnection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TaxExemptId {get;set;}
        public TaxExempt TaxExempt {get;set;}
        public int TaxExemptProgramCategoryId {get;set;}
        public TaxExemptProgramCategory TaxExemptProgramCategory {get;set;}
    } 
}
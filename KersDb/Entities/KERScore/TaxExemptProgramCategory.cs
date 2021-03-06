using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
 
    public partial class TaxExemptProgramCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TaxExemptId {get;set;}
        public TaxExempt TaxExempt {get;set;}
        public int ProgramCategoryId {get;set;}
        public ProgramCategory ProgramCategory {get;set;}
    } 
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
 
    public partial class CountyEventProgramCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CountyEventId {get;set;}
        public CountyEvent CountyEvent {get;set;}
        public int ProgramCategoryId {get;set;}
        public ProgramCategory ProgramCategory {get;set;}
    } 
}
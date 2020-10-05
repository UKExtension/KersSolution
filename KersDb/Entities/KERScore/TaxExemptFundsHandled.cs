using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
 
    public partial class TaxExemptFundsHandled
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name {get;set;}
        public int Order {get;set;}
        public bool Active {get;set;}
        public bool Is501 {get;set;}
    } 
}
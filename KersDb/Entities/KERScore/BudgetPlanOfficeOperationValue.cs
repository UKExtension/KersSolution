using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class BudgetPlanOfficeOperationValue : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public BudgetPlanOfficeOperation BudgetPlanOfficeOperation {get;set;}
        public float Value {get;set;}

    }
}
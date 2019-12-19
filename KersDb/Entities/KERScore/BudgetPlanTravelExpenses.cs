using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class BudgetPlanTravelExpenses : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KersUser Person {get;set;}
        public string PersonNameIfNotAUser {get;set;}
        public float Amount {get;set;}
        public BudgetPlanStaffType StaffType {get;set;}

    }
}
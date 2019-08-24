using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class BudgetPlanStaffExpenditure : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? PersonId {get;set;}
        public KersUser Person {get;set;}
        public string PersonNameIfNotAUser {get;set;}
        public float HourlyRate {get;set;}
        public float HoursPerWeek {get;set;}
        public float BenefitRateInPercents {get;set;}
        public int ExpenditureType {get;set;}
        public int index {get;set;}

    }
}
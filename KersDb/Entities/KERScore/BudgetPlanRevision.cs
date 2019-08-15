using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class BudgetPlanRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KersUser KersUser {get;set;}
        public int KersUserId {get;set;}
        public DateTime Created {get;set;}

        /****************************/
        // Anticipated Income
        /****************************/
        public float RealPropertyAssesment {get;set;}
        public float RealPropertyTaxRate {get;set;}
        public float PersonalPropertyAssesment {get;set;}
        public float PersonalPropertyTaxRate {get;set;}
        public float MotorVehicleAssesment {get;set;}
        public float MotorVehicleTaxRate {get;set;}
        public float AnticipatedDelinquency {get;set;}
        public float Collection {get;set;}
        public float OtherExtDistTaxes1 {get;set;}
        public float OtherExtDistTaxes2 {get;set;}
        public float CoGenFund {get; set;}
        public ICollection<BudgetPlanUserDefinedIncome> UserDefinedIncome {get;set;}
        public float Interest {get;set;}
        public float Reserve {get;set;}
        public float CapitalImpFund {get;set;}
        public float EquipmentFund {get;set;}
        public float AnticipatedCarryover {get; set;}
        /****************************/
        // Anticipated Expenditures
        /****************************/
        public ICollection<BudgetPlanStaffExpenditure> BudgetPlanStaffExpenditures {get;set;}
        public float BaseAgentContribution {get;set;}
        public ICollection<BudgetPlanTravelExpenses> TravelExpenses {get;set;}
        public ICollection<BudgetPlanProfessionalImprovementExpenses> ProfessionalImprovemetnExpenses {get;set;}
        public int NumberOfProfessionalStaff {get;set;}
        public float AmontyPerProfessionalStaff {get;set;}
        public float AdditionalOperationalCostPerPerson {get;set;}
        public float UkPostage {get;set;}
        public float UkPublications {get;set;}
        public float CapitalImprovementFundForEmergency {get;set;}
        public float EquipmentFundForEmergency {get;set;}
        public ICollection<BudgetPlanOfficeOperationValue> OfficeOperationValues {get;set;}

    }
}
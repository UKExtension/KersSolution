using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ExpenseRevision : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ExpenseId {get;set;}
        public DateTime Created {get;set;}
        public DateTime ExpenseDate {get;set;}
        public Boolean isOvernight {get;set;}
        public int ProgramCategoryId {get;set;}
        public ProgramCategory ProgramCategory {get;set;}
        public string BusinessPurpose {get;set;}
        public int? VehicleType {get;set;}
        public int? CountyVehicleId {get;set;}
        public CountyVehicle CountyVehicle {get;set;}
        [Column(TypeName = "text")]
        public string Comment {get;set;}
        // Workplace = 1, Home = 2
        public int StartingLocationType {get; set;}
        //public string StartingLocationOther {get;set;}
        public String ExpenseLocation {get;set;}
        public int? FundingSourceNonMileageId {get;set;}
        public ExpenseFundingSource FundingSourceNonMileage {get;set;}
        public int? FundingSourceMileageId {get;set;}
        public ExpenseFundingSource FundingSourceMileage {get;set;}
        public float Mileage {get;set;}
        public float Registration {get;set;}
        public float Lodging {get;set;}
        public int? MealRateBreakfastId {get;set;}
        public ExpenseMealRate MealRateBreakfast {get;set;}
        public float MealRateBreakfastAmount {get;set;}
        public float? MealRateBreakfastCustom {get;set;}
        public int? MealRateLunchId {get;set;}
        public ExpenseMealRate MealRateLunch {get;set;}
        public float? MealRateLunchAmount {get;set;}
        public float? MealRateLunchCustom {get;set;}
        public int? MealRateDinnerId {get;set;}
        public ExpenseMealRate MealRateDinner {get;set;}
        public float? MealRateDinnerAmount {get;set;}
        public float? MealRateDinnerCustom {get;set;}
        public float otherExpenseCost {get;set;}
        public string otherExpenseExplanation {get;set;}
        public DateTime? departTime {get;set;}
        public DateTime? returnTime {get;set;}
        public int StartingLocationId {get;set;}
        [ForeignKey("StartingLocationId")]
        public ExtensionEventLocation StartingLocation {get;set;}

        public List<MileageSegment> Segments {get;set;}

    }
}
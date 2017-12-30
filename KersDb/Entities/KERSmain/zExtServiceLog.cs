using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zExtServiceLog")]
    public partial class zExtServiceLog
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        public int? FY { get; set; }

        public int? FYservlog { get; set; }

        public int? FYsnap { get; set; }

        [StringLength(10)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(8)]
        public string personID { get; set; }

        public int? pacID1 { get; set; }

        [StringLength(8)]
        public string calDate { get; set; }

        public int? activityHours { get; set; }

        [StringLength(200)]
        public string activityTitle { get; set; }

        public string activityDescription { get; set; }

        public bool? notPresent { get; set; }

        public bool? night { get; set; }

        public bool? weekend { get; set; }

        public bool? notExtensionSponsored { get; set; }

        public bool? multiState { get; set; }

        public bool? MS4 { get; set; }

        public int? direct_W_Hn { get; set; }

        public int? direct_W_Hy { get; set; }

        public int? direct_B_Hn { get; set; }

        public int? direct_B_Hy { get; set; }

        public int? direct_A_Hn { get; set; }

        public int? direct_A_Hy { get; set; }

        public int? direct_I_Hn { get; set; }

        public int? direct_I_Hy { get; set; }

        public int? direct_H_Hn { get; set; }

        public int? direct_H_Hy { get; set; }

        public int? direct_O_Hn { get; set; }

        public int? direct_O_Hy { get; set; }

        public int? direct_X_Hn { get; set; }

        public int? direct_X_Hy { get; set; }

        public int? directGenderM { get; set; }

        public int? directGenderF { get; set; }

        public int? directAdultVolunteers { get; set; }

        public int? directYouth { get; set; }

        public int? indirectContacts { get; set; }

        public int? snapModeID { get; set; }

        [StringLength(8)]
        public string snapCopies { get; set; }

        public int? snapDirectDeliverySiteID { get; set; }

        [StringLength(75)]
        public string snapDirectSpecificSiteName { get; set; }

        public int? snapDirectSessionTypeID { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_FarmersMarket { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_FarmersMarket { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_FarmersMarket { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_FarmersMarket { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_PreSchool { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_PreSchool { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_PreSchool { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_PreSchool { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_Family { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_Family { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_Family { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_Family { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_SchoolAge { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_SchoolAge { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_SchoolAge { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_SchoolAge { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_LimitedEnglish { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_LimitedEnglish { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_LimitedEnglish { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_LimitedEnglish { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_00_04_Seniors { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_05_17_Seniors { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_18_59_Seniors { get; set; }

        [StringLength(8)]
        public string snapDirectAudience_60_pl_Seniors { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedPsaRadio { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedPsaTv { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedArticles { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedGroceryStore { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedFairsParticipated { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedFairsSponsored { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedNewsletter { get; set; }

        [StringLength(8)]
        public string snapIndirectEstNumbReachedOther { get; set; }

        public bool? snapIndirectMethodFactSheets { get; set; }

        public bool? snapIndirectMethodPosters { get; set; }

        public bool? snapIndirectMethodCalendars { get; set; }

        public bool? snapIndirectMethodPromoMaterial { get; set; }

        public bool? snapIndirectMethodWebsite { get; set; }

        public bool? snapIndirectMethodEmail { get; set; }

        public bool? snapIndirectMethodVideo { get; set; }

        public bool? snapIndirectMethodOther { get; set; }

        public int? snapCommunityGroupFocusID { get; set; }

        public bool? snapCommunityAimImprovementInAgriculture { get; set; }

        public bool? snapCommunityAimImprovementInCommunityDesign { get; set; }

        public bool? snapCommunityAimImprovementInCommunitySafety { get; set; }

        public bool? snapCommunityAimImprovementInEarlyChildhood { get; set; }

        public bool? snapCommunityAimImprovementInFoodInsecurity { get; set; }

        public bool? snapCommunityAimImprovementInFoodRetail { get; set; }

        public bool? snapCommunityAimImprovementInFoodService { get; set; }

        public bool? snapCommunityAimImprovementInHomes { get; set; }

        public bool? snapCommunityAimImprovementInParksAndRecAreas { get; set; }

        public bool? snapCommunityAimImprovementInSchoolsAndAfterschoolProg { get; set; }

        public bool? snapCommunityAimImprovementInWorkplaces { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersBoysGirlsClub { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersChamberOfCommerce { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersCivicOrgsGroups { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersIndustry { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersVolunteers { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersCntyCityGovernment { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersCourts { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersDaycares { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersEducation { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersExtensionProgramCouncil { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersFaithBased { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersFamilyResourceCenters { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersFarmers { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersFoodPantries { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersFosterFamilyProgram { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersGroceryStores { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersHospitalsClinics { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersHousingAuthority { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersLocalBusiness { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersMentalHeathRehab { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersMilitary { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersParksRecreation { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersPublicHealth { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersPublicLibrary { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersStateAndFederalGovernment { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersUtilityCompany { get; set; }

        [StringLength(8)]
        public string snapCommunityPartnersYMCA { get; set; }

        public string snapCommunityOverallPurposeGoal { get; set; }

        public string snapCommunityResultImpact { get; set; }

        public string expenseLocation { get; set; }

        public int? expenseFundingSourceNonMileage { get; set; }

        public int? expenseFundingSourceMileage { get; set; }

        public int? expenseMileage { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseRegistrationCost { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseLodgingCost { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseBreakfastRate { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseBreakfastRateCustom { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseLunchRate { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseLunchRateCustom { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseDinnerRate { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseDinnerRateCustom { get; set; }

        [Column(TypeName = "money")]
        public decimal? expenseOtherExpenseCost { get; set; }

        [StringLength(90)]
        public string expenseOtherExpenseExplanation { get; set; }

        [StringLength(50)]
        public string expenseDepartTime { get; set; }

        [StringLength(50)]
        public string expenseReturnTime { get; set; }
    }
}

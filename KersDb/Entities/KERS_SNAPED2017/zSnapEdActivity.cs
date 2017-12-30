using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kers.Models.Entities.KERS_SNAPED2017
{

    [Table("zSnapEdActivity")]
    public partial class zSnapEdActivity
    {
        [Key]
        public int rID { get; set; }

        public DateTime? rDT { get; set; }

        public int? FY { get; set; }

        [StringLength(10)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }

        [StringLength(250)]
        public string planningUnitName { get; set; }

        [StringLength(8)]
        public string personID { get; set; }

        [StringLength(200)]
        public string personName { get; set; }

        public int? snapModeID { get; set; }

        [StringLength(8)]
        public string snapDate { get; set; }

        [StringLength(8)]
        public string snapHours { get; set; }

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
        public string snapDirectGenderMale { get; set; }

        [StringLength(8)]
        public string snapDirectGenderFemale { get; set; }

        [StringLength(8)]
        public string snapDirectRaceWhiteNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceWhiteHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceBlackNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceBlackHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceAsianNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceAsianHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceAmericanIndianNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceAmericanIndianHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceHawaiianNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceHawaiianHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceOtherNonHispanic { get; set; }

        [StringLength(8)]
        public string snapDirectRaceOtherHispanic { get; set; }

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
        public string snapIndirectEstNumbReachedSocialMedia  { get; set; }

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
    }
}



/*
rID	rDT	FY	instID	planningUnitID	planningUnitName	personID	personName	snapModeID	snapDate	snapHours	snapCopies	snapDirectDeliverySiteID	snapDirectSpecificSiteName	snapDirectSessionTypeID	snapDirectAudience_00_04_FarmersMarket	snapDirectAudience_05_17_FarmersMarket	snapDirectAudience_18_59_FarmersMarket	snapDirectAudience_60_pl_FarmersMarket	snapDirectAudience_00_04_PreSchool	snapDirectAudience_05_17_PreSchool	snapDirectAudience_18_59_PreSchool	snapDirectAudience_60_pl_PreSchool	snapDirectAudience_00_04_Family	snapDirectAudience_05_17_Family	snapDirectAudience_18_59_Family	snapDirectAudience_60_pl_Family	snapDirectAudience_00_04_SchoolAge	snapDirectAudience_05_17_SchoolAge	snapDirectAudience_18_59_SchoolAge	snapDirectAudience_60_pl_SchoolAge	snapDirectAudience_00_04_LimitedEnglish	snapDirectAudience_05_17_LimitedEnglish	snapDirectAudience_18_59_LimitedEnglish	snapDirectAudience_60_pl_LimitedEnglish	snapDirectAudience_00_04_Seniors	snapDirectAudience_05_17_Seniors	snapDirectAudience_18_59_Seniors	snapDirectAudience_60_pl_Seniors	snapDirectGenderMale	snapDirectGenderFemale	snapDirectRaceWhiteNonHispanic	snapDirectRaceWhiteHispanic	snapDirectRaceBlackNonHispanic	snapDirectRaceBlackHispanic	snapDirectRaceAsianNonHispanic	snapDirectRaceAsianHispanic	snapDirectRaceAmericanIndianNonHispanic	snapDirectRaceAmericanIndianHispanic	snapDirectRaceHawaiianNonHispanic	snapDirectRaceHawaiianHispanic	snapDirectRaceOtherNonHispanic	snapDirectRaceOtherHispanic	snapIndirectEstNumbReachedPsaRadio	snapIndirectEstNumbReachedPsaTv	snapIndirectEstNumbReachedArticles	snapIndirectEstNumbReachedGroceryStore	snapIndirectEstNumbReachedFairsParticipated	snapIndirectEstNumbReachedFairsSponsored	snapIndirectEstNumbReachedNewsletter	snapIndirectEstNumbReachedSocialMedia	snapIndirectEstNumbReachedOther	snapIndirectMethodFactSheets	snapIndirectMethodPosters	snapIndirectMethodCalendars	snapIndirectMethodPromoMaterial	snapIndirectMethodWebsite	snapIndirectMethodEmail	snapIndirectMethodVideo	snapIndirectMethodOther	snapCommunityGroupFocusID	snapCommunityAimImprovementInAgriculture	snapCommunityAimImprovementInCommunityDesign	snapCommunityAimImprovementInCommunitySafety	snapCommunityAimImprovementInEarlyChildhood	snapCommunityAimImprovementInFoodInsecurity	snapCommunityAimImprovementInFoodRetail	snapCommunityAimImprovementInFoodService	snapCommunityAimImprovementInHomes	snapCommunityAimImprovementInParksAndRecAreas	snapCommunityAimImprovementInSchoolsAndAfterschoolProg	snapCommunityAimImprovementInWorkplaces	snapCommunityPartnersBoysGirlsClub	snapCommunityPartnersChamberOfCommerce	snapCommunityPartnersCivicOrgsGroups	snapCommunityPartnersIndustry	snapCommunityPartnersVolunteers	snapCommunityPartnersCntyCityGovernment	snapCommunityPartnersCourts	snapCommunityPartnersDaycares	snapCommunityPartnersEducation	snapCommunityPartnersExtensionProgramCouncil	snapCommunityPartnersFaithBased	snapCommunityPartnersFamilyResourceCenters	snapCommunityPartnersFarmers	snapCommunityPartnersFoodPantries	snapCommunityPartnersFosterFamilyProgram	snapCommunityPartnersGroceryStores	snapCommunityPartnersHospitalsClinics	snapCommunityPartnersHousingAuthority	snapCommunityPartnersLocalBusiness	snapCommunityPartnersMentalHeathRehab	snapCommunityPartnersMilitary	snapCommunityPartnersParksRecreation	snapCommunityPartnersPublicHealth	snapCommunityPartnersPublicLibrary	snapCommunityPartnersStateAndFederalGovernment	snapCommunityPartnersUtilityCompany	snapCommunityPartnersYMCA	snapCommunityOverallPurposeGoal	snapCommunityResultImpact
100000001	2016-10-12 08:18:36.027	2017	21000-1862	21073	Franklin County CES	12092108	Rogers, Courtney J	1	20161003	3.0	0	1015	Second Street School	100003	0	0	0	0	0	0	0	0	0	0	0	0	0	11	0	0	0	0	0	0	0	0	0	0	3	8	0	0	0	0	0	0	0	0	0	0	11	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000002	2016-10-12 10:00:02.570	2017	21000-1862	21101	Henderson County CES	12062086	Rollins, Rohdene R	1	20161004	1.5	78	1006	Christian Outreach Food Pantry Group 3	100002	0	0	0	0	0	0	0	0	0	0	1	1	0	0	0	0	0	0	0	0	0	0	0	1	1	2	3	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000003	2016-10-12 10:02:47.710	2017	21000-1862	21101	Henderson County CES	12062086	Rollins, Rohdene R	1	20161004	1.0	14	1010	Riverview School 2016	100002	0	0	0	0	14	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	9	5	11	0	2	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000004	2016-10-12 10:04:57.210	2017	21000-1862	21101	Henderson County CES	12062086	Rollins, Rohdene R	1	20161005	1.0	0	1017	Women's Shelter 2016	100003	0	0	0	0	2	0	0	0	0	0	3	0	0	0	0	0	0	0	0	0	0	0	0	0	1	4	1	0	4	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000005	2016-10-12 10:06:44.380	2017	21000-1862	21101	Henderson County CES	12062086	Rollins, Rohdene R	2	20161007	2.5	200	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	200	0	0	NULL	0	1	0	0	1	0	0	0	1	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000006	2016-10-12 10:13:09.350	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161004	1.5	0	1002	Andrea's Mission for Women	100003	0	0	0	0	0	0	0	0	0	0	6	0	0	0	0	0	0	0	0	0	0	0	0	0	0	6	6	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000007	2016-10-12 10:16:27.027	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161007	1.5	0	1002	Andrea's Mission for Men	100003	0	0	0	0	0	0	0	0	0	0	8	0	0	0	0	0	0	0	0	0	0	0	0	0	8	0	8	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000008	2016-10-12 10:15:59.107	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161007	1.0	0	1001	LifeSkills	100003	0	0	0	0	0	0	0	0	0	0	5	0	0	0	0	0	0	0	0	0	0	0	0	0	3	2	5	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000010	2016-10-12 10:38:30.100	2017	21000-1862	21051	Clay County CES	10716126	Sebastian, Alissa R	1	20161006	2.5	40	1015	Professor Popcorn	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	40	0	0	0	0	0	0	0	0	0	0	18	22	39	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000012	2016-10-12 10:39:55.270	2017	21000-1862	21051	Clay County CES	10716126	Sebastian, Alissa R	1	20161010	3.5	40	1015	Big Creek Elementary 	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	40	0	0	0	0	0	0	0	0	0	0	23	17	38	0	1	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000015	2016-10-12 10:42:26.110	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161010	1.0	0	1010	Butler County Head Start	100003	0	0	0	0	0	0	17	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	8	9	10	7	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000016	2016-10-12 10:44:27.520	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161003	2.0	0	1007	McCreary County Extension Office - Jr. Cooking Club	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	6	0	0	0	0	0	0	0	0	0	0	0	6	6	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000019	2016-10-12 10:50:34.663	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161003	1.0	0	1015	colony	100003	0	0	0	0	18	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	8	10	18	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000020	2016-10-12 10:51:09.070	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161004	1.0	0	1015	Pine Knot Intermediate - 4th	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	21	1	0	0	0	0	0	0	0	0	0	13	9	22	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000021	2016-10-12 10:51:27.800	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161003	1.0	0	1015	camp ground	100003	0	0	0	0	19	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	7	12	18	0	0	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000022	2016-10-12 10:51:53.407	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161004	1.0	0	1015	Pine Knot Intermediate - 5th	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	20	1	0	0	0	0	0	0	0	0	0	11	10	21	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000023	2016-10-12 10:51:59.443	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161003	2.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	74	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000024	2016-10-12 10:52:35.793	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161004	1.0	0	1015	Pine Knot Intermediate - 6th	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	20	1	0	0	0	0	0	0	0	0	0	12	9	21	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000025	2016-10-12 10:58:20.353	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161004	2.0	0	1015	bush	100003	0	0	0	0	29	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	12	17	29	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000026	2016-10-12 10:54:02.810	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161004	1.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	58	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000027	2016-10-12 10:54:20.613	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	3	20161005	7.5	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000028	2016-10-12 10:55:12.380	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161006	1.0	0	1015	johnson	100003	0	0	0	0	12	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	7	5	12	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000029	2016-10-12 10:55:27.593	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161006	1.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	24	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000030	2016-10-12 10:56:27.987	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161006	3.0	0	1008	london laurel farmers market	100001	0	0	0	0	0	0	0	0	0	0	75	0	0	0	0	0	0	0	0	0	0	0	0	0	25	50	75	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000031	2016-10-12 10:58:02.677	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161010	2.0	0	1015	hunter hills	100003	0	0	0	0	36	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	20	16	34	0	1	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000009	2016-10-12 10:31:34.630	2017	21000-1862	21185	Oldham County CES	10154274	Logsdon, Amy M	3	20161012	1.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000013	2016-10-12 10:40:19.340	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161010	1.0	0	1010	Butler County Head Start	100003	0	0	0	0	0	0	17	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	12	5	10	7	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000014	2016-10-12 10:41:08.000	2017	21000-1862	21051	Clay County CES	10716126	Sebastian, Alissa R	1	20161011	4.5	60	1015	Manchester Elementary	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	60	0	0	0	0	0	0	0	0	0	0	25	35	57	1	2	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000017	2016-10-12 10:44:53.600	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161011	1.5	0	1002	Andrea's Mission for Women	100003	0	0	0	0	0	0	0	0	0	0	4	0	0	0	0	0	0	0	0	0	0	0	0	0	0	4	4	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000018	2016-10-12 10:45:50.463	2017	21000-1862	21031	Butler County CES	12118092	Moore, Michele G	1	20161011	1.0	0	1013	Bear's Den After School	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	6	0	0	0	0	0	0	0	0	0	0	1	5	6	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000035	2016-10-12 11:00:22.337	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161010	2.0	0	1012	McCreary County Public Library	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	11	6	0	0	0	0	0	0	0	0	0	3	14	17	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000043	2016-10-12 11:41:51.960	2017	21000-1862	21223	Trimble County CES	12184204	Perkins, Kevin R	1	20161011	3.0	0	1007	Cattlemen's Meeting in Trimble County	100001	0	0	0	0	0	0	0	0	0	0	52	0	0	12	0	0	0	0	0	0	0	0	0	3	45	22	67	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000047	2016-10-12 12:15:14.743	2017	21000-1862	21177	Muhlenberg County CES	12061851	Wood, Viola F	1	20161004	4.0	0	1015	Central City Elementary	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	424	0	0	0	0	0	0	0	0	0	0	226	198	362	18	14	0	8	0	0	0	2	0	20	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000048	2016-10-12 12:18:13.897	2017	21000-1862	21177	Muhlenberg County CES	12061851	Wood, Viola F	1	20161004	3.0	0	1014	Greenville Housing Authority	100003	0	0	0	0	0	0	0	0	0	0	2	2	0	0	0	0	0	0	0	0	0	0	0	0	0	4	3	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000050	2016-10-12 12:24:57.870	2017	21000-1862	21177	Muhlenberg County CES	12061851	Wood, Viola F	1	20161006	4.0	0	1015	Longest Elementary School	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	120	0	0	0	0	0	0	0	0	0	0	60	60	120	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000051	2016-10-12 12:26:08.210	2017	21000-1862	21177	Muhlenberg County CES	12061851	Wood, Viola F	1	20161006	2.0	0	1007	Muhlenberg County Extension Office	100001	0	0	0	0	0	0	0	0	0	0	0	0	0	8	0	0	0	0	0	0	0	0	0	0	0	8	7	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000052	2016-10-12 12:27:03.767	2017	21000-1862	21177	Muhlenberg County CES	12061851	Wood, Viola F	1	20161006	2.0	0	1007	Muhlenberg County Extension Office	100002	0	0	0	0	0	0	0	0	0	0	0	0	0	32	0	0	0	0	0	0	0	0	0	0	12	20	32	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000058	2016-10-12 12:56:23.353	2017	21000-1862	21065	Estill County CES	00003403	Baker, Eric L	1	20161011	4.0	0	1008	Estill County Extension grounds	100001	0	0	7	30	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	5	32	37	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000070	2016-10-12 15:29:11.210	2017	21000-1862	21089	Greenup County CES	12061789	King, Morgan B	1	20161011	4.0	0	1015	Argillite Elementary	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	49	0	0	0	0	0	0	0	0	0	0	23	26	48	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000071	2016-10-12 15:29:47.193	2017	21000-1862	21089	Greenup County CES	12061789	King, Morgan B	1	20161012	1.0	0	1015	Argillite Elementary	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	23	0	0	0	0	0	0	0	0	0	0	5	18	23	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000085	2016-10-12 16:11:44.760	2017	21000-1862	21193	Perry County CES	10898447	Fugate, Reda E	1	20161006	3.0	0	1012	perry County Public Library	100001	0	0	0	0	0	0	0	0	0	0	73	0	0	2	0	0	0	0	0	0	0	0	0	0	27	48	69	0	2	0	0	0	0	0	0	0	4	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000086	2016-10-12 16:12:39.210	2017	21000-1862	21193	Perry County CES	10898447	Fugate, Reda E	1	20161006	1.0	0	1015	Robinson Elm. 8th grade farm 2 school mr. slagill	100003	0	0	0	0	0	0	0	0	0	0	2	0	0	33	0	0	0	0	0	0	0	0	0	0	17	18	35	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000087	2016-10-12 16:13:57.553	2017	21000-1862	21193	Perry County CES	10898447	Fugate, Reda E	1	20161007	2.0	0	1001	Altenatal outlook	100003	0	0	0	0	0	0	0	0	0	0	19	0	0	0	0	0	0	0	0	0	0	0	0	0	6	13	19	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000088	2016-10-12 16:46:58.600	2017	21000-1862	21075	Fulton County CES	10710294	Wiseman, Ashley R	1	20161011	1.0	0	1004	Fulton Housing Authority Community Center on Carr St. 	100004	0	0	0	0	0	0	0	0	2	1	1	0	0	0	0	0	0	0	0	0	0	0	0	0	2	2	3	0	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000089	2016-10-12 16:49:46.133	2017	21000-1862	21075	Fulton County CES	10710294	Wiseman, Ashley R	1	20161012	1.0	0	1014	Hickman Housing Authority Activity Center	100004	0	0	0	0	0	0	0	0	0	0	2	4	0	0	0	0	0	0	0	0	0	0	0	0	3	3	4	0	2	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000090	2016-10-12 16:52:02.037	2017	21000-1862	21075	Fulton County CES	10710294	Wiseman, Ashley R	1	20161004	1.0	0	1004	Fulton Housing Authority Community Center on Carr St.	100004	0	0	0	0	0	0	0	0	3	1	1	0	0	0	0	0	0	0	0	0	0	0	0	0	4	1	5	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000091	2016-10-12 17:02:42.247	2017	21000-1862	21075	Fulton County CES	10710294	Wiseman, Ashley R	1	20161005	1.0	40	1015	Hickman Housing Authority Activity Center	100004	0	0	0	0	0	0	0	0	2	10	3	1	0	0	0	0	0	0	0	0	0	0	0	0	6	10	3	3	9	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000092	2016-10-12 16:57:38.977	2017	21000-1862	21075	Fulton County CES	10710294	Wiseman, Ashley R	1	20161010	.5	0	1005	Hickman Senior Citizens Center	100003	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	12	2	10	11	0	1	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000093	2016-10-12 17:18:19.147	2017	21000-1862	21219	Todd County CES	12091803	Stooksbury, Amy P	1	20161004	2.0	0	1007	Todd County Ext Office	100004	0	0	0	0	0	0	0	0	0	0	6	0	0	1	0	0	0	0	0	0	0	0	0	3	1	9	2	0	8	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000032	2016-10-12 10:58:43.770	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161010	2.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	74	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000033	2016-10-12 10:59:56.913	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161011	2.0	0	1015	wyan pine	100003	0	0	0	0	39	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	20	19	37	0	1	0	0	0	0	0	0	0	0	1	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000034	2016-10-12 11:00:13.967	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161011	2.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	78	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000036	2016-10-12 11:00:59.147	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161012	1.0	0	1015	hazel green	100003	0	0	0	0	16	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	7	9	16	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000037	2016-10-12 11:01:14.933	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	2	20161012	1.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	32	1	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000038	2016-10-12 11:02:24.210	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	1	20161013	3.0	0	1008	london laurel farmers market	100001	0	0	0	0	0	0	0	0	0	0	82	0	0	0	0	0	0	0	0	0	0	0	0	0	39	43	82	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000039	2016-10-12 11:02:44.333	2017	21000-1862	21125	Laurel County CES	10915129	House, Mary M	3	20161014	3.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000040	2016-10-12 11:07:31.993	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	1	20161011	1.0	0	1004	McCreary County Senior Citizens	100004	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	1	13	7	7	14	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL
100000041	2016-10-12 11:08:00.247	2017	21000-1862	21147	McCreary County CES	10734246	Ridener, Donna K	2	20161011	1.0	0	NULL	NULL	NULL	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	NULL	20	0	0	0	1	0	0	0	0	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL





 */
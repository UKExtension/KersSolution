namespace Kers.Models.Repositories
{
    public static class CacheKeys{
        /***************************
        //Activity
         ***************************/
        
        
        public static string ActivityLastRevisionIdsPerFiscalYear = "ActivityLastRevisionIdsPerFiscalYear";
        public static string ActivityLastRevisionIdsPerPeriod = "ActivityLastRevisionIdsPerPeriod";
        public static string ContactsLastRevisionIdsPerPeriod = "ContactsLastRevisionIdsPerPeriod";
        public static string ActivityLastRevision = "ActivityLastRevision";
        public static string ContactLastRevision = "ContactLastRevision";
        public static string LastStoryRevisionIds = "LastStoryRevisionIds";
        public static string LastStoryRevisions = "LastStoryRevisions";
        public static string LastStories = "LastStories";
        public static string LastStoriesByUser = "LastStoriesByUser";
        public static string LastStoryWithImages = "LastStoryWithImages";
        public static string StatsPerMonth = "StatsPerMonth";
        public static string ActivitiesPerFyPerUserPerMajorProgram = "ActivitiesPerFyPerUserPerMajorProgram";
        public static string FilteredContactSummaries = "ContactSummariesPerGroup";
        public static string TopProgramsPerFiscalYear = "TopProgramsPerFiscalYear";
        public static string ReportsDataByMonth = "ReportsDataByMonth";
        public static string ActivitiesPerPeriod = "ActivitiesPerPeriod";

        //TableViewModel
        public static string ActivityContactsByCountyByMajorProgram = "ActivityContactsByCountyByMajorProgram";
        public static string StateAllContactsData = "StateAllContactsData";
        public static string StateByMajorProgram = "ActivityStateByMajorProgram";
        public static string ByEmployeeContactsData = "ByEmployeeContactsData";
        public static string ByMajorProgramContactData = "ByMajorProgramContactData";

        //List<ActivityUnitResult>
        public static string AllActivitiesByPlanningUnit = "AllActivitiesByPlanningUnit";

        //List<ContactUnitResult>
        public static string AllContactsByPlanningUnit = "AllContactsByPlanningUnit";

        //List<PlanningUnit>
        public static string CountiesList = "CountiesList";

        //List<StoryViewModel>
        public static string LastStoriesWithoutImages = "LastStoriesWithoutImages";

        public static string LastStoriesWithImages = "LastStoriesWithImages";

        /***************************
        //Snap Ed
        ***************************/
        public static string SnapData = "SnapData";
        public static string SnapEdTotalByMonth = "SnapEdTotalByMonth";
        public static string SnapEdTotalByCounty = "SnapEdTotalByCounty";
        public static string SnapEdTotalByEmployee = "SnapEdTotalByEmployee";
        public static string UserRevisionWithSnapData = "UserRevisionWithSnapData";
        public static string SitesPerPersonPerMonth = "SitesPerPersonPerMonth";
        public static string PersonalHourDetails = "PersonalHourDetails";
        public static string AimedTowardsImprovement = "AimedTowardsImprovement";
        public static string SnapPartnerCategory = "SnapPartnerCategory";
        public static string SnapAgentCommunityEventDetail = "SnapAgentCommunityEventDetail";
        public static string CopiesSummarybyCountyAgents = "CopiesSummarybyCountyAgents";
        public static string CopiesSummarybyCountyNotAgents = "CopiesSummarybyCountyNotAgents";
        public static string CopiesDetailAgents = "CopiesDetailAgents";
        public static string CopiesDetailANotAgents = "CopiesDetailANotAgents";
        public static string ReimbursementNepAssistants = "ReimbursementNepAssistants";
        public static string SnapReimbursementCounty = "SnapReimbursementCounty";
        public static string SnapSpecificSiteNamesByMonth = "SnapSpecificSiteNamesByMonth";
        public static string NumberofDeliverySitesbyTypeofSetting = "NumberofDeliverySitesbyTypeofSetting";
        public static string SnapMethodsUsedRecordCount = "MethodsUsedRecordCount";
        public static string SnapIndividualContactTotals = "SnapIndividualContactTotals";
        public static string SnapEstimatedSizeofAudiencesReached = "SnapEstimatedSizeofAudiencesReached";
        public static string SnapSessionTypebyMonth = "SnapSessionTypebyMonth";
        public static string SnapCommitmentSummary = "SnapCommitmentSummary";
        public static string SnapCommitmentHoursDetail = "SnapCommitmentHoursDetail";
        public static string SnapAgentsWithoutCommitment = "SnapAgentsWithoutCommitment";
        public static string SnapCommitmentSummaryByPlanningUnit = "SnapCommitmentSummaryByPlanningUnit";
        public static string SnapCommitmentReinforcementItems = "SnapCommitmentReinforcementItems";
        public static string SnapCommitmentReinforcementItemsPerCounty = "SnapCommitmentReinforcementItemsPerCounty";
        public static string SnapCommitmentSuggestedIncentiveItems = "SnapCommitmentSuggestedIncentiveItems";
        public static string IndirectByEmployee = "IndirectByEmployee";
        public static string PartnersOfACounty = "PartnersOfACounty";
        public static string AudienceAgeCategory = "AudienceAgeCategory";

    }
}
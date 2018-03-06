namespace Kers.Models.Repositories
{
    public static class CacheKeys{
        /***************************
        //Activity
         ***************************/
        public static string ActivityLastRevisionIdsPerFiscalYear = "ActivityLastRevisionIdsPerFiscalYear";
        public static string StateAllContactsData = "StateAllContactsData";
        //List<ActivityUnitResult>
        public static string AllActivitiesByPlanningUnit = "AllActivitiesByPlanningUnit";
        //List<ContactUnitResult>
        public static string AllContactsByPlanningUnit = "AllContactsByPlanningUnit";

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

    }
}
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace Kers.Models.Contexts
{

    public class KERScoreContext : DbContext
    {
        
        public KERScoreContext(DbContextOptions<KERScoreContext> options) : base(options)
        {
        
        }

        /***************************************/
        //  User Profile
        /***************************************/
        public virtual DbSet<KersUser> KersUser {get; set;}
        public virtual DbSet<PersonalProfile> PersonalProfile {get;set;}
        public virtual DbSet<ReportingProfile> ReportingProfile {get;set;}
        public virtual DbSet<ExtensionPosition> ExtensionPosition {get;set;}
        public virtual DbSet<Specialty> Specialty {get;set;}
        public virtual DbSet<KersUserSpecialty> KersUserSpecialty {get;set;}
        public virtual DbSet<zEmpProfileRole> zEmpProfileRole {get;set;}
        public virtual DbSet<Interest> Interest {get;set;}
        public virtual DbSet<InterestProfile> InterestProfile {get;set;}
        public virtual DbSet<PersonalEmail> PersonalEmail {get;set;}
        public virtual DbSet<ProfileTimeZone> ProfileTimeZone {get; set;}
        public virtual DbSet<SocialConnection> SocialConnection {get; set;}
        public virtual DbSet<SocialConnectionType> SocialConnectionType {get; set;} 
        /***************************************/
        //  Roles
        /***************************************/
        public virtual DbSet<zEmpRoleType> zEmpRptRoleType { get; set; }
        public virtual DbSet<zEmpProfileRole> zEmpProfileRoles {get; set;}
        /***************************************/
        //  Navigation
        /***************************************/
        public virtual DbSet<NavGroup> NavGroup {get;set;}
        public virtual DbSet<NavSection> NavSection {get;set;}
        public virtual DbSet<NavItem> NavItem {get;set;}
        /***************************************/
        //  Strategic Initiatives and Major Programs
        /***************************************/
        public virtual DbSet<StrategicInitiative> StrategicInitiative {get;set;}
        public virtual DbSet<MajorProgram> MajorProgram {get;set;}
        public virtual DbSet<ProgramCategory> ProgramCategory {get; set;}
        /***************************************/
        //  Locations
        /***************************************/
        public virtual DbSet<District> District {get; set;}
        public virtual DbSet<Region> Region {get; set;}
        public virtual DbSet<PlanningUnit> PlanningUnit {get; set;}
        public virtual DbSet<GeneralLocation> GeneralLocation {get;set;}
        public virtual DbSet<Institution> Institution {get;set;}
        /***************************************/
        //  Plans of Work
        /***************************************/
        public virtual DbSet<PlanOfWork> PlanOfWork {get; set;}
        public virtual DbSet<PlanOfWorkRevision> PlanOfWorkRevision {get; set;}
        public virtual DbSet<Map> Map {get; set;}
        /***************************************/
        //  Help Content
        /***************************************/
        public virtual DbSet<HelpCategory> HelpCategory {get; set;}
        public virtual DbSet<HelpContent> HelpContent {get; set;}
        /***************************************/
        //  Affirmative Action Plan
        /***************************************/
        public virtual DbSet<AffirmativeAdvisoryGroupType> AffirmativeAdvisoryGroupType {get;set;} 
        public virtual DbSet<AffirmativeActionPlan> AffirmativeActionPlan {get;set;}
        public virtual DbSet<AffirmativeActionPlanRevision> AffirmativeActionPlanRevision {get;set;}
        public virtual DbSet<AffirmativeMakeupDiversityType> AffirmativeMakeupDiversityType {get;set;}
        public virtual DbSet<AffirmativeMakeupDiversityTypeGroup> AffirmativeMakeupDiversityTypeGroup {get;set;}
        public virtual DbSet<AffirmativeMakeupValue> AffirmativeMakeupValue {get;set;}
        public virtual DbSet<AffirmativeSummaryDiversityType> AffirmativeSummaryDiversityType {get;set;}
        public virtual DbSet<AffirmativeSummaryValue> AffirmativeSummaryValue {get;set;}
        /***************************************/
        //  Uploads
        /***************************************/
        public virtual DbSet<UploadFile> UploadFile {get;set;}
        public virtual DbSet<UploadImage> UploadImage {get;set;}

        /***************************************/
        //  Program Indicators
        /***************************************/
        public virtual DbSet<ProgramIndicator> ProgramIndicator {get;set;}
        public virtual DbSet<ProgramIndicatorValue> ProgramIndicatorValue {get;set;}
        /***************************************/
        //  Expoenses
        /***************************************/
        public virtual DbSet<Expense> Expense {get;set;}
        public virtual DbSet<ExpenseRevision> ExpenseRevision {get;set;}
        public virtual DbSet<ExpenseFundingSource> ExpenseFundingSource {get;set;}
        public virtual DbSet<ExpenseMealRate> ExpenseMealRate {get;set;}
        public virtual DbSet<ExpenseMileageRate> ExpenseMileageRate {get;set;}
        
        /***************************************/
        //  Service Log
        /***************************************/
        public virtual DbSet<Ethnicity> Ethnicity {get;set;}
        public virtual DbSet<Race> Race {get;set;}
        public virtual DbSet<RaceEthnicityValue> RaceEthnicityValue {get;set;}
        public virtual DbSet<ActivityOption> ActivityOption {get;set;}
        public virtual DbSet<ActivityOptionSelection> ActivityOptionSelection {get;set;}
        public virtual DbSet<ActivityOptionNumber> ActivityOptionNumber {get;set;}
        public virtual DbSet<ActivityOptionNumberValue> ActivityOptionNumberValue {get;set;}
        public virtual DbSet<Activity> Activity {get;set;}
        public virtual DbSet<ActivityRevision> ActivityRevision {get;set;}

        /***************************************/
        //  Snap Ed
        /***************************************/
        public virtual DbSet<SnapDirect> SnapDirect {get;set;}
        public virtual DbSet<SnapIndirect> SnapIndirect {get;set;}
        public virtual DbSet<SnapPolicy> SnapPolicy {get;set;}
        public virtual DbSet<SnapDirectAges> SnapDirectAges {get;set;}
        public virtual DbSet<SnapDirectAudience> SnapDirectAudience {get;set;}
        public virtual DbSet<SnapDirectAgesAudienceValue> SnapDirectAgesAudienceValue {get;set;}
        public virtual DbSet<SnapDirectDeliverySite> SnapDirectDeliverySite {get;set;}
        public virtual DbSet<SnapDirectSessionType> SnapDirectSessionType {get;set;}
        public virtual DbSet<SnapIndirectMethod> SnapIndirectMethod {get;set;}
        public virtual DbSet<SnapIndirectMethodSelection> SnapIndirectMethodSelection {get;set;}
        public virtual DbSet<SnapIndirectReached> SnapIndirectReached {get;set;}
        public virtual DbSet<SnapIndirectReachedValue> SnapIndirectReachedValue {get;set;}
        public virtual DbSet<SnapPolicyAimed> SnapPolicyAimed {get;set;}
        public virtual DbSet<SnapPolicyAimedSelection> SnapPolicyAimedSelection {get;set;}
        public virtual DbSet<SnapPolicyPartner> SnapPolicyPartner {get;set;}
        public virtual DbSet<SnapPolicyPartnerValue> SnapPolicyPartnerValue {get;set;}
        
        /***************************************/
        //  Snap Ed Budget
        /***************************************/
        public virtual DbSet<SnapBudgetAllowance> SnapBudgetAllowance {get;set;}
        public virtual DbSet<SnapCountyBudget> SnapCountyBudget {get;set;}
        public virtual DbSet<SnapBudgetReimbursementsCounty> SnapBudgetReimbursementsCounty {get;set;}
        public virtual DbSet<SnapBudgetReimbursementsNepAssistant> SnapBudgetReimbursementsNepAssistant {get;set;}
        
        /***************************************/
        //  Snap Ed Commitment
        /***************************************/
        public virtual DbSet<SnapEd_Commitment> SnapEd_Commitment {get;set;}
        public virtual DbSet<SnapEd_ActivityType> SnapEd_ActivityType {get;set;}
        public virtual DbSet<SnapEd_ProjectType> SnapEd_ProjectType {get;set;}
        public virtual DbSet<SnapEd_ReinforcementItem> SnapEd_ReinforcementItem {get;set;}
        public virtual DbSet<SnapEd_ReinforcementItemChoice> SnapEd_ReinforcementItemChoice {get;set;}
        public virtual DbSet<SnapEd_ReinforcementItemSuggestion> SnapEd_ReinforcementItemSuggestion {get;set;}

        /***************************************/
        //  Statistical Contact
        /***************************************/
        public virtual DbSet<ContactRaceEthnicityValue> ContactRaceEthnicityValue {get;set;}
        public virtual DbSet<ContactOptionNumberValue> ContactOptionNumberValue {get;set;}
        public virtual DbSet<Contact> Contact {get;set;}
        public virtual DbSet<ContactRevision> ContactRevision {get;set;}

        /***************************************/
        //  Success Stories
        /***************************************/
        public virtual DbSet<Story> Story {get;set;}
        public virtual DbSet<StoryRevision> StoryRevision {get;set;}
        public virtual DbSet<StoryOutcome> StoryOutcome {get;set;}
        public virtual DbSet <StoryImage> StoryImage {get;set;}

        
        /***************************************/
        //  Extension Events
        /***************************************/

        public virtual DbSet<ExtensionEvent> ExtensionEvent {get;set;}
        public virtual DbSet<ExtensionEventGeoCoordinates> ExtensionEventGeoCoordinates {get;set;}
        public virtual DbSet<ExtensionEventLocation> ExtensionEventLocation {get;set;}
        public virtual DbSet<ExtensionEventPatternedRecurrence> ExtensionEventPatternedRecurrence {get;set;}
        public virtual DbSet<ExtensionEventRecurrencePattern> ExtensionEventRecurrencePattern {get;set;}
        public virtual DbSet<ExtensionEventRecurrenceRange> ExtensionEventRecurrenceRange {get;set;}
        public virtual DbSet<PhysicalAddress> PhysicalAddress {get;set;}

        /***************************************/
        //  Extension InService Training
        /***************************************/

        public virtual DbSet<Training> Training {get;set;}
        public virtual DbSet<TrainingCancelEnrollmentWindow> TrainingCancelEnrollmentWindow {get;set;}
        public virtual DbSet<TrainingEnrollment> TrainingEnrollment {get;set;}
        public virtual DbSet<TainingInstructionalHour> TainingInstructionalHour {get;set;}
        public virtual DbSet<TainingRegisterWindow> TainingRegisterWindow {get;set;}
        

        /***************************************/
        //  Scheduled Tasks
        /***************************************/
        /* public virtual DbSet<TaskOperation> TaskOperation {get;set;}
        public virtual DbSet<TaskPerformed> TaskPerformed {get;set;}
        public virtual DbSet<TaskRecurringSchedule> TaskRecurringSchedule {get;set;}
        public virtual DbSet <TaskSchedule> TaskSchedule {get;set;}
 */
        /***************************************/
        //  General
        /***************************************/
        public virtual DbSet<Log> Log {get; set;}
        public virtual DbSet<FiscalYear> FiscalYear {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            
            modelBuilder.Entity<ActivityOption>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<ActivityRevision>()
                .Property(b => b.SnapCopiesBW)
                .HasDefaultValue(0);
           // Workplace = 1 is the default value
            modelBuilder.Entity<ExpenseRevision>()
                .Property(b => b.StartingLocationType)
                .HasDefaultValue(1);
 /* 
            modelBuilder.Entity<ExpenseRevision>()
                .Property(b => b.StartingLocationOther)
                .HasDefaultValue(""); 
 */
            modelBuilder.Entity<SnapDirectAges>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapDirectAudience>()
                .Property(b => b.Active)
                .HasDefaultValue(true);
            
            modelBuilder.Entity<SnapDirectDeliverySite>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapDirectSessionType>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapIndirectMethod>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapIndirectReached>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapPolicyAimed>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<SnapPolicyPartner>()
                .Property(b => b.Active)
                .HasDefaultValue(true);

            modelBuilder.Entity<UploadFile>().HasIndex(
                file => new { file.Name }).IsUnique(true);
        }
    }
}

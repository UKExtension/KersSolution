using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class SoilReportBundle : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UniqueCode {get;set;}
        public DateTime SampleLabelCreated {get;set;}
        public DateTime LabTestsReady {get;set;}
        public DateTime DataProcessed {get;set;}
        public DateTime AgentReviewed {get;set;}
        public CountyCode PlanningUnit {get;set;}
        public int PlanningUnitId {get;set;}
        public FarmerForReport FarmerForReport {get;set;}
        public int FarmerForReportId {get;set;}
        public int? FarmerAddressId {get;set;}
        public string CoSamnum {get;set;}
        public FarmerAddress FarmerAddress {get;set;}
        public TypeForm TypeForm {get;set;}
        public int TypeFormId {get;set;}
        public List<SoilReport> Reports {get;set;}
        //public List<SoilReportStatusChange> StatusHistory {get;set;}
        public SoilReportStatusChange LastStatus {get;set;}
        public BillingType BillingType {get;set;}
        public int? BillingTypeId {get;set;}
        public string OwnerID {get;set;}
        public string Acres {get;set;}
        public string OptionalInfo {get;set;}
        public string PrivateNote {get;set;}
        public List<SampleInfoBundle> SampleInfoBundles {get;set;}
        public List<OptionalTestSoilReportBundle> OptionalTestSoilReportBundles {get;set;}
    }
}
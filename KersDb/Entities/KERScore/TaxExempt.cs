using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class TaxExempt : IEntityBase
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public KersUser By {get;set;}
        public int ById {get;set;}
        public PlanningUnit Unit {get;set;}
        public int UnitId {get;set;}
        public string Name {get;set;}
        public string Ein {get;set;}
        public string BankName {get;set;}
        public string BankAccountName {get;set;}
        public string DonorsReceivedAck {get;set;}
        public string AnnBudget {get;set;}
        public string AnnFinancialRpt {get;set;}
        public string AnnAuditRpt {get;set;}
        public string AnnInvRpt {get;set;}
        public Boolean GeographicAreaAll {get;set;}
        public Boolean GeographicAreaDistricts {get;set;}
        public Boolean GeographicAreaRegions {get;set;}
        public Boolean GeographicAreaAreas {get;set;}
        public Boolean GeographicAreaCounties {get;set;}
        public List<TaxExemptArea> Areas {get;set;}
        public TaxExemptFinancialYear TaxExemptFinancialYear {get;set;}
        public int? TaxExemptFinancialYearId {get;set;}
        public List<TaxExemptProgramCategory> TaxExemptProgramCategories {get;set;}
        public int HandledId {get;set;}
        public TaxExemptFundsHandled Handled {get;set;}

        //INFORMATION SPECIFIC TO TAX EXEMPT STATUS DERIVED FROM COUNTY EXTENSION DISTRICT 
        public string DistrictName {get;set;}
        public string DistrictEin {get;set;}
        //INFORMATION SPECIFIC TO TAX EXEMPT STATUS DERIVED FROM 501(c) ORGANIZATION 
        public string OrganizationName {get;set;}
        public string OrganizationEin {get;set;}
        public int? OrganizationResidesId {get;set;}
        public PlanningUnit OrganizationResides {get;set;} 
        public string OrganizationLetterDate {get;set;}
        public string OrganizationSignedDate {get;set;}
        public string OrganizationAppropriate {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}
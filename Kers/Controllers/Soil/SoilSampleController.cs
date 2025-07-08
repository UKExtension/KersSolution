using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kers.Models.Repositories;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Kers.Models.Entities;
using Kers.Models.Contexts;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Kers.Models.Data;
using Kers.Models.ViewModels;
using Kers.Models.Entities.SoilData;
using Microsoft.Extensions.Caching.Memory;

namespace Kers.Controllers.Soil
{

    [Route("api/[controller]")]
    public class SoilSampleController : BaseController
    {
        KERScoreContext _coreContext;
        SoilDataContext _context;
    

        public SoilSampleController( 
                    KERSmainContext mainContext,
                    IKersUserRepository userRepo,
                    SoilDataContext _context,
                    KERScoreContext _coreContext,
                    IMemoryCache memoryCache
            ):base(mainContext, _coreContext, userRepo, memoryCache){
                this._context = _context;
                this._coreContext = _coreContext;

        }


        [HttpGet("forms")]
        public IActionResult GetFormTypes(){
            var forms = this._context.TypeForm.OrderBy(r => r.Code).ToList();
            return new OkObjectResult(forms);
        }

        [HttpGet("attributetypes/{formTypeId}")]
        public IActionResult AttributeTypes(int formTypeId){
            var types = this._context.SampleAttributeType.Where(t => t.TypeFormId == formTypeId).OrderBy(r => r.Order).ToList();
            return new OkObjectResult(types);
        }

        [HttpGet("attributes/{attributeTypeId}")]
        public IActionResult Attributes(int attributeTypeId){
            var types = this._context.SampleAttribute.Where(t => t.SampleAttributeTypeId == attributeTypeId).OrderBy(r => r.Name).ToList();
            return new OkObjectResult(types);
        }

        [HttpGet("billingtypes")]
        public IActionResult BillingTypes(){
            var types = this._context.BillingType.OrderBy(r => r.Id).ToList();
            return new OkObjectResult(types);
        }

        [HttpGet("optionaltests")]
        public IActionResult OptionalTests(){
            var types = this._context.OptionalTest.OrderBy(r => r.Id).ToList();
            return new OkObjectResult(types);
        }



        [HttpGet("lastsamplenum/{CountyCodeId?}")]
        public IActionResult LastSampleNum(int CountyCodeId = 0){

            if(CountyCodeId ==  0) CountyCodeId = this.CurrentCountyCode().Id;

            var NumRecord = this._context.CountyAutoCoSamNum.Where( c => c.CountyCodeId == CountyCodeId).FirstOrDefault();
            if( NumRecord != null ) return new OkObjectResult( NumRecord.LastSampleNumber );
            // Last Sample Code Number not found in the database
            int LastNumber = FindLastCountyNum(CountyCodeId);
            // Create Record
            CountyAutoCoSamNum numberRecord = new CountyAutoCoSamNum();
            numberRecord.CountyCodeId = CountyCodeId;
            numberRecord.AutoSampleNumber = true;
            numberRecord.LastSampleNumber = LastNumber;
            this._context.CountyAutoCoSamNum.Add(numberRecord);
            this._context.SaveChanges();
            return new OkObjectResult(LastNumber);
        }

        private int FindLastCountyNum(int CountyCodeId){
            var countyNumbers = this._context.SoilReportBundle.Where( b => b.PlanningUnitId == CountyCodeId).OrderByDescending( b => b.SampleLabelCreated).Take(40).Select( b => b.CoSamnum).ToList();
            int currentNumber = 0;
            foreach ( string countyCodes in countyNumbers){
                var parts = countyCodes.Split("-");
                var NumPart = parts[0];
                int i = 0; 
                bool result = int.TryParse(NumPart, out i);
                if( i > currentNumber ) currentNumber = i;
            }
            return currentNumber;
        }

        private CountyCode CurrentCountyCode(){
            var PlanningUnitId = _coreContext.ReportingProfile.Where( p => p.LinkBlueId == CurrentUserLinkBlueId()).Select( p => p.PlanningUnitId).FirstOrDefault();
            return _context.CountyCodes.Where( c => c.PlanningUnitId == PlanningUnitId ).FirstOrDefault();
        }


        protected string CurrentUserLinkBlueId(){
            return User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }






        [HttpPost("addsample")]
        [Authorize]
        public IActionResult AddSample( [FromBody] SoilReportBundle sample){
            if(sample != null){
                foreach( SampleInfoBundle smpl in sample.SampleInfoBundles){
                    var cleanedConnections = new List<SampleAttributeSampleInfoBundle>();
                    foreach( SampleAttributeSampleInfoBundle attr in smpl.SampleAttributeSampleInfoBundles ){
                        if( attr.SampleAttributeId != 0 ){
                            cleanedConnections.Add(attr);
                        }
                    }
                    smpl.SampleAttributeSampleInfoBundles = cleanedConnections;
                    smpl.PurposeId = 1;
                    sample.TypeFormId = smpl.TypeFormId;
                }
                var contCode = this.CurrentCountyCode();
                sample.PlanningUnit = contCode;
                sample.UniqueCode = Guid.NewGuid().ToString();
                var cntId = sample.CoSamnum;
                var now = DateTime.Now;
                sample.SampleLabelCreated = new DateTime(
                                                    sample.SampleLabelCreated.Year,
                                                    sample.SampleLabelCreated.Month,
                                                    sample.SampleLabelCreated.Day,
                                                    now.Hour,
                                                    now.Minute,
                                                    now.Second) ;
                int i = 0; 
                if( int.TryParse(cntId, out i) ){
                    var NumRecord = this._context.CountyAutoCoSamNum.Where( c => c.CountyCodeId == contCode.Id).FirstOrDefault();
                    NumRecord.LastSampleNumber = i;
                }
                if( sample.FarmerAddress != null ){
                    sample.FarmerAddressId = sample.FarmerAddress.Id;
                    sample.FarmerForReport = CreateFarmerForReport(sample.FarmerAddressId??0);
                    sample.FarmerAddress = null;
                }
                var status = _context.SoilReportStatus.Where( s => s.Name == "Entered").FirstOrDefault();
                sample.LastStatus = new SoilReportStatusChange();
                sample.LastStatus.SoilReportStatus = status;
                sample.LastStatus.Created = DateTime.Now;
                sample.CoSamnum = sample.CoSamnum.PadLeft(5, '0');
                sample.TypeForm = this._context.TypeForm.Find(sample.TypeFormId);
                _context.Add(sample);
                _context.SaveChanges();
                this.Log( sample ,"SoilReportBundle", "Soil Sample Info added.", "SoilReportBundle");

               return new OkObjectResult(sample);
            }else{
                this.Log( sample ,"SoilReportBundle", "Error in adding Soil Sample Info attempt.", "SoilReportBundle", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPut("updatesample/{id}")]
        [Authorize]
        public IActionResult UpdateSample( int id, [FromBody] SoilReportBundle sample){
            var smpl = _context.SoilReportBundle.Where( t => t.Id == id)
                        .Include( s => s.FarmerForReport)
                        .Include( b => b.SampleInfoBundles).ThenInclude( i => i.SampleAttributeSampleInfoBundles)
                        .Include( b => b.OptionalTestSoilReportBundles)
                        .Include( b => b.LastStatus).ThenInclude( s => s.SoilReportStatus)
                        .Include( b => b.Reports)
                        .FirstOrDefault();
            if(sample != null && smpl != null ){
                foreach( var bndl in smpl.SampleInfoBundles){
                    _context.RemoveRange(bndl.SampleAttributeSampleInfoBundles);
                    _context.SaveChanges();
                }
                _context.RemoveRange(smpl.SampleInfoBundles);
                _context.SaveChanges();
                var isItAnAltCrop = false;
                foreach( SampleInfoBundle smplBundle in sample.SampleInfoBundles){
                    var cleanedConnections = new List<SampleAttributeSampleInfoBundle>();
                    foreach( SampleAttributeSampleInfoBundle attr in smplBundle.SampleAttributeSampleInfoBundles ){
                        if( attr.SampleAttributeId != 0 ){
                            cleanedConnections.Add(attr);
                        }
                    }
                    smplBundle.SampleAttributeSampleInfoBundles = cleanedConnections;
                    if( smplBundle.PurposeId == 2 ) isItAnAltCrop = true;
                    smpl.TypeFormId = smplBundle.TypeFormId;
                }
                if(isItAnAltCrop){
                    var status = _context.SoilReportStatus.Where( s => s.Name == "AltCrop").FirstOrDefault();
                    if( smpl.LastStatus == null ){
                        smpl.LastStatus = new SoilReportStatusChange();
                    }
                    smpl.LastStatus.SoilReportStatus = status;
                }else{
                    var now = DateTime.Now;
                    smpl.SampleLabelCreated = new DateTime(
                                                    sample.SampleLabelCreated.Year,
                                                    sample.SampleLabelCreated.Month,
                                                    sample.SampleLabelCreated.Day,
                                                    now.Hour,
                                                    now.Minute,
                                                    now.Second);
                }
                smpl.SampleInfoBundles = sample.SampleInfoBundles;
                smpl.OptionalTestSoilReportBundles = sample.OptionalTestSoilReportBundles; 
                smpl.OwnerID = sample.OwnerID;
                smpl.Acres = sample.Acres;
                smpl.OptionalInfo = sample.OptionalInfo;
                smpl.PrivateNote = sample.PrivateNote;
                if(sample.FarmerAddress != null ){
                    smpl.FarmerAddressId = sample.FarmerAddress.Id;
                }
                var numFarmersForReport = _context.SoilReportBundle.Where( r => r.FarmerForReportId == smpl.FarmerForReportId).Count();
                if(numFarmersForReport > 1){
                    smpl.FarmerForReport = CreateFarmerForReport(smpl.FarmerAddressId??0);
                }else{
                    UpdateFarmerForReport(smpl.FarmerForReport, smpl.FarmerAddressId??0);
                }
                
                smpl.SampleInfoBundles = sample.SampleInfoBundles;
                smpl.BillingTypeId = sample.BillingTypeId;
                smpl.CoSamnum = sample.CoSamnum.PadLeft(5, '0');
                _context.SaveChanges();
                smpl.TypeForm = this._context.TypeForm.Find(smpl.TypeFormId);
                this.Log( sample ,"SoilReportBundle", "Soil Sample Info updated.", "SoilReportBundle");
                return new OkObjectResult(smpl);
            }else{
                this.Log( sample ,"SoilReportBundle", "Not Found SoilSample in an update attempt.", "SoilReportBundle", "Error");
                return new StatusCodeResult(500);
            }
        }


        private FarmerForReport CreateFarmerForReport( int FarmerId ){
            var newFarmer = new FarmerForReport();
            UpdateFarmerForReport(newFarmer, FarmerId);
            return newFarmer;

        }

        private void UpdateFarmerForReport( FarmerForReport newFarmer, int FarmerId){
            var Frmr = this._context.FarmerAddress.Find(FarmerId);
            newFarmer.First = Frmr.First;
            newFarmer.FarmerID = Frmr.FarmerID;
            newFarmer.Mi = Frmr.Mi;
            newFarmer.Last = Frmr.Last;
            newFarmer.Title = Frmr.Title;
            newFarmer.Modifier = Frmr.Modifier;
            newFarmer.Company = Frmr.Company;
            newFarmer.Address = Frmr.Address;
            newFarmer.City = Frmr.City;
            newFarmer.St = Frmr.St;
            newFarmer.Status = Frmr.Status;
            newFarmer.WorkNumber = Frmr.WorkNumber;
            newFarmer.DuplicateHouseHold = Frmr.DuplicateHouseHold;
            newFarmer.HomeNumber = Frmr.HomeNumber;
            newFarmer.Fax = Frmr.Fax;
            newFarmer.FarmerID = Frmr.FarmerID;
            newFarmer.Zip = Frmr.Zip;
            newFarmer.EmailAddress = Frmr.EmailAddress;
        }

        [HttpPost("reportsdata")]
		public IActionResult ReportsData([FromBody] UniqueIds unigueIds)
        {
            var samples = this._context.SoilReportBundle
											.Where( b => unigueIds.ids.Contains(b.UniqueCode) && b.Reports.Count() > 0)
											.Include( b => b.Reports)
                                            .Include( b => b.FarmerForReport)
											.OrderBy( b => b.CoSamnum)
											.ToList();
            List<List<string>> data = new List<List<string>>();
            data.Add( this.ReportHeader());
            foreach( var sample in samples){
                foreach( var report in sample.Reports){
                    data.Add( ReportToStringList(report, sample));
                }
            }
            return new OkObjectResult(data);
        }


        private List<string> ReportHeader(){
            var header = new List<string>{
                "Date",
                "County #",
                "Owner #",
                "Owner Name",
                "Crop",
                "Form Type",
                "Lab pH",
                "Lab P",
                "Lab K",
                "Lab Ca",
                "Lab Mg",
                "Lab Zn",
                "Rec N",
                "Rec P2O5",
                "Rec K2O",
                "Rec LIME",
                "Rec Zn"

            };
            return header;
        }


        private List<string> ReportToStringList(SoilReport report, SoilReportBundle bundle ){
            var row = new List<string>
            {
                bundle.SampleLabelCreated.ToString(),
                bundle.CoSamnum.TrimStart('0'),
                bundle.OwnerID,
                bundle.FarmerForReport.First + " " + bundle.FarmerForReport.Last,
                report.CropInfo1,
                report.TypeForm
            };
            var tests = this._context.TestResults.Where( r => r.PrimeIndex == report.Prime_Index).ToList();
            var LabPh = tests.Where( t => t.TestName == "Soil pH").FirstOrDefault();
            row.Add( LabPh == null ? "" : LabPh.Result);    
            var LabP = tests.Where( t => t.TestName == "Phosphorus").FirstOrDefault();
            row.Add( LabP == null ? "" : LabP.Result);
            var LabK = tests.Where( t => t.TestName == "Potassium").FirstOrDefault();
            row.Add( LabK == null ? "" : LabK.Result);
            var LabCa = tests.Where( t => t.TestName == "Calcium").FirstOrDefault();
            row.Add( LabCa == null ? "" : LabCa.Result);
            var LabMg = tests.Where( t => t.TestName == "Magnesium").FirstOrDefault();
            row.Add( LabMg == null ? "" : LabMg.Result);
            var LabZn = tests.Where( t => t.TestName == "Zinc").FirstOrDefault();
            row.Add( LabMg == null ? "" : LabZn.Result);
            var RecN = tests.Where( t => t.TestName == "Nitrogen").FirstOrDefault();
            row.Add( RecN == null ? "" : RecN.Recommmendation);
            var RecP = tests.Where( t => t.TestName == "Phosphorus").FirstOrDefault();
            row.Add( RecP == null ? "" : RecP.Recommmendation);
            var RecK = tests.Where( t => t.TestName == "Potassium").FirstOrDefault();
            row.Add( RecK == null ? "" : RecK.Recommmendation);
            row.Add( report.LimeComment);
            var RecZn = tests.Where( t => t.TestName == "Zinc").FirstOrDefault();
            row.Add( RecZn == null ? "" : RecZn.Recommmendation);
            return row;
        }

        [HttpPost("checksamnum")]
		public IActionResult CheckSampleNumber([FromBody] CoSamNumCheck SampleNumber)
        {
            var exists = false;
            var sNum = SampleNumber.CoSamNum.PadLeft(5, '0');
            var dateToCompare = DateTime.Now.AddDays(-100);
            CountyCode CountyCode = this.CurrentCountyCode();
            exists = _context.SoilReportBundle
                        .Where(
                            b =>
                                b.PlanningUnit == CountyCode
                                &&
                                b.CoSamnum == sNum
                                &&
                                b.SampleLabelCreated > dateToCompare
                        ).Any();
            return new OkObjectResult(exists);
        }



    }
    public class UniqueIds{
		public List<string> ids;
	}
    public class CoSamNumCheck{
        public string CoSamNum;
    }
}
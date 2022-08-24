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

namespace Kers.Controllers.Soil
{

    [Route("api/[controller]")]
    public class SoilSampleController : Controller
    {
        KERScoreContext _coreContext;
        SoilDataContext _context;

        public SoilSampleController( 
                    SoilDataContext _context,
                    KERScoreContext _coreContext
            ){
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
                int i = 0; 
                if( int.TryParse(cntId, out i) ){
                    var NumRecord = this._context.CountyAutoCoSamNum.Where( c => c.CountyCodeId == contCode.Id).FirstOrDefault();
                    NumRecord.LastSampleNumber = i;
                }
                if( sample.FarmerAddress != null ){
                    sample.FarmerAddressId = this.FarmerAddress.Id;
                    sample.FarmerForReport = CreateFarmerForReport(sample.FarmerAddressId??0);
                }


                _context.Add(sample);
                _context.SaveChanges();
                
               
               return new OkObjectResult(sample);
            }else{
                //this.Log( activity ,"ActivityRevision", "Error in adding Activity attempt.", "Activity", "Error");
                return new StatusCodeResult(500);
            }
        }

        private FarmerForReport CreateFarmerForReport( int FarmerId ){
            var newFarmer = new FarmerForReport();
            var Frmr = this._context.FarmerAddress.Find(FarmerId);
            newFarmer.First = Frmr.First;
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
            
            return newFarmer;

        }




    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Contexts;
using Kers.Models.Entities.SoilData;
using System;

namespace Kers.Controllers
{

    [Route("api/[controller]")]
    public class SoilDataController : ExtensionEventController
    {
        KERScoreContext _context;
        SoilDataContext _soilDataContext;
        IKersUserRepository _userRepo;
        ILogRepository logRepo;
        public SoilDataController( 
                    KERSmainContext mainContext,
                    KERScoreContext context,
                    SoilDataContext soilDataContext,
                    IKersUserRepository userRepo,
                    ILogRepository logRepo
            ):base(mainContext, context, userRepo){
           this._context = context;
           this._userRepo = userRepo;
           this.logRepo = logRepo;
           this._soilDataContext = soilDataContext;
            
            //Associate County Codes with Planning Units if not done yet
            if(!soilDataContext.CountyCodes.Where( c => c.PlanningUnitId != 0).Any()){
                foreach( var countyCode in _soilDataContext.CountyCodes){
                    var unit = this.context.PlanningUnit.
                                    Where( u => 
                                                u.Name.Count() > 11
                                                &&
                                                u.Name.Substring(0,u.Name.Count() - 11).ToUpper() == countyCode.Name 
                                        ).
                                    FirstOrDefault();
                    if( unit != null) countyCode.PlanningUnitId = unit.Id;
                }
                soilDataContext.SaveChanges();
            }



        }



        [HttpGet("GetCustom/{allCounties?}/{countyId?}")]
        [Authorize]
        public IActionResult GetCustom( [FromQuery] string search, 
                                        [FromQuery] DateTime start,
                                        [FromQuery] DateTime end,
                                        [FromQuery] string status,
                                        Boolean allCounties = false,
                                        int countyId = 0
                                        ){
            UpdateBundles();
            var bundles = from i in _soilDataContext.SoilReportBundle select i;
            // Allow reports by all counties
            if( !allCounties ){
                if(countyId == 0){
                    var user = this.CurrentUser();
                    countyId = user.RprtngProfile.PlanningUnitId;
                }
                bundles = bundles.Where( b => b.PlanningUnit.PlanningUnitId == countyId);
            }
            if(search != null && search != ""){
                bundles = bundles.Where( i => i.FarmerForReport != null 
                                    && 
                                (
                                    i.FarmerForReport.First.Contains(search)
                                    ||
                                    i.FarmerForReport.Last.Contains(search)
                                )            
                        );
            } 
            if(start != null){
                bundles = bundles.Where( i => i.DataProcessed > start);
            }
            if( end != null){
                bundles = bundles.Where( i => i.DataProcessed < end);
            }
            bundles = bundles
                        .Include( b => b.Reports)
                        .Include( b => b.FarmerForReport)
                        .Include( b => b.LastStatus).ThenInclude( s => s.SoilReportStatus)
                        .Include( b => b.TypeForm);
            return new OkObjectResult(bundles.OrderBy(t => t.DataProcessed));
        }

        private void UpdateBundles(){
            SoilReport OrphanedReport =  _soilDataContext.SoilReport
                                                .Where( r => r.SoilReportBundleId == null)
                                                .FirstOrDefault();
            if( OrphanedReport != null ){
                do{
                    var SameSample = _soilDataContext.SoilReport
                                        .Where( r => r.CoId == OrphanedReport.CoId && r.CoSamnum == OrphanedReport.CoSamnum)
                                        .ToList();
                    var Bundle = new SoilReportBundle();
                    Bundle.Reports = SameSample;
                    Bundle.StatusHistory = new List<SoilReportStatusChange>();
                    Bundle.PlanningUnit = _soilDataContext.CountyCodes.Where( c => c.CountyID == OrphanedReport.CoId).FirstOrDefault();
                    if(Bundle.FarmerForReport == null){
                        Bundle.FarmerForReport = _soilDataContext.FarmerForReport
                                                        .Where(f => f.FarmerID == OrphanedReport.FarmerID)
                                                        .FirstOrDefault();
                        if(Bundle.FarmerForReport != null){
                            Bundle.FarmerAddress = _soilDataContext.FarmerAddress
                                                        .Where(f => f.FarmerID == Bundle.FarmerForReport.FarmerID.Substring(0,11))
                                                        .FirstOrDefault();
                        }
                        
                    }
                    Bundle.SampleLabelCreated = OrphanedReport.DateIn;
                    Bundle.LabTestsReady = OrphanedReport.DateOut;
                    Bundle.DataProcessed = OrphanedReport.DateSent;
                    Bundle.TypeForm = _soilDataContext.TypeForm
                                                .Where( f => f.Code == OrphanedReport.TypeForm)
                                                .FirstOrDefault();
                    Bundle.UniqueCode = Guid.NewGuid().ToString();
                    _soilDataContext.Add(Bundle);
                    _soilDataContext.SaveChanges();
                    OrphanedReport = _soilDataContext.SoilReport.Where( r => r.SoilReportBundleId == null).FirstOrDefault();
                }while( OrphanedReport != null);
            }
        }

        [HttpPut("updatebundleaddress/{bundleId}")]
        [Authorize]
        public IActionResult UpdateBundleAddress( int bundleId, [FromBody] FarmerAddress address){
            var bundle = _soilDataContext.SoilReportBundle
                            .Where( b => b.Id == bundleId)
                            .Include( b => b.FarmerForReport)
                            .FirstOrDefault();
            if(address != null && bundle != null ){
                FarmerForReport adr;
                if(bundle.FarmerForReport != null){
                    if(_soilDataContext.SoilReportBundle
                            .Where(b => b.FarmerForReport == bundle.FarmerForReport)
                            .Count() > 1){
                        adr = new FarmerForReport();
                    }else{
                        adr = bundle.FarmerForReport;
                    }
                    
                }else{
                    adr = new FarmerForReport();
                }
                adr.First = address.First;
                adr.Last = address.Last;
                adr.Address = address.Address;
                adr.City = address.City;
                adr.St = address.St;
                adr.Zip = address.Zip;
                adr.HomeNumber = address.HomeNumber;
                adr.EmailAddress = address.EmailAddress;
                bundle.FarmerForReport = adr;
                bundle.FarmerAddressId = address.Id;
                _soilDataContext.SaveChanges();
                this.Log(adr,"SoilReportBundle", "Bundle FarmerAddress Updated.");
                return new OkObjectResult(bundle);
            }else{
                this.Log( address ,"SoilReportBundle", "Not Found SoilReportBundle or missing Farmer Address in an update bundle attempt.", "SoilReportBundle", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("updatecropnote/{reportId}")]
        [Authorize]
        public IActionResult UpdateCropNote( int reportId, [FromBody] SoilReport note){
            var crop = _soilDataContext.SoilReport
                            .Where( b => b.Id == reportId)
                            .FirstOrDefault();
            if(crop != null && note != null ){
                crop.AgentNote = note.AgentNote;
                _soilDataContext.SaveChanges();
                this.Log(crop,"SoilReport", "SoilReport note Updated.");
                return new OkObjectResult(crop);
            }else{
                this.Log( note ,"SoilReport", "Not Found SoilReport or missing note in an update report attempt.", "SoilReport", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPost("addaddress")]
        [Authorize]
        public IActionResult AddAddress( [FromBody] FarmerAddress address){
            if(address != null){
                var user = this.CurrentUser();
                var countyCode = _soilDataContext.CountyCodes.FirstOrDefault( c => c.PlanningUnitId == user.RprtngProfile.PlanningUnitId);
                address.CountyCode = countyCode;
                _soilDataContext.Add(address); 
                _soilDataContext.SaveChanges();
                this.Log(address,"FarmerAddress", "Farmer Address added.");

                return new OkObjectResult(address);
            }else{
                this.Log( address ,"FarmerAddress", "Error in adding FarmerAddress attempt.", "FarmerAddress", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("updateaddress/{id}")]
        [Authorize]
        public IActionResult UpdateAddress( int id, [FromBody] FarmerAddress address){
            var adr = _soilDataContext.FarmerAddress.Find(id);
            if(address != null && adr != null ){
                adr.First = address.First;
                adr.Last = address.Last;
                adr.Address = address.Address;
                adr.City = address.City;
                adr.St = address.St;
                adr.Zip = address.Zip;
                adr.HomeNumber = address.HomeNumber;
                adr.EmailAddress = address.EmailAddress;
                _soilDataContext.SaveChanges();
                this.Log(address,"FarmerAddress", "FarmerAddress Updated.");
                return new OkObjectResult(address);
            }else{
                this.Log( address ,"FarmerAddress", "Not Found Farmer Address in an update attempt.", "FarmerAddress", "Error");
                return new StatusCodeResult(500);
            }
        }


        [HttpPost("addNote")]
        [Authorize]
        public IActionResult AddNote( [FromBody] CountyNote note){
            if(note != null){
                var user = this.CurrentUser();
                var countyCode = _soilDataContext.CountyCodes.FirstOrDefault( c => c.PlanningUnitId == user.RprtngProfile.PlanningUnitId);
                note.CountyCode = countyCode;
                _soilDataContext.Add(note); 
                _soilDataContext.SaveChanges();
                this.Log(note,"CountyNote", "County Note added.");
                return new OkObjectResult(note); 
            }else{
                this.Log( note ,"CountyNote", "Error in adding County Note attempt.", "CountyNote", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpPut("updateNote/{id}")]
        [Authorize]
        public IActionResult UpdateNote( int id, [FromBody] CountyNote note){
            var nte = _soilDataContext.CountyNotes.Find(id);
            if(note != null && nte != null ){
                nte.Name = note.Name;
                nte.Note = note.Note;
                _soilDataContext.SaveChanges();
                this.Log(note,"CountyNote", "County Note Updated.");
                return new OkObjectResult(nte);
            }else{
                this.Log( note ,"CountyNote", "Not Found County Note in an update attempt.", "CountyNote", "Error");
                return new StatusCodeResult(500);
            }
        }

        [HttpDelete("deleteNote/{id}")]
        public IActionResult DeleteNote( int id){
            var nte = _soilDataContext.CountyNotes.Find(id);

            if(nte != null){
                
                this._soilDataContext.Remove(nte);
                _soilDataContext.SaveChanges();
                
                return new OkResult();
            }else{
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("updateSignees/{countyId}")]
        [Authorize]
        public async Task<IActionResult> UpdateSignees( [FromBody] SigneesObject signees, int countyId = 0){
            if( countyId == 0 ){
                countyId = this.CurrentUser().RprtngProfile.PlanningUnitId;
            }
            var CurrentCountyCode = await this._soilDataContext.CountyCodes.Where( c => c.PlanningUnitId == countyId).FirstOrDefaultAsync();
            if( CurrentCountyCode == null){
                this.Log( signees ,"FormTypeSignees", "Not Found County in an FormTypeSignees update attempt.", "FormTypeSignees", "Error");
                return new StatusCodeResult(500);
            }

            var currentSignees = await _soilDataContext.FormTypeSignees
                                    .Where( s =>  s.PlanningUnit == CurrentCountyCode)
                                    .ToListAsync();
            _soilDataContext.FormTypeSignees.RemoveRange(currentSignees);
            _soilDataContext.FormTypeSignees.AddRange(signees.signees);
            _soilDataContext.SaveChanges();
            return new OkObjectResult(signees);
        }



        [HttpGet("addresses/{countyid?}")]
        public async Task<IActionResult> FarmerAddressesByCounty(int countyid = 0){
            if( countyid == 0 ){
                countyid = this.CurrentUser().RprtngProfile.PlanningUnitId;
            }
            var addresses = await _soilDataContext.FarmerAddress.
                                    Where(a => a.CountyCode.PlanningUnitId == countyid).
                                    ToListAsync();
            return new OkObjectResult(addresses);
        }

        [HttpGet("labResults/{reportId}")]
        [Authorize]
        public async Task<IActionResult> labResults(int reportId){
            var results = await _soilDataContext.TestResults
                                    .Where(a => a.PrimeIndex == reportId)
                                    .OrderBy( a => a.Order)
                                    .ToListAsync();
            return new OkObjectResult(results);
        }

        [HttpGet("notesByCounty/{countyid?}")]
        public async Task<IActionResult> NotesByCounty(int countyid = 0){
            if( countyid == 0 ){
                countyid = this.CurrentUser().RprtngProfile.PlanningUnitId;
            }
            var notes = await _soilDataContext.CountyNotes.
                                    Where(a => a.CountyCode.PlanningUnitId == countyid).
                                    ToListAsync();
            return new OkObjectResult(notes);
        }

        [HttpGet("signeesByCounty/{countyid?}")]
        public async Task<IActionResult> SigneesByCounty(int countyid = 0){
            if( countyid == 0 ){
                countyid = this.CurrentUser().RprtngProfile.PlanningUnitId;
            }
            var CurrentCountyCode = await this._soilDataContext.CountyCodes.Where( c => c.PlanningUnitId == countyid).FirstOrDefaultAsync();
            if( CurrentCountyCode == null) return new StatusCodeResult(500);
            var FormTypes = this._soilDataContext.TypeForm.OrderBy( t => t.Code).ToListAsync();
            var signeesPerCounty = new List<FormTypeSignees>();
            foreach( var type in await FormTypes){
                var signee = await _soilDataContext.FormTypeSignees
                                    .Where( s => s.TypeForm == type && s.PlanningUnit == CurrentCountyCode)
                                    .FirstOrDefaultAsync();
                if(signee != null ){
                    signeesPerCounty.Add(signee);
                }else{
                    var empty = new FormTypeSignees();
                    empty.PlanningUnit = CurrentCountyCode;
                    empty.TypeForm = type;
                    signeesPerCounty.Add( empty );
                }
            }
            return new OkObjectResult(signeesPerCounty);
        }

    }


    public class SigneesObject{
        public List<FormTypeSignees> signees;
    }
}
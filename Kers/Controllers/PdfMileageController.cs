using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkiaSharp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.RegularExpressions;

namespace Kers.Controllers
{
	[Route("api/[controller]")]
    public class PdfMileageController : PdfBaseController
    {

		IExpenseRepository expenseRepo;


        public PdfMileageController(
            KERScoreContext _context,
			IKersUserRepository userRepo,
            IExpenseRepository expenseRepo,
			KERSmainContext mainContext,
			IMemoryCache _cache
        ):
			base(_context, userRepo, mainContext, _cache){
				this.expenseRepo = expenseRepo;
			}


		[HttpGet("mileagelog/{year}/{month}/{userId?}/{overnight?}/{personal?}")]
        [Authorize]
		public IActionResult TripExpenses(int year, int month, int userId = 0, Boolean overnight = false, Boolean personal = true)
        {
			

			using (var stream = new SKDynamicMemoryWStream ())
                using (var document = SKDocument.CreatePdf (stream, this.metadata( "Kers, Expenses, Reporting, Activity", "Expenses Report", "Mileage Report") )) {
				
					KersUser user;
					if(userId == 0){
						user = this.CurrentUser();
					}else{
						user = this._context.KersUser.
										Where(u => u.Id == userId).
										Include(u => u.PersonalProfile). 
										Include(u => u.RprtngProfile).ThenInclude(p => p.PlanningUnit).
										Include(u => u.ExtensionPosition).
										FirstOrDefault();
					}
                

					var expenses = this.expenseRepo.MileagePerMonth(user, year, month);
	
					if(personal){
						expenses = expenses.Where( e => e.LastRevision.VehicleType != 2 && e.LastRevision.isOvernight == overnight);
					}else{
						expenses = expenses.Where( e => e.LastRevision.VehicleType == 2);
					}

					var dataObjext = new MileageData(expenses);
					dataObjext.SetIsItPersonalVehicle(personal);
					var dt = dataObjext.getData();
					
					
					document.Close();
					
					return File(stream.DetachAsData().AsStream(), "application/pdf", "TripExpensesReport.pdf");	
			}			
		}

	}


	class MileageData{



		IQueryable<Expense> _expenses;

		List<MileageSegmentWithDate> _segments = new List<MileageSegmentWithDate>();
		int mileageColumnsCount = 3;



		List<ExpenseNumLines> _expenseLines;
		bool isItPersonalVehicle = true;
		List<MileageLogPageData> pages = new List<MileageLogPageData>();
		string[] UKSourceNames = new string[]{"State", "Federal"};
		string[] countySourceNames = new string[]{"County Travel (Reimbursed to Employee)"};
		string[] professionalDevelopmentNames = new string[]{"Professional Improvement (Reimbursed to Employee)"};

		public int startLocationCharacterLength = 52;
		public int endLocationCharacterLength = 52;
		public int businessPurposeCharacterLength = 50;

		public int mileageColumnCharacterLength = 12;
		
		public bool CountyColumnPresent = true;
		public bool ProfImprvmntColumnPresent = true;
		public bool UKColumnPresent = true;

		

		//********************************/
		// Dimensions definition         //
		//********************************/

		int headerHeight = 130;
		int lineHeight = 15;
		int signaturesHeight = 160;
		int pageHeight = 612;
		int pageMargins = 92;


		public MileageData( IQueryable<Expense> data){
			this._expenses = data;
		}
		public void SetIsItPersonalVehicle( bool personal ){
			this.isItPersonalVehicle = personal;
		}

		private void ExtractSegments(){
			foreach( var expns in this._expenses){
				foreach( var segment in expns.LastRevision.Segments){
					var SegmentWithDate = new MileageSegmentWithDate(){
						MileageDate = expns.ExpenseDate,
						segment = segment
					};
					_segments.Add(SegmentWithDate);
				}
			}
		}
		private void AdjustCharacterLengths(){
			var countyMileage = _segments.Where( e => countySourceNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			var profImprvMileage = _segments.Where( e => this.professionalDevelopmentNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			var UKMileage = _segments.Where( e => this.UKSourceNames.Contains( e.segment.FundingSource.Name) ).Sum( e => e.segment.Mileage );
			if( countyMileage == 0 ){
				this.CountyColumnPresent = false;
				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);
			}
			if( profImprvMileage == 0 ){
				this.ProfImprvmntColumnPresent = false;
				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);
			}
			if( UKMileage == 0 ){
				this.UKColumnPresent = false;
				this.startLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.endLocationCharacterLength += (mileageColumnCharacterLength/3);
				this.businessPurposeCharacterLength += (mileageColumnCharacterLength/3);
			}
		}

		public List<MileageLogPageData> getData(){
			ExtractSegments();
			AdjustCharacterLengths();
			//getLines();
			//DivideExpenses();
			//CalculatePageData();
			return pages;
		}



	}

	public class MileageSegmentWithDate{
		public DateTime MileageDate;
		public MileageSegment segment;
	}

	public class MileageLogPageData{
		public bool header = false;
		public bool signatures = false;

		public List<ExpenseMileageLogTableData> data = new List<ExpenseMileageLogTableData>();

	}

	public class MileageLogTableData{
		public bool header = true;
		public List<MileageNumLines> expenses = new List<MileageNumLines>();
	}

	public class MileageNumLines{
		public int lines;
		public ExpenseRevision expense;
		public List<string> locationLines;
		public List<string> purposeLines;
	}


}
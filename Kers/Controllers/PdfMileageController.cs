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


		[HttpGet("tripexpenses/{year}/{month}/{userId?}/{overnight?}/{personal?}")]
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
                

					var expenses = this.expenseRepo.PerMonth(user, year, month, "asc");
	
					if(personal){
						expenses = expenses.Where( e => e.VehicleType != 2 && e.isOvernight == overnight && e.Mileage != 0).ToList();
					}else{
						expenses = expenses.Where( e => e.VehicleType == 2).ToList();
					}

					var dataObjext = new MileageData(expenses);
					dataObjext.SetIsItPersonalVehicle(personal);
					
					
					document.Close();
					
					return File(stream.DetachAsData().AsStream(), "application/pdf", "TripExpensesReport.pdf");	
			}			
		}

	}


	class MileageData{



		List<ExpenseRevision> _expenses;
		List<ExpenseNumLines> _expenseLines;
		List<ExpenseNumLines> _countyExpenses;
		List<ExpenseNumLines> _proffesionalDevelopmentExpenses;
		List<ExpenseNumLines> _nonCountyExpenses;
		bool divideExpanses = true;
		bool isItPersonalVehicle = true;
		List<ExpenseMileageLogPageData> pages = new List<ExpenseMileageLogPageData>();
		string[] nonCountySourceNames = new string[]{"State", "Federal"};
		string[] professionalDevelopmentNames = new string[]{"Professional Improvement (Reimbursed to Employee)"};
		int locationLinesCharacterLength = 52;
		int businessPurposeLinesCharacterLength = 50;

		//********************************/
		// Dimensions definition         //
		//********************************/

		int headerHeight = 130;
		int lineHeight = 15;
		int signaturesHeight = 160;
		int pageHeight = 612;
		int pageMargins = 92;


		public MileageData( List<ExpenseRevision> data){
			this._expenses = data;
		}
		public void SetIsItPersonalVehicle( bool personal ){
			this.isItPersonalVehicle = personal;
		}

	}


}
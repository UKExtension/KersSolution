using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Kers.Models.Entities;

namespace Kers.Models.Repositories
{
    public class ExpenseRepository : EntityBaseRepository<Expense>, IExpenseRepository
    {

        private KERScoreContext coreContext;
        public ExpenseRepository(KERScoreContext context)
            : base(context)
        { 
            this.coreContext = context;
        }



        public List<ExpenseRevision> PerMonth(KersUser user, int year, int month, string order = "desc"){
            
            IOrderedQueryable<Expense> lastExpenses;
            if(order == "desc"){
                lastExpenses = coreContext.Expense.
                                Where(e=>e.KersUser == user && e.ExpenseDate.Month == month && e.ExpenseDate.Year == year).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceNonMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateBreakfast).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateLunch).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateDinner).
                                OrderByDescending(e=>e.Revisions.OrderBy(f=>f.Created).Last().ExpenseDate);
            }else{
                lastExpenses = coreContext.Expense.
                                Where(e=>e.KersUser == user && e.ExpenseDate.Month == month && e.ExpenseDate.Year == year).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceNonMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateBreakfast).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateLunch).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateDinner).
                                OrderBy(e=>e.Revisions.OrderBy(f=>f.Created).Last().ExpenseDate);
            }
                                
            var revs = new List<ExpenseRevision>();
            if( lastExpenses != null){
                foreach(var expense in lastExpenses){
                    if(expense.Revisions.Count != 0){
                        revs.Add( expense.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            }
            return revs;
        }

        public List<ExpenseRevision> PerFiscalYear(KersUser user, FiscalYear fiscalYear){
            
            var lastExpenses = coreContext.Expense.
                                Where(e=>e.KersUser == user && e.ExpenseDate <= fiscalYear.End && e.ExpenseDate >= fiscalYear.Start).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceNonMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.FundingSourceMileage).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateBreakfast).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateLunch).
                                Include(e=>e.Revisions).ThenInclude( r => r.MealRateDinner).
                                OrderByDescending(e=>e.Revisions.OrderBy(f=>f.Created).Last().ExpenseDate);
            var revs = new List<ExpenseRevision>();
            if( lastExpenses != null){
                foreach(var expense in lastExpenses){
                    if(expense.Revisions.Count != 0){
                        revs.Add( expense.Revisions.OrderBy(r=>r.Created).Last() );
                    }
                }
            }
            return revs;
        }

        public List<ExpenseSummary> SummariesPerFiscalYear(KersUser user, FiscalYear fiscalYear){
            
            var expenses = PerFiscalYear(user, fiscalYear);
            var mileageRate = MileageRate(user);

            return DivideByType(expenses, mileageRate);
        }


        public List<ExpenseSummary> Summaries(KersUser user, int year, int month){
            
            var expenses = PerMonth(user, year, month);
            var mileageRate = MileageRate(user, year, month);

            return DivideByType(expenses, mileageRate);
        }

        private List<ExpenseSummary> DivideByType(List<ExpenseRevision>expenses, float mileageRate){
            var list = new List<ExpenseSummary>();
            foreach(var source in FundingSources()){
                var expensesMileage = expenses.Where(e => e.FundingSourceMileageId == source.Id).ToList();
                var expenseNonMileage = expenses.Where( e => e.FundingSourceNonMileageId == source.Id).ToList();
                var sum = Summarize(source, expensesMileage, expenseNonMileage, mileageRate);
                if(sum.total != 0){
                    list.Add(sum);
                }
            }
            return list;
        }

        public float MileageRate(KersUser user, int year = 0, int month = 0){
            ExpenseMileageRate rate;
            if(month == 0 || year == 0){
                rate = this.coreContext.
                                ExpenseMileageRate.
                                Where(e => e.InstitutionId == user.RprtngProfile.InstitutionId && e.Start < DateTime.Now && e.End > DateTime.Now).
                                FirstOrDefault();   
            }else{
                var reportDate = new DateTime(year, month, 15);
                rate = this.coreContext.
                                ExpenseMileageRate.
                                Where(e => e.InstitutionId == user.RprtngProfile.InstitutionId && e.Start < reportDate && e.End > reportDate).
                                FirstOrDefault();
            }
                     
            return rate.Rate;
        }

        private ExpenseSummary Summarize(ExpenseFundingSource fundingSource, List<ExpenseRevision> mileage, List<ExpenseRevision> nonMileage, float MileageRate){
            var expenseSourceSummary = new ExpenseSummary();
            expenseSourceSummary.fundingSource = fundingSource;
            float miles = 0;
            float milesCost = 0;
            float meals = 0;
            float lodging = 0;
            float registration = 0;
            float other = 0;
            foreach(var mileageExpenses in mileage){
                miles += mileageExpenses.Mileage;
            }
            milesCost = (miles * MileageRate);
            foreach(var nonMileageExpenses in nonMileage){
                lodging += nonMileageExpenses.Lodging;
                registration += nonMileageExpenses.Registration;
                other += nonMileageExpenses.otherExpenseCost;
                meals += Breakfast(nonMileageExpenses);
                meals += Lunch(nonMileageExpenses);
                meals += Dinner(nonMileageExpenses);
            }
            expenseSourceSummary.miles = miles;
            expenseSourceSummary.mileageCost = milesCost;
            expenseSourceSummary.lodging = lodging;
            expenseSourceSummary.registration = registration;
            expenseSourceSummary.other = other;
            expenseSourceSummary.meals = meals;
            expenseSourceSummary.total = lodging + milesCost + meals + registration + other;
            
            return expenseSourceSummary;
            
        }


        public List<ExpenseFundingSource> FundingSources(){
            return this.coreContext.ExpenseFundingSource.OrderBy(f => f.Order).ToList();
        }


        public float Breakfast(ExpenseRevision expense){
            if(expense.MealRateBreakfast != null){
                return expense.MealRateBreakfast.BreakfastRate;
            }else if(expense.MealRateBreakfastId == 0){
                return expense.MealRateBreakfastCustom?? 0.0f;
            }
            return 0.0f;
        }
        public float Lunch(ExpenseRevision expense){
            if(expense.MealRateLunch != null){
                return expense.MealRateLunch.LunchRate;
            }else if(expense.MealRateLunchId == 0){
                return expense.MealRateLunchCustom?? 0.0f;
            }
            return 0.0f;
        }
        public float Dinner(ExpenseRevision expense){
            if(expense.MealRateDinner != null){
                return expense.MealRateDinner.DinnerRate;
            }else if(expense.MealRateDinnerId == 0){
                return expense.MealRateDinnerCustom?? 0.0f;
            }
            return 0.0f;
        }




        /*
        
        
        calculate(){
        for(let source of this.fundingSources){
            var expensesMileage = this.monthExpenses.filter(e => e.fundingSourceMileageId == source.id);
            var expenseNonMileage = this.monthExpenses.filter( e => e.fundingSourceNonMileageId == source.id);
            this.summarize(source, expensesMileage, expenseNonMileage);
        }
        this.loading = false;
    }   


    



    breakfast(expense:Expense){
        if(expense.mealRateBreakfast != null){
            return expense.mealRateBreakfast.breakfastRate;
        }else if(expense.mealRateBreakfastId == 0){
            return expense.mealRateBreakfastCustom;
        }
        return 0;
    }
    lunch(expense:Expense){
        if(expense.mealRateLunch != null){
            return expense.mealRateLunch.lunchRate;
        }else if(expense.mealRateLunchId == 0){
            return expense.mealRateLunchCustom;
        }
        return 0;
    }
    dinner(expense:Expense){
        if(expense.mealRateDinner != null){
            return expense.mealRateDinner.dinnerRate;
        }else if(expense.mealRateDinnerId == 0){
            return expense.mealRateDinnerCustom;
        }
        return 0;
    }
        
        
        
         */


    }


    public class ExpenseSummary{
        public ExpenseFundingSource fundingSource;
        public float miles;
        public float mileageCost;
        public float meals;
        public float lodging;
        public float registration;
        public float other;
        public float total;
    }

}
import { Component, Input } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { User } from "../../user/user.service";

@Component({
    selector: 'expense-reports-details',
    template: `
  <loading *ngIf="loading"></loading>
  <div *ngIf="!loading">
        <div class="col-md-12 col-sm-12 col-xs-12" *ngFor="let expense of monthExpenses">
            <div class="ln_solid"></div>
                <h3>{{expense.expenseDate| date:'mediumDate'}}</h3>
                <p><strong>Location: </strong>{{expense.expenseLocation}}</p>

                <div class="row invoice-info">
                    <div class="col-sm-6 invoice-col">
                        <p *ngIf="expense.fundingSourceMileage">
                            <strong>Mileage Funding: </strong><br>{{expense.fundingSourceMileage.name}}
                        </p>
                        <div><strong>Miles: </strong>{{expense.mileage}}</div>
                        <div *ngIf="expense.departTime"><strong>Time Departed: </strong>{{expense.departTime | date:'shortTime'}}</div>
                        <div *ngIf="expense.returnTime"><strong>Time Returned: </strong>{{expense.returnTime | date:'shortTime'}}</div>
                    </div>

                    <div class="col-sm-6 invoice-col">
                        <p *ngIf="expense.fundingSourceNonMileage">
                            <strong>Expense Funding: </strong><br>{{expense.fundingSourceNonMileage.name}}
                        </p>
                        <div class="row">
                            <div class="col-md-4"><strong>Breakfast: </strong>{{ breakfast(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Lunch: </strong>{{ lunch(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Dinner: </strong>{{ dinner(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                        </div>
                        <div class="row">
                            <div class="col-md-4"><strong>Lodging: </strong>{{ expense.lodging | currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Registration: </strong>{{ expense.registration | currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Other: </strong>{{ expense.otherExpenseCost| currency:'USD':'symbol':'1.2-2'}}</div>
                        </div>
                        <div *ngIf="expense.otherExpenseCost != 0">{{expense.otherExpenseExplanation}}</div>
                    </div>

                </div>

        </div>
    </div>
        `
})
export class ExpenseReportsDetailsComponent { 


    errorMessage: string;
    @Input() month;
    @Input() year;
    @Input() user:User;
    monthExpenses: Expense[];
    loading = false;

    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.loading = true;
        this.service.expensesPerMonth(this.month.month, this.year.year, userid, 'asc').subscribe(
            res=> {
                this.monthExpenses = <Expense[]>res;
                this.loading = false;
            },
            err => this.errorMessage = <any>err
        );
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

}
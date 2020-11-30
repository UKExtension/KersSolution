import { Component, Input } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { User } from "../../user/user.service";
import { Mileage, MileageSegment } from '../../mileage/mileage';
import { MileageService } from '../../mileage/mileage.service';

@Component({
    selector: 'expense-reports-details',
    template: `
  <loading *ngIf="loading"></loading>
  <div *ngIf="!loading && !isMileage">
        <div class="col-md-12 col-sm-12 col-xs-12" *ngFor="let expense of monthExpenses">
            <div class="ln_solid"></div>
                <div class="row">
                    <div class="col-sm-6">
                        <h3 style="margin-bottom:0;">{{expense.expenseDate| date:'mediumDate'}}</h3>
                        <p *ngIf="expense.isOvernight">Overnight Trip</p>
                        <p *ngIf="!expense.isOvernight">Day Trip</p>
                    </div>
                    <div class="col-sm-6">
                        <div><strong>Starting Location: </strong>{{expense.startingLocationType == 2 ? "Home" : "Workplace"}}</div>
                        <div><strong>Destination(s): </strong>{{expense.expenseLocation}}</div>
                        <div><strong>Business Purpose: </strong>{{expense.businessPurpose}}</div>
                        <div *ngIf="expense.comment != ''"><strong>Comment: </strong>{{expense.comment}}</div>
                    </div>
                </div>
                

                <div class="row invoice-info">
                    <div class="col-sm-6 invoice-col">
                        <p *ngIf="expense.fundingSourceMileage">
                            <strong>Mileage Funding: </strong><br>{{expense.fundingSourceMileage.name}}
                        </p>
                        <div *ngIf="expense.fundingSourceMileage"><strong>Miles: </strong>{{expense.mileage}}</div>
                        <div *ngIf="expense.departTime"><strong>Time Departed: </strong>{{expense.departTime | date:'shortTime'}}</div>
                        <div *ngIf="expense.returnTime"><strong>Time Returned: </strong>{{expense.returnTime | date:'shortTime'}}</div>
                    </div>

                    <div class="col-sm-6 invoice-col">
                        <p *ngIf="expense.fundingSourceNonMileage">
                            <strong>Expense Funding: </strong><br>{{expense.fundingSourceNonMileage.name}}
                        </p>
                        <div class="row" *ngIf="expense.isOvernight && expense.fundingSourceNonMileage">
                            <div class="col-md-4"><strong>Breakfast: </strong>{{ breakfast(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Lunch: </strong>{{ lunch(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Dinner: </strong>{{ dinner(expense)| currency:'USD':'symbol':'1.2-2'}}</div>
                        </div>
                        <div class="row" *ngIf="expense.fundingSourceNonMileage">
                            <div class="col-md-4"><strong>Lodging: </strong>{{ expense.lodging | currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Registration: </strong>{{ expense.registration | currency:'USD':'symbol':'1.2-2'}}</div>
                            <div class="col-md-4"><strong>Other: </strong>{{ expense.otherExpenseCost| currency:'USD':'symbol':'1.2-2'}}</div>
                        </div>
                        <div *ngIf="expense.otherExpenseCost != 0">{{expense.otherExpenseExplanation}}</div>
                    </div>

                </div>

        </div>
    </div>
    <div *ngIf="!loading && isMileage">
        <div class="col-md-12 col-sm-12 col-xs-12" *ngFor="let expense of monthMileage">
            <div class="ln_solid"></div>
                <div class="row">
                    <div class="col-sm-4">
                        <h3 style="margin-bottom:0;">{{expense.expenseDate| date:'mediumDate'}}</h3>
                        <p *ngIf="expense.isOvernight">Overnight Trip</p>
                        <p *ngIf="!expense.isOvernight">Day Trip</p>
                    </div>
                    <div class="col-sm-8">
                        <p *ngIf="expense.lastRevision.comment != null && expense.lastRevision.comment!=''"><strong>Comment: </strong>{{expense.lastRevision.comment}}</p>
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
    monthMileage: Mileage[];


    isMileage:boolean = false;

    loading = false;

    constructor( 
        private service:ExpenseService,
        private mileageService:MileageService
    )   
    {}

    ngOnInit(){
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        if( this.year.year > 2019 && this.month.month > 10 ) this.isMileage = true;

        this.loading = true;

        if( this.isMileage ){
            this.mileageService.mileagePerMonth(this.month.month, this.year.year, userid, 'asc').subscribe(
                res=> {
                    this.monthMileage = <Mileage[]>res;
                    this.loading = false;
                },
                err => this.errorMessage = <any>err
            );
        }else{
            this.service.expensesPerMonth(this.month.month, this.year.year, userid, 'asc').subscribe(
                res=> {
                    this.monthExpenses = <Expense[]>res;
                    this.loading = false;
                },
                err => this.errorMessage = <any>err
            );
        }
        
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
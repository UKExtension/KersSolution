import { Component, Input } from '@angular/core';
import { ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth, ExpenseSummary } from '../expense.service';
import { saveAs } from 'file-saver';
import { User } from "../../user/user.service";

@Component({
    selector: 'expense-reports-summary',
    template: `
  


<loading *ngIf="loading"></loading>

     <div class="col-md-12 col-sm-12 col-xs-12" *ngIf="!loading">

            <div class="table-responsive">
                <table class="table table-striped" style="background-color: white">
                    <thead>
                        <tr>
                            <th>FUNDING SOURCE</th>
                            <th class="text-right">MILES</th>
                            <th class="text-right">MILEAGE COST</th>
                            <th class="text-right">MEALS</th>
                            <th class="text-right">LODGING</th>
                            <th class="text-right">REGISTRATION</th>
                            <th class="text-right">OTHER</th>
                            <th class="text-right">TOTALS</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr *ngFor="let summary of summaries">
                            <td>{{summary.fundingSource.name}}</td>
                            <td class="text-right">{{summary.miles}}</td>
                            <td class="text-right">{{summary.mileageCost | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.meals | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.lodging | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.registration | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.other | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.total | currency:'USD':'symbol':'1.2-2'}}</td>
                        </tr>
                        
                    </tbody>
                </table>
            </div>
            <div class="ln_solid"></div>
            <!--
            <div>Hours reported on Supplemental Nutrition Assistance Program (SNAP): {{snapHours}}</div>
            -->
            <br><br><br>
                            <table width="80%" style="margin-top: 60px;">
                            <tbody><tr>
                            <td><hr></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><hr></td>
                            </tr>
                            <tr>
                            <td>Signature</td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>Date</td>
                            </tr>
                            </tbody></table>
                            <br><br><br><br>
                            <div class="row">
                                <div class="col-md-6">
                                    <button class="btn btn-primary" style="margin-bottom: 15px;" (click)="print()" *ngIf="!pdfLoading" ><i class="fa fa-download"></i> Detailed Monthly Report</button><br><br>
                                    <loading [type]="'bars'" *ngIf="pdfLoading"></loading>
                                </div>
                                <div class="col-md-6">
                                    <button class="btn btn-primary" style="margin-bottom: 15px;" (click)="printTrip()" *ngIf="!pdfTripLoading" ><i class="fa fa-download"></i> Monthly Mileage Log</button><br><br>
                                    <loading [type]="'bars'" *ngIf="pdfTripLoading"></loading>
                                </div>
                            </div>
                         </div>




        `
})
export class ExpenseReportsSummaryComponent { 


    loading = false;
    pdfLoading = false;
    pdfTripLoading = false;

    errorMessage: string;
    @Input() month;
    @Input() year;
    @Input() user:User;
    userid = 0;
    monthExpenses: Expense[];
    fundingSources: ExpenseFundingSource[];
    mileageRate;
    summaries: ExpenseSummary[] = [];
    snapHours:number = 0;

    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        this.loading = true;
        if(this.user != null){
            this.userid = this.user.id;
        }
/*
        this.service.snapHours(this.month.month, this.userid).subscribe(
            res => {
                if(res != null){
                    this.snapHours = res.totalHours;
                }
            },
            err => this.errorMessage = <any> err
        );
        
*/
        
        this.service.fundingSources().subscribe(
            res => {
                this.fundingSources = <ExpenseFundingSource[]>res;
                this.service.mileageRate(this.month.month, this.year.year, this.userid).subscribe(
                    res => {
                        this.mileageRate = res;
                        this.processExpenses();
                    },
                    err => this.errorMessage = <any> err

                )
                
            },
            err => this.errorMessage = <any> err
        );
        
    }

    processExpenses(){
        this.service.expensesPerMonth(this.month.month,this.year.year, this.userid).subscribe(
            res=> {
                this.monthExpenses = <Expense[]>res;
                this.calculate();
            },
            err => this.errorMessage = <any>err
        );
    }


    calculate(){
        for(let source of this.fundingSources){
            var expensesMileage = this.monthExpenses.filter(e => e.fundingSourceMileageId == source.id);
            var expenseNonMileage = this.monthExpenses.filter( e => e.fundingSourceNonMileageId == source.id);
            this.summarize(source, expensesMileage, expenseNonMileage);
        }
        this.loading = false;
    }   

    print(){
        this.pdfLoading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdf(this.year.year, this.month.month, userid).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ExpensesReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfLoading = false;
            },
            err => console.error(err)
        )
    }

    printTrip(){
        this.pdfTripLoading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdfTrip(this.year.year, this.month.month, userid).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ExpensesMileageReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfTripLoading = false;
            },
            err => console.error(err)
        )
    }


    summarize(fundingSource:ExpenseFundingSource, mileage: Expense[], nonMileage: Expense[]){
        var expenseSourceSummary = <ExpenseSummary> {};
        expenseSourceSummary.fundingSource = fundingSource;
        var miles = 0;
        var milesCost = 0;
        var meals = 0;
        var lodging = 0;
        var registration = 0;
        var other = 0;
        for(let mileageExpenses of mileage){
            miles += mileageExpenses.mileage;
        }
        milesCost = (miles * this.mileageRate);
        for(let nonMileageExpenses of nonMileage){
            lodging += nonMileageExpenses.lodging;
            registration += nonMileageExpenses.registration;
            other += nonMileageExpenses.otherExpenseCost;
            meals += this.breakfast(nonMileageExpenses);
            meals += this.lunch(nonMileageExpenses);
            meals += this.dinner(nonMileageExpenses);
        }
        expenseSourceSummary.miles = miles;
        expenseSourceSummary.mileageCost = milesCost;
        expenseSourceSummary.lodging = lodging;
        expenseSourceSummary.registration = registration;
        expenseSourceSummary.other = other;
        expenseSourceSummary.meals = meals;
        expenseSourceSummary.total = lodging + milesCost + meals + registration + other;
        if(expenseSourceSummary.total != 0){
            this.summaries.push(expenseSourceSummary);
        }
    }



    breakfast(expense:Expense){
        if(expense.mealRateBreakfast != null){
            return expense.mealRateBreakfast.breakfastRate;
        }else if(expense.mealRateBreakfastId == 0){
            return (expense.mealRateBreakfastCustom == undefined ? 0 : expense.mealRateBreakfastCustom );
        }
        return 0;
    }
    lunch(expense:Expense){
        if(expense.mealRateLunch != null){
            return expense.mealRateLunch.lunchRate;
        }else if(expense.mealRateLunchId == 0){
            return (expense.mealRateLunchCustom == undefined ? 0 : expense.mealRateLunchCustom );
        }
        return 0;
    }
    dinner(expense:Expense){
        if(expense.mealRateDinner != null){
            return expense.mealRateDinner.dinnerRate;
        }else if(expense.mealRateDinnerId == 0){
            return (expense.mealRateDinnerCustom == undefined ? 0 : expense.mealRateDinnerCustom );
        }
        return 0;
    }

}



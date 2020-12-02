import { Component, Input } from '@angular/core';
import { ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth, ExpenseSummary } from '../expense.service';
import { saveAs } from 'file-saver';
import { User } from "../../user/user.service";
import { FiscalyearService } from '../../admin/fiscalyear/fiscalyear.service';
import { MileageService } from '../../mileage/mileage.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'expense-reports-summary',
    template: `
  


<loading *ngIf="loading"></loading>

     <div class="col-md-12 col-sm-12 col-xs-12" *ngIf="!loading">

            <div class="table-responsive" *ngIf="!isMileage">
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
                            <th class="text-right">MTD TOTALS</th>
                            <th class="text-right">YTD</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr *ngFor="let summary of summaries">
                            <td>{{summary.fundingSource.name}}</td>
                            <td class="text-right">{{summary.miles | number:'0.0-2'}}</td>
                            <td class="text-right">{{summary.mileageCost | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.meals | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.lodging | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.registration | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.other | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.total | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right" *ngIf="fiscalYearSummaries">{{ytd(summary.fundingSource.id) | currency:'USD':'symbol':'1.2-2'}}</td>
                        </tr>
                        <tr *ngFor="let blank of blankRows">
                            <td>{{blank.fundingSource.name}}</td>
                            <td class="text-right">0</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">$0.00</td>
                            <td class="text-right">{{blank.total | currency:'USD':'symbol':'1.2-2'}}</td>
                        </tr>
                        
                    </tbody>
                </table>
            </div>
        </div><br>
        <div class="table-responsive" *ngIf="isMileage"><br>
                <table class="table table-striped" style="background-color: white" *ngIf="(mileageSummary$ | async) as mileageSummary">
                    <thead>
                        <tr>
                            <th>FUNDING SOURCE</th>
                            <th class="text-right">MILES</th>
                            <th class="text-right">COST PER MILE</th>
                            <th class="text-right">TOTAL</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr *ngFor="let summary of mileageSummary">
                            <td>{{summary.fundingSource.name}}</td>
                            <td class="text-right">{{summary.miles}}</td>
                            <td class="text-right">{{summary.mileageCost | currency:'USD':'symbol':'1.2-2'}}</td>
                            <td class="text-right">{{summary.total | currency:'USD':'symbol':'1.2-2'}}</td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td><strong>Total:</strong></td>
                            <td class="text-right">{{mileageToatl(mileageSummary)}}</td>
                            <td>&nbsp;</td>
                            <td class="text-right">{{total(mileageSummary) | currency:'USD':'symbol':'1.2-2'}}</td>
                        </tr>
                    </tfoot>
                </table>
<br><br>

        </div>


        `
})
export class ExpenseReportsSummaryComponent { 


    loading = false;
    pdfLoading = false;
    pdfTripLoading = false;
    pdfTripLoadingOvernight = false;

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

    fiscalYearSummaries: ExpenseSummary[];
    blankRows: ExpenseSummary[];


    mileageSummary$:Observable<ExpenseSummary[]>;
    isMileage:boolean = false;

    constructor( 
        private service:ExpenseService,
        private mileageService: MileageService,
        private fiscalYearService: FiscalyearService
    )   
    {}

    ngOnInit(){
        
        if(this.user != null){
            this.userid = this.user.id;
        }
        if( this.year.year > 2019 && this.month.month > 10 ) this.isMileage = true;
        if(this.isMileage){
            this.mileageSummary$ = this.mileageService.summaryPerMonth(this.month.month, this.year.year, this.userid);
        }else{
            this.loading = true;
            this.fiscalYearService.forDate( new Date(this.year.year, this.month.month - 1, 15) )
                .subscribe(
                    res => {
                        var fiscalYear = res;
                        
                        this.service.SummariesPerPeriod( fiscalYear.start, new Date(this.year.year, this.month.month, 0,23, 59, 59), this.userid)
                            .subscribe(
                                res =>
                                {
                                    this.fiscalYearSummaries = res;
                                    if(this.loading == true ){
                                        this.getBlankRows();
                                        this.loading = false;
                                    }
                                }
                            )
                    
                    }
                )



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
       
        if( this.fiscalYearSummaries != null){
            this.getBlankRows();
            this.loading = false;
        }
        
    }
    
    getBlankRows(){
        this.blankRows = [];
        for( let row of this.fiscalYearSummaries){
            if( this.summaries.filter( s => s.fundingSource.id == row.fundingSource.id ).length == 0 ){
                this.blankRows.push(row);
            }
        }
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

    printTrip(isOvernight:boolean = false){
        if( isOvernight )
        {
            this.pdfTripLoadingOvernight = true;
        }else{
            this.pdfTripLoading = true;
        }
        
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdfTrip(this.year.year, this.month.month, userid, isOvernight).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ExpensesMileageReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfTripLoading = false;
                this.pdfTripLoadingOvernight = false;
            },
            err => console.error(err)
        )
    }

    mileageToatl(summary:ExpenseSummary[]):number{
        var ttl = 0;
        for( let smr of summary){
            ttl += smr.miles;
        }
        return ttl;
    }
    total(summary:ExpenseSummary[]):number{
        var ttl = 0;
        for( let smr of summary){
            ttl += smr.total;
        }
        return ttl;
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

    ytd( fundingSourceId:number ){
        var summary = this.fiscalYearSummaries.filter( s => s.fundingSource.id == fundingSourceId );
        if( summary.length > 0 ){
            return summary[0].total;
        }else{
            return 0;
        }
        
    }

}



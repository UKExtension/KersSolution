import { Component, Input } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { User } from "../../user/user.service";
import { Mileage, MileageSegment } from '../../mileage/mileage';
import { MileageService } from '../../mileage/mileage.service';
import { ProgramCategory } from '../../admin/programs/programs.service';

@Component({
    selector: 'expense-reports-details',
    template: `
<loading *ngIf="loading"></loading>
<div *ngIf="!loading && !isMileage">
    <expense-reports-details-item [expense]="expense" *ngFor="let expense of monthExpenses"></expense-reports-details-item>
</div>
<div *ngIf="!loading && isMileage">
    <mileage-reports-details-item [sources]="sources" [categories]="categories" [expense]="expense" *ngFor="let expense of monthMileage"></mileage-reports-details-item>
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

    categories:ProgramCategory[];
    sources:ExpenseFundingSource[];


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
        if( (this.year.year > 2019 && this.month.month > 10) || this.year.year > 2020 ) this.isMileage = true;

        this.loading = true;
        
        if( this.isMileage ){
            this.mileageService.mileagePerMonth(this.month.month, this.year.year, userid, 'asc').subscribe(
                res=> {
                    this.monthMileage = <Mileage[]>res;
                    this.loading = false;
                },
                err => this.errorMessage = <any>err
            );
            this.mileageService.categories().subscribe(
                res => this.categories = res
            );
            this.mileageService.sources().subscribe(
                res => this.sources = res
            )
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
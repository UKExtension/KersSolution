import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from './expense.service';

import { Router } from "@angular/router";

@Component({
  template: `
        <div>
            <div class="text-right">
                <a class="btn btn-info btn-xs" *ngIf="!newExpense" (click)="newExpense = true">+ new expense record</a>
            </div>
            <expense-form *ngIf="newExpense" (onFormCancel)="newExpense=false" (onFormSubmit)="newExpenseSubmitted($event)"></expense-form>
        </div>
    <br><expense-list [byMonth]="byMonth" (onDeleted)="deleted($event)" (onEdited)="edited($event)"></expense-list>
    <div *ngIf="numbExpenses != 0" class="text-center">
        <div>Showing {{latest.length}} of {{numbExpenses}} expense records</div>
        <div *ngIf="latest.length < numbExpenses" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
            load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
    </div>`
})
export class ExpenseHomeComponent { 

    latest:Expense[] = [];
    numbExpenses:number = 0;
    newExpense=false;
    byMonth:ExpenseMonth[] = [];
    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private router: Router,
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
        this.service.latest().subscribe(
            res=>{
                    this.latest = <Expense[]>res; 
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
        this.service.num().subscribe(
            res => {
                this.numbExpenses = <number>res;
            },
            err => this.errorMessage = <any>err
        );
        
        
    }
    loadMore(){
        var lt = this.latest;
        this.service.latest(this.latest.length, 2).subscribe(
            res=>{
                    var batch = <Expense[]>res; 
                    batch.forEach(function(element){
                        lt.push(element);
                    });
                    this.byMonth = [];
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
    }
    populateByMonth(){
        var bm = this.byMonth;
        this.latest.forEach(function(element){
            
                var exDt = new Date(element.expenseDate);
                var exMonth = bm.filter(f=>f.month==exDt.getMonth() && f.year == exDt.getFullYear());
                if(exMonth.length == 0){
                    var ob = <ExpenseMonth>{
                        month : exDt.getMonth(),
                        year : exDt.getFullYear(),
                        date: exDt,
                        expenses : [element]
                    };
                    bm.push(ob);
                }else{
                    exMonth[0].expenses.push(element);
                }
            }); 
    }


    newExpenseSubmitted(expense:Expense){
        this.newExpense=false;
        this.latest.unshift(expense);
        this.byMonth = [];
        this.populateByMonth();
        this.numbExpenses++;
    }
    
    deleted(expense:Expense){
        let index: number = this.latest.indexOf(expense);
        if (index !== -1) {
            this.latest.splice(index, 1);
            this.numbExpenses--;
        }
        this.byMonth = [];
        this.populateByMonth();
    }

    edited(expense:Expense){

        this.latest = this.latest.map(function(item) { return item.expenseId == expense.expenseId ? expense : item; });
        this.latest.sort(
                    function(a, b) {

                        var dateA = new Date(a.expenseDate);
                        var dateB = new Date(b.expenseDate);
                        if( dateA  > dateB ){
                            return -1;
                        }else{
                            return 1;
                        }
                    }
                 );
        this.byMonth = [];
        this.populateByMonth();
    }

    defaultTitle(){
        this.reportingService.setTitle("Expense Records");
    }
}
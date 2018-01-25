import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { saveAs } from 'file-saver';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { User } from "../../user/user.service";

@Component({
    selector: 'expense-reports-year',
    template: `
    <div class="panel-year">
        <a class="panel-heading" (click)="toggle()">{{year.year}}</a>
        <div class="panel-collapse" *ngIf="condition">
            <expense-reports-month *ngFor="let month of months | async; let i = index" [month]="month" [index]="i" [year]="year" [user]="user"></expense-reports-month>
        </div>
    </div>
        `,
    styles: [`
        .panel-year{
            border-bottom: 1px solid #efefef;
        }
        .panel-heading{
            cursor:pointer;
        }
    `]
})
export class ExpenseReportsYearComponent { 


    errorMessage: string;

    months:Observable<string[]>

    condition = false;

    @Input() year;
    @Input() index;
    @Input() user:User;

    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        if(this.user == null){
            this.months = this.service.monthsWithExpenses(this.year.year);
        }else{
            this.months = this.service.monthsWithExpenses(this.year.year, this.user.id);
        }
        if(this.index == 0){
            this.toggle();
        }
    }

    toggle(){
        if(this.condition){
            this.close();
        }else{
            this.open();
        }
    }
    open(){
        this.condition = true;
    }
    close(){
        this.condition = false;
    }
    

}
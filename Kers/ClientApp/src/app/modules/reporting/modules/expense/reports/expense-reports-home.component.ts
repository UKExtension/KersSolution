import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { saveAs } from 'file-saver';

import { Router } from "@angular/router";
import { User } from "../../user/user.service";

@Component({
    selector: 'user-expenses',
    template: `
        
        <div class="accordion">
            <expense-reports-year *ngFor="let year of years | async; let i = index" [year]="year" [index]="i" [user]="user"></expense-reports-year>
        
        </div><loading *ngIf="!(years | async)"></loading><br><br>
        <div class="text-right" *ngIf="!user"><a class="btn btn-default btn-xs" href="https://kers.ca.uky.edu/kers_mobile/ReportExpenseMain.aspx">Expense Reports Archive</a></div>
        `
})
export class ExpenseReportsHomeComponent { 

    @Input() user:User;

    errorMessage: string;

    years;

    constructor( 
        private reportingService: ReportingService,
        private router: Router,
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }else{
            this.defaultTitle();
        }
        this.years = this.service.yearsWithExpenses(userid);
        
        
    }


    defaultTitle(){
        this.reportingService.setTitle("Mileage Records Reports");
    }
}
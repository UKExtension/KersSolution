import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';
import { saveAs } from 'file-saver';

import { Router } from "@angular/router";
import { User } from "../../user/user.service";

@Component({
    selector: 'user-expenses',
    template: `

<div class="accordion">
  @for (year of years | async; track year; let i = $index) {
    <expense-reports-year [year]="year" [index]="i" [user]="user"></expense-reports-year>
  }

</div>@if (!(years | async)) {
<loading></loading>
}<br><br>
@if (!user) {
  <div class="text-right"><a class="btn btn-default btn-xs" href="https://kers.ca.uky.edu/kers_mobile/ReportExpenseMain.aspx">Expense Reports Archive</a></div>
}
`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
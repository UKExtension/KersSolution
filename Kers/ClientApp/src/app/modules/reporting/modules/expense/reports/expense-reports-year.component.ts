import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {ExpenseService} from '../expense.service';
import { Observable } from "rxjs";
import { User } from "../../user/user.service";

@Component({
    selector: 'expense-reports-year',
    template: `
    <div class="panel-year">
      <a class="panel-heading" (click)="toggle()">{{year.year}}</a>
      @if (condition) {
        <div class="panel-collapse">
          @for (month of months | async; track month; let i = $index) {
            <expense-reports-month [month]="month" [index]="i" [year]="year" [user]="user"></expense-reports-month>
          }
        </div>
      }
    </div>
    `,
    styles: [`
        .panel-year{
            border-bottom: 1px solid #efefef;
        }
        .panel-heading{
            cursor:pointer;
        }
    `],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
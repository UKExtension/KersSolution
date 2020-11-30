import { Component, Input, OnInit } from '@angular/core';
import { Expense } from '../expense.service';

@Component({
  selector: 'expense-reports-details-item',
  template: `
  <div class="col-md-12 col-sm-12 col-xs-12">
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
  `,
  styles: []
})
export class ExpenseReportsDetailsItemComponent implements OnInit {
  @Input() expense:Expense;

  constructor() { }

  ngOnInit() {
  }

}

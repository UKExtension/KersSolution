import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BudgetPlanOfficeOperation } from '../budget.service';

@Component({
  selector: 'budget-plan-office-operations-detail',
  template: `

  <div class="ln_solid"></div>
  <div class="row">
      <div class="col-xs-10">
          
          <article class="media event" *ngIf="rowDefault">
              <div class="media-body">
              <a class="title">{{operation.name}}</a>
              <p>Order: {{operation.order}}</p>
              </div>
          </article>
          <div class="col-xs-12" *ngIf="rowEdit">
              <budget-plan-office-operations-form [operation]="operation" (onFormCancel)="default()" (onFormSubmit)="operationSubmitted($event)"></budget-plan-office-operations-form>
          </div>

          
      </div>
      <div class="col-xs-2 text-right">
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault">edit</a>
          <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
      </div>  
  </div>





  `,
  styles: []
})
export class BudgetPlanOfficeOperationsDetailComponent implements OnInit {
  
  @Input() operation: BudgetPlanOfficeOperation;

  rowDefault = true;
  rowEdit = false;

  @Output() onEdited = new EventEmitter<BudgetPlanOfficeOperation>();

  constructor() { }

  ngOnInit() {
  }

  edit(){
    this.rowEdit = true;
    this.rowDefault = false;
  }
  default(){
    this.rowDefault = true;
    this.rowEdit = false;
  }

  operationSubmitted(event:BudgetPlanOfficeOperation){
    this.onEdited.emit(event);
  }

}

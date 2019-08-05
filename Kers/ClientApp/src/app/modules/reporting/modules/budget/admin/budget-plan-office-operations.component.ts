import { Component, OnInit } from '@angular/core';
import { BudgetPlanOfficeOperation, BudgetService } from '../budget.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'budget-plan-office-operations',
  template: `<br>
    <h3>Office Operations</h3>
    <br>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newOperation" (click)="newOperation = true">+ new office operation</a>
    </div>
    <budget-plan-office-operations-form *ngIf="newOperation" (onFormCancel)="newOperation=false" (onFormSubmit)="newOperationSubmitted($event)"></budget-plan-office-operations-form>
    <budget-plan-office-operations-detail *ngFor="let operation of officeoperatoins | async" [operation]="operation"></budget-plan-office-operations-detail>
   

  `,
  styles: []
})
export class BudgetPlanOfficeOperationsComponent implements OnInit {

  officeoperatoins:Observable<BudgetPlanOfficeOperation[]>;
  newOperation = false;

  constructor(
    private service:BudgetService
  ) {
    
   }

  ngOnInit() {
    this.officeoperatoins = this.service.getOfficeOperations();
  }

  newOperationSubmitted(_:BudgetPlanOfficeOperation){
    this.newOperation = false;
    this.officeoperatoins = this.service.getOfficeOperations();
  }

}

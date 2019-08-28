import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BudgetPlanUserDefinedIncome, BudgetPlanStaffExpenditure, BudgetPlanTravelExpenses, BudgetPlanProfessionalImprovementExpenses } from '../budget.service';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { User } from '../../user/user.service';



@Component({
  selector: 'budget-professional-improvement-expenses',
  template: `
<div class="form-group" [formGroup]="budgetImprovement">
  <div class="form-inline">
      <div class="form-group">
        <select formControlName="staffTypeId" class="form-control">
          <option value="0">-- Select --</option>
          <option value="1">Base Agent/ANR</option>
          <option value="2">Base Agent/FCS</option>
          <option value="3">Base Agent/4H</option>
          <option value="4">Agent (4th+)</option>
          <option value="5">Support Staff</option>
        </select><br>
        <small>(Position)</small>
      </div>
      &nbsp;
      <div class="form-group">
        <select formControlName="personId" class="form-control">
          <option value="0">-- Select --</option>
          <option *ngFor="let employee of employees" [value]="employee.id">{{employee.personalProfile.firstName}} {{employee.personalProfile.lastName}}</option>
          <option disabled>──────────</option>
          <option [ngValue]="null">Other</option>
        </select><br>
        <small *ngIf="budgetTravel.controls.personId.value != null">(Person)</small>
        <input *ngIf="budgetTravel.controls.personId.value == null" type="text" style="margin-top: 3px;" class="form-control" formControlName="personNameIfNotAUser" /><br>
          <small  *ngIf="budgetTravel.controls.personId.value == null">(Other Person Name)</small>
      </div>
      &nbsp;
      <div class="form-group">
          <input type="number" class="form-control col-md-3 col-xs-6" formControlName="amount"/><br>
          <small>(Amount)</small>
      </div>
      <div class="form-group">
              <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="cursor:pointer;"><i class="fa fa-close"></i></a></span></div>
              <br>
          <small>&nbsp;</small>
      </div>
  </div>
</div>

  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => BudgetPlanProfessionalImprovementExpensesComponent),
                  multi: true
                } 
                ]
})
export class BudgetPlanProfessionalImprovementExpensesComponent extends BaseControlValueAccessor<BudgetPlanProfessionalImprovementExpenses> implements ControlValueAccessor, OnInit { 
  budgetImprovement: FormGroup;
    @Input('type') type:number;
    @Input('rate') rate:number;
    @Input('index') index:number;
    @Input('employees') employees:User[];

    @Output() removeMe = new EventEmitter<number>();
    
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      this.budgetImprovement = formBuilder.group({
        personId: [0],
        personNameIfNotAUser: "",
        staffTypeId: 0,
        amount: "",
        index: 0
      });
  
      this.budgetImprovement.valueChanges.subscribe(val => {
        this.value = <BudgetPlanProfessionalImprovementExpenses>val;
        this.onChange(this.value);
      });
    }
    
    ngOnInit(){
        
    }

    writeValue(expenditure: BudgetPlanProfessionalImprovementExpenses) {
      this.value = expenditure;
      this.budgetImprovement.patchValue(expenditure);
    }

    onRemove(){
      this.removeMe.emit(this.value.index);
  } 



}
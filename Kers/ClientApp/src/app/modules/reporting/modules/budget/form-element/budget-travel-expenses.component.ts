import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BudgetPlanUserDefinedIncome, BudgetPlanStaffExpenditure, BudgetPlanTravelExpenses } from '../budget.service';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { User } from '../../user/user.service';



@Component({
  selector: 'budget-travel-expenses',
  template: `
<div class="form-group" [formGroup]="budgetTravel">
  <div class="form-inline">
      
      <div class="form-group">
        <label>Position</label><br>
        <select formControlName="staffTypeId" class="form-control">
          <option value="0">-- Select --</option>
          <option value="1">Base Agent/ANR</option>
          <option value="2">Base Agent/FCS</option>
          <option value="3">Base Agent/4H</option>
          <option value="4">Agent (4th+)</option>
          <option value="5">Support Staff</option>
        </select><br><br>
      </div>
      &nbsp;
      <div class="form-group">
        <label>Person</label><br>
        <select formControlName="personId" class="form-control">
          <option value="0">-- Select --</option>
          <option *ngFor="let employee of employees" [value]="employee.id">{{employee.personalProfile.firstName}} {{employee.personalProfile.lastName}}</option>
          <option disabled>──────────</option>
          <option [ngValue]="null">Other</option>
        </select><br>
        <input *ngIf="budgetTravel.controls.personId.value == null" type="text" style="margin-top: 3px;" class="form-control" formControlName="personNameIfNotAUser" /><br>
          <small  *ngIf="budgetTravel.controls.personId.value == null">(Other Person Name)</small>
      </div>
      &nbsp;
      <div class="form-group">
        <label>Travel</label><br>
        <input type="number" class="form-control col-md-3 col-xs-6" formControlName="amount"/><br>
        <small>(Amount)</small>
      </div>&nbsp;
      <div class="form-group">
        <label>Professional Improvement</label><br>
        <input type="number" class="form-control col-md-3 col-xs-6" /><br>
        <small>(Amount)</small>
      </div>&nbsp;
      <div class="form-group">
        <label>Program Support</label><br>
        <input type="number" class="form-control col-md-3 col-xs-6" /><br>
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
                  useExisting: forwardRef(() => BudgetTravelExpensesComponent),
                  multi: true
                } 
                ]
})
export class BudgetTravelExpensesComponent extends BaseControlValueAccessor<BudgetPlanTravelExpenses> implements ControlValueAccessor, OnInit { 
  budgetTravel: FormGroup;
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
      this.budgetTravel = formBuilder.group({
        personId: [0],
        personNameIfNotAUser: "",
        staffTypeId: 0,
        amount: "",
        index: 0
      });
  
      this.budgetTravel.valueChanges.subscribe(val => {
        this.value = <BudgetPlanTravelExpenses>val;
        this.onChange(this.value);
      });
    }
    
    ngOnInit(){
        
    }

    writeValue(expenditure: BudgetPlanTravelExpenses) {
      this.value = expenditure;
      this.budgetTravel.patchValue(expenditure);
    }

    onRemove(){
      this.removeMe.emit(this.value.index);
  } 



}
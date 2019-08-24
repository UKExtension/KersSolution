import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BudgetPlanUserDefinedIncome, BudgetPlanStaffExpenditure } from '../budget.service';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { User } from '../../user/user.service';



@Component({
  selector: 'budget-personel-support-staff',
  template: `
<div class="form-group" [formGroup]="budgetStaffExpenditure">
  <div class="form-inline">
      <div class="form-group">
        <select formControlName="personId" class="form-control">
          <option value="0">-- Select --</option>
          <option *ngFor="let employee of employees" [value]="employee.id">{{employee.personalProfile.firstName}} {{employee.personalProfile.lastName}}</option>
          <option disabled>──────────</option>
          <option value="null">Other</option>
        </select><br>
        <small *ngIf="budgetStaffExpenditure.controls.personId.value != 'null'">(Person)</small>
        <input *ngIf="budgetStaffExpenditure.controls.personId.value == 'null'" type="text" style="margin-top: 3px;" class="form-control" formControlName="personNameIfNotAUser" /><br>
          <small  *ngIf="budgetStaffExpenditure.controls.personId.value == 'null'">(Other Person Name)</small>
      </div>
      &nbsp;
      <div class="form-group">
          <input type="number" class="form-control col-md-3 col-xs-6" formControlName="hourlyRate"/><br>
          <small>(Hourly Rate)</small>
      </div>
      &nbsp;
      <div class="form-group">
          <input type="number" class="form-control col-md-3 col-xs-6" formControlName="hoursPerWeek"/><br>
          <small>(Hours per Week)</small>
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
                  useExisting: forwardRef(() => BudgetPersonelSupportStaffComponent),
                  multi: true
                } 
                ]
})
export class BudgetPersonelSupportStaffComponent extends BaseControlValueAccessor<BudgetPlanStaffExpenditure> implements ControlValueAccessor, OnInit { 
    budgetStaffExpenditure: FormGroup;
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
      this.budgetStaffExpenditure = formBuilder.group({
        personId: [0, Validators.required],
        personNameIfNotAUser: "",
        hourlyRate: "",
        hoursPerWeek: "",
        benefitRateInPercents: this.rate,
        expenditureType: this.type,
        index: 0
      });
  
      this.budgetStaffExpenditure.valueChanges.subscribe(val => {
        this.value = <BudgetPlanStaffExpenditure>val;
        this.onChange(this.value);
      });
    }
    
    ngOnInit(){
        
    }

    totalWithoutBenefits():number{
      return this.value.hourlyRate * this.value.hoursPerWeek * 52;
    }

    writeValue(expenditure: BudgetPlanStaffExpenditure) {
      this.value = expenditure;
      this.budgetStaffExpenditure.patchValue(expenditure);
    }

    onRemove(){
      this.removeMe.emit(this.value.index);
  } 



}
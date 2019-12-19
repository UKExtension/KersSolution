import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { BudgetPlanUserDefinedIncome } from '../budget.service';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';



@Component({
  selector: 'budget-user-defined-income',
  template: `
<div class="form-group" [formGroup]="budgetUserDefinedIncomeGroup">
  <div class="form-inline">
      <div class="form-group">
          <input type="text" class="form-control" formControlName="name" /><br>
          <small>(Source of Income)</small>
      </div>
      &nbsp;
      <div class="form-group">
          <input type="number" class="form-control col-md-3 col-xs-6" formControlName="value"/><br>
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
                  useExisting: forwardRef(() => BudgetUserDefinedIncomeComponent),
                  multi: true
                } 
                ]
})
export class BudgetUserDefinedIncomeComponent extends BaseControlValueAccessor<BudgetPlanUserDefinedIncome> implements ControlValueAccessor, OnInit { 
    budgetUserDefinedIncomeGroup: FormGroup;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();
    
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      this.budgetUserDefinedIncomeGroup = formBuilder.group({
        name: [''], 
        value: ['']
      });
  
      this.budgetUserDefinedIncomeGroup.valueChanges.subscribe(val => {
        this.value = <BudgetPlanUserDefinedIncome>val;
        this.onChange(this.value);
      });
    }
    onRemove(){
        this.removeMe.emit(this.index);
    } 

    ngOnInit(){
        
    }
    writeValue(income: BudgetPlanUserDefinedIncome) {
      this.value = income;
      this.budgetUserDefinedIncomeGroup.patchValue(income);
    }
}
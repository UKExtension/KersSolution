import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, FormArray } from '@angular/forms';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { BudgetPlanOfficeOperationValue, BudgetService, BudgetPlanOfficeOperation } from '../budget.service';



@Component({
  selector: 'office-operations',
  template: `
  <div [formGroup]="officeOperations">
    <div *ngIf="operations" formArrayName="officeOperationValues">
      <br>
      <div class="form-group" *ngFor="let operation of operations; let i = index" >
          <label class="control-label col-md-3 col-sm-3 col-xs-6">{{operation.name}}
          </label>
          <div class="col-md-8 col-sm-9 col-xs-6" [formGroupName]="i">
            <input type="number" class="form-control col-md-3 col-xs-6" formControlName="value">
            <input type="hidden" [value]="operation.id" formControlName="budgetPlanOfficeOperationId" >
          </div>
      </div>
    </div>
  </div>


  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => OfficeOperationsComponent),
                  multi: true
                } 
                ]
})
export class OfficeOperationsComponent extends BaseControlValueAccessor<BudgetPlanOfficeOperationValue[]> implements ControlValueAccessor, OnInit { 
    officeOperations: FormGroup;
    writtenValues:BudgetPlanOfficeOperationValue[];
    operations:BudgetPlanOfficeOperation[];
    constructor( 
      private formBuilder: FormBuilder,
      private service:BudgetService,
    )   
    {
      super();
      this.officeOperations = formBuilder.group(
        {
          officeOperationValues: formBuilder.array([])
        }
      )
      
      formBuilder.array([]);
  
      this.officeOperations.valueChanges.subscribe(val => {
        this.value = val.officeOperationValues;
        this.onChange(this.value);
      });
    }
    

    ngOnInit(){
      this.service.getOfficeOperations().subscribe(
        res => {
          let control = <FormArray>this.officeOperations.get("officeOperationValues");
          for( let val of res ){
            control.push(this.formBuilder.group({
              //budgetPlanOfficeOperation: val,
              budgetPlanOfficeOperationId: val.id,
              value: 0
            }));
          }
          console.log(this.writtenValues);
          control.patchValue(this.writtenValues);
          this.operations = res;
        }
      );
    }
    writeValue(operationValues: BudgetPlanOfficeOperationValue[]) {
      
      this.value = operationValues;
      this.writtenValues = operationValues;
      this.officeOperations.get("officeOperationValues").patchValue(operationValues);
    }
    
}
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BudgetPlanOfficeOperation, BudgetService } from '../budget.service';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'budget-plan-office-operations-form',
  template: `
    <loading *ngIf="loading"></loading>
    <div class="row" *ngIf="!loading && !proposed">
        <div class="col-sm-offset-3 col-sm-9">
            <h2 *ngIf="!operation">New Office Operation</h2>
            <h2 *ngIf="operation">Update Office Operation</h2>
        </div>

        <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="officeOperationsForm">
            <div class="form-group">
                <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">Name:</label>           
                <div class="col-md-9 col-sm-9 col-xs-12">
                    <input type="text" name="name" formControlName="name" id="name" class="form-control col-xs-12" />
                </div>
            </div>
            <div class="form-group">
                <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">Order:</label>           
                <div class="col-md-3 col-sm-3 col-xs-4">
                    <input type="number" name="order" formControlName="order" id="order" class="form-control col-xs-12" />
                </div>
            </div>
            <div class="form-group">
                <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">Active:</label>           
                <div class="col-md-9 col-sm-9 col-xs-12">
                    <input type="checkbox" name="active" formControlName="active" id="active" class="form-control" />
                </div>
            </div>
            <div class="ln_solid"></div>
            <div class="form-group">
                <div class="col-md-6 col-sm-6 col-xs-12 col-sm-offset-3">
                    <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
                    <button type="submit" [disabled]="officeOperationsForm.invalid"  class="btn btn-success">Submit</button>
                </div>
            </div>
              
        </form>



    </div>
  `,
  styles: []
})
export class BudgetPlanOfficeOperationsForm implements OnInit {
    
    @Input() operation:BudgetPlanOfficeOperation;

    officeOperationsForm:any;

    loading = false;
    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<BudgetPlanOfficeOperation>();

    constructor(
        private fb: FormBuilder,
        private service:BudgetService
    ) {
        this.officeOperationsForm = this.fb.group(
            { 
                name: ["", Validators.required],
                order: 0,
                active: true
            });

    }

    ngOnInit() {
        if(this.operation) this.officeOperationsForm.patchValue(this.operation);
    }

    onCancel(){
        this.onFormCancel.emit();
    }

    onSubmit(){
        if(!this.operation){
            var newOperation:BudgetPlanOfficeOperation = this.officeOperationsForm.value;
            console.log( newOperation );
            this.service.add( newOperation ).subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                }
            )
        }else{
            var newOperation:BudgetPlanOfficeOperation = this.officeOperationsForm.value;
            console.log( newOperation );
            this.service.update(this.operation.id,  newOperation ).subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                }
            )
        }
        
    }

}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormControl } from "@angular/forms";
import { SnapedAdminService } from './snaped-admin.service';



@Component({
    selector: 'snaped-reimbursment-form',
    template: `
        <div class="col-sm-offset-3 col-sm-9">
            <h2 *ngIf="!reimbursment">New Reimbursment</h2>
            <h2 *ngIf="reimbursment">Edit Reimbursment</h2>
        </div>
        <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="reimbursmentForm">
            <div class="form-group">
                <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Amount:</label>           
                <div class="col-md-9 col-sm-9 col-xs-12">
                    <input type="number" name="amount" formControlName="amount" id="notes" class="form-control" style="width: 150px;" />
                </div>
            </div>
            <div class="form-group">
                <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Notes:</label>           
                <div class="col-md-9 col-sm-9 col-xs-12">
                    <input type="text" name="notes" formControlName="notes" id="notes" class="form-control col-xs-12" />
                </div>
            </div>
            <div class="ln_solid"></div>
            <div class="form-group">
                <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
                    <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
                    <button type="submit" [disabled]="reimbursmentForm.invalid"  class="btn btn-success">Submit</button>
                </div>
            </div>
        </form>
    `
})
export class SnapedReimbursmentFormComponent implements OnInit{ 

    @Input() reimbursment = null;

    @Input() userId;
    @Input() countyId;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<{}>();

    loading = true;
    reimbursmentForm = null;

    errorMessage;

    constructor( 
        private fb: FormBuilder,
        private service: SnapedAdminService
    )   
    {
        this.reimbursmentForm = this.fb.group(
            {
              amount: ["", [this.isIntOrFloat, Validators.required]],
              notes: ["", Validators.required]
            });
        

    }

    ngOnInit(){
        if(this.reimbursment != null){
            this.reimbursmentForm.patchValue(this.reimbursment);
        }
        
    }


        


    onSubmit(){

        if(this.reimbursment == null){
            if(this.countyId != null){
                this.service.addCountyReimbursment(this.countyId, this.reimbursmentForm.value).subscribe(
                    res => {
                        this.onFormSubmit.emit(res);
                    },
                    err => this.errorMessage = <any> err
                );
            }else if(this.userId != null){
                this.service.addAssistantReimbursment(this.userId, this.reimbursmentForm.value).subscribe(
                    res => {
                        this.onFormSubmit.emit(res);
                    },
                    err => this.errorMessage = <any> err
                );
            }
            
        }else{
            if(this.reimbursment.toId == null){
                this.service.editCountyReimbursment( this.reimbursment.id, this.reimbursmentForm.value ).subscribe(
                    res=>{
                        this.onFormSubmit.emit(res);
                    },
                    err => this.errorMessage = <any> err
                );
            }else{
                this.service.editAssistantReimbursment( this.reimbursment.id, this.reimbursmentForm.value ).subscribe(
                    res=>{
                        this.onFormSubmit.emit(res);
                    },
                    err => this.errorMessage = <any> err
                );
            }
            
        }
        
        

    }

    onCancel(){
        this.onFormCancel.emit();
    }

    /************************
      
      Validators
    
     ***********************/

    isIntOrFloat(control:FormControl){
        if(control.value == +control.value && +control.value >= 0){
            return null;
        }
        return {"notDigit":true};
      }
  
      isPositiveInt(control:FormControl){
          
          if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
              return null;
          }
          return {"notInt":true};
      }
}

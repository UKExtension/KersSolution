import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FiscalyearService, FiscalYear } from './fiscalyear.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';

@Component({
    selector: 'fiscalyear-form',
    templateUrl: 'fiscalyear-form.component.html' 
})
export class FiscalyearFormComponent implements OnInit{

    fiscalyearForm = null;
    @Input() fiscalyear:FiscalYear = null;
    errorMessage: string;
    public options: Object;
    startValue:Date;
    endValue:Date;
    availableatValue:Date;
    extendedtoValue:Date;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: FiscalyearService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location
    ){

        

        this.fiscalyearForm = fb.group(
            {
              start: [''],
              end: [''],
              availableAt: [''],
              extendedTo: [''],
              name: ['', Validators.required],
              type: ['', Validators.required]
            }
        );
    }
   
    ngOnInit(){
       if(this.fiscalyear){
           this.fiscalyearForm.patchValue(this.fiscalyear);
           if(  Date.parse(  this.fiscalyear.start.toString() ) > 0){
                this.startValue = new Date(this.fiscalyear.start);
           }
           if(  Date.parse(  this.fiscalyear.end.toString() ) > 0){
                this.endValue = new Date(this.fiscalyear.end);
           }
           if(  Date.parse(  this.fiscalyear.availableAt.toString() ) > 0){
                this.availableatValue = new Date(this.fiscalyear.availableAt);
           }
           if(  Date.parse(  this.fiscalyear.extendedTo.toString() ) > 0){
                this.extendedtoValue = new Date(this.fiscalyear.extendedTo);
           }
       }

    }

    endDateSelected(event){
        this.fiscalyearForm.patchValue({end:event});
    }
    startDateSelected(event){
        this.fiscalyearForm.patchValue({start:event});
    }
    availableAtDateSelected(event){
        this.fiscalyearForm.patchValue({availableat:event});
    }
    extendedToDateSelected(event){
        this.fiscalyearForm.patchValue({extendedto:event});
    }

    onSubmit(){            
        if(this.fiscalyear){
            this.service.updateFiscalYear(this.fiscalyear.id, this.fiscalyearForm.value).
            subscribe(
                res => {
                    this.fiscalyear = <FiscalYear> res;
                    this.onFormSubmit.emit();
                }
            );
        }else{
            this.service.addFiscalYear(this.fiscalyearForm.value).
            subscribe(
                res => {
                    this.onFormSubmit.emit();
                }
            );
        }
        
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}
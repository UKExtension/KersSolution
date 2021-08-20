import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FiscalyearService, FiscalYear } from './fiscalyear.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';

@Component({
    selector: 'fiscalyear-form',
    templateUrl: 'fiscalyear-form.component.html' 
})
export class FiscalyearFormComponent implements OnInit{

    fiscalyearForm = null;
    @Input() fiscalyear:FiscalYear = null;
    errorMessage: string;
    public options: Object;
    public myDatePickerOptions: IAngularMyDpOptions = {
        dateFormat: 'mm/dd/yyyy',
        satHighlight: true,
        firstDayOfWeek: 'su'
    };

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: FiscalyearService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location
    ){

        
        let date = new Date();
        this.fiscalyearForm = fb.group(
            {
              start: [{
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }, Validators.required],
              end: [{
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }],
              availableAt: [{
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }],
              extendedTo: [{
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }],
              name: ['', Validators.required],
              type: ['', Validators.required]
            }
        );
    }
   
    ngOnInit(){
        if(this.fiscalyear){
            this.fiscalyearForm.patchValue(this.fiscalyear);
            var startParts = this.fiscalyear.start.toString().split(/[^0-9]/);
            var start = new Date(+startParts[0], +startParts[1] -1, +startParts[2]);
            var endParts = this.fiscalyear.end.toString().split(/[^0-9]/);
            var end = new Date(+endParts[0], +endParts[1] -1, +endParts[2]);
            var availableAtParts = this.fiscalyear.availableAt.toString().split(/[^0-9]/);
            var availableAt = new Date(+availableAtParts[0], +availableAtParts[1] -1, +availableAtParts[2]);
            var extendedToParts = this.fiscalyear.extendedTo.toString().split(/[^0-9]/);
            var extendedTo = new Date(+extendedToParts[0], +extendedToParts[1] -1, +extendedToParts[2]);
            this.fiscalyearForm.patchValue({
                start: {
                    isRange: false, singleDate: {jsDate: start}
                },
                end: {
                    isRange: false, singleDate: {jsDate: end}
                },
                availableAt: {
                    isRange: false, singleDate: {jsDate: availableAt}
                },
                extendedTo: {
                    isRange: false, singleDate: {jsDate: extendedTo}    
                }
            
            });
        }
        

    }


    onSubmit(){
        var val = this.fiscalyearForm.value;
        if(val.start.singleDate != undefined){
            var startValue:Date = val.start.singleDate.jsDate;
            var dStart = new Date(Date.UTC(startValue.getFullYear(), startValue.getMonth(), startValue.getDate(), 8, 5, 12));  
            val.start = dStart; 
        }
        if(val.end.singleDate != undefined){
            var endValue:Date = val.end.singleDate.jsDate;
            var dEnd = new Date(Date.UTC(endValue.getFullYear(), endValue.getMonth(), endValue.getDate(), 8, 5, 12));   
            val.end = dEnd;
        }
        if(val.availableAt.singleDate != undefined){
            var availableAtValue:Date = val.availableAt.singleDate.jsDate;
            var dAvailable = new Date(Date.UTC(availableAtValue.getFullYear(), availableAtValue.getMonth(), availableAtValue.getDate(), 8, 5, 12)); 
            val.availableAt = dAvailable;   
        }
        if(val.extendedTo.singleDate != undefined){
            var extendedToValue:Date = val.extendedTo.singleDate.jsDate;
            var dExtendedTo = new Date(Date.UTC(extendedToValue.getFullYear(), extendedToValue.getMonth(), extendedToValue.getDate(), 8, 5, 12));       
            val.extendedTo = dExtendedTo;
        }
        if(this.fiscalyear){
            this.service.updateFiscalYear(this.fiscalyear.id, val).
                subscribe(
                    res => {
                        this.fiscalyear = <FiscalYear> res;
                        this.onFormSubmit.emit();
                    }
                );
        }else{
            this.service.addFiscalYear(val).
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
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FiscalyearService, FiscalYear } from './fiscalyear.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import { IMyDpOptions } from 'mydatepicker';

@Component({
    selector: 'fiscalyear-form',
    templateUrl: 'fiscalyear-form.component.html' 
})
export class FiscalyearFormComponent implements OnInit{

    fiscalyearForm = null;
    @Input() fiscalyear:FiscalYear = null;
    errorMessage: string;
    public options: Object;
    myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true
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
            var start = new Date(this.fiscalyear.start);
            var end = new Date(this.fiscalyear.end);
            var availableAt = new Date(this.fiscalyear.availableAt);
            var extendedTo = new Date(this.fiscalyear.extendedTo);
            this.fiscalyearForm.patchValue({
                start: {
                    date: {
                        year: start.getFullYear(),
                        month: start.getMonth() + 1,
                        day: start.getDate()}
                    },
                end: {
                        date: {
                            year: end.getFullYear(),
                            month: end.getMonth() + 1,
                            day: end.getDate()}
                        },
                availableAt: {
                        date: {
                            year: availableAt.getFullYear(),
                            month: availableAt.getMonth() + 1,
                            day: availableAt.getDate()}
                        },
                extendedTo: {
                        date: {
                            year: extendedTo.getFullYear(),
                            month: extendedTo.getMonth() + 1,
                            day: extendedTo.getDate()}
                        }
            
            });
        }
        this.myDatePickerOptions.editableDateField = false;
        this.myDatePickerOptions.showClearDateBtn = false;

    }


    onSubmit(){
        var val = this.fiscalyearForm.value;
        if(val.start.date.year != undefined){
            var startValue = val.start;
            var dStart = new Date(Date.UTC(startValue.date.year, startValue.date.month - 1, startValue.date.day, 8, 5, 12));  
            val.start = dStart; 
        }
        if(val.end.date.year != undefined){
            var endValue = val.end;
            var dEnd = new Date(Date.UTC(endValue.date.year, endValue.date.month - 1, endValue.date.day, 8, 5, 12));   
            val.end = dEnd;
        }
        if(val.availableAt.date.year != undefined){
            var availableAtValue = val.availableAt;
            var dAvailable = new Date(Date.UTC(availableAtValue.date.year, availableAtValue.date.month - 1, availableAtValue.date.day, 8, 5, 12)); 
            val.availableAt = dAvailable;   
        }
        if(val.extendedTo.date.year != undefined){
            var extendedToValue = val.extendedTo;
            var dExtendedTo = new Date(Date.UTC(extendedToValue.date.year, extendedToValue.date.month - 1, extendedToValue.date.day, 8, 5, 12));       
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
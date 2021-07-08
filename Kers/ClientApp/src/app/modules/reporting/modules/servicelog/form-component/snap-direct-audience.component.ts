import { Component, Input, forwardRef, OnInit } from '@angular/core';
import {    FormBuilder, ControlValueAccessor, AbstractControl, 
            NG_VALUE_ACCESSOR, 
            NG_VALIDATORS, 
            FormControl, 
            Validator
        } from '@angular/forms';
import { ServicelogService, SnapIndirectMethod, SnapIndirectReached, SnapIndirectReachedValue, SnapDirectAgesAudienceValue, SnapDirectAges, SnapDirectAudience } from "../servicelog.service";
import { Observable } from 'rxjs';

@Component({
  selector: 'snap-direct-audience',
  template: `
  <loading *ngIf="loading"></loading>
    <div class="table-responsive" *ngIf="!loading" [formGroup]="audienceForm">
        <table class="table table-striped table-bordered snap-direct-audience" formArrayName="snapDirectAgesAudienceValue">
            <thead>
                <tr>
                    <th>&nbsp;</th>
                    <th *ngFor="let age of ages">{{age.name}}</th>
                    <th>TOTALS</th>
                </tr>
            </thead>
            <tbody>
                <tr *ngFor="let audience of audiences">
                    <td>{{audience.name}}</td>
                    <td *ngFor="let age of ages; let i=index" [formGroupName]="rIndex()">
                        <input type="number" formControlName="value" (change)="changed($event)" maxlength="5">
                    </td>
                    <td>{{totalAudienceById(audience.id)}}</td>
                </tr>
                <tr>
                    <td></td>
                    <td *ngFor="let age of ages"></td>
                    <td>{{totalAudience()}}</td>
                </tr>
            </tbody>
        </table>
    </div>
  
  `,
  styles:[`
  .snap-direct-audience input{
    width: 80px;
  }
  `],
    providers:[  {
                    provide: NG_VALIDATORS,
                    useExisting: forwardRef(() => SnapDirectAudienceComponent),
                    multi: true,
                } ,
                { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => SnapDirectAudienceComponent),
                    multi: true
                  } 
                ]
})
export class SnapDirectAudienceComponent implements ControlValueAccessor, OnInit, Validator {

    @Input() _value:SnapDirectAgesAudienceValue[] = [];

    audienceForm = null;
    loading = true;

    ages:SnapDirectAges[];
    audiences:SnapDirectAudience[];

    ageaudienceindex = 0;

    errorMessages;


    errors = 0;

    get selections() {
        return this._value;
    }
    set selections(val) {
        this._value = val;     
        this.propagateChange(val);
    }


    public propagateChange: any;
    

    constructor(
        private fb: FormBuilder,
        private service: ServicelogService,
    ){
        
        this.propagateChange = () => {};

    }

    ngOnInit(){

        let ageAudienceArray = [];
        this.service.snapdirectages().subscribe(
            res=>{
                this.ages = <SnapDirectAges[]> res;
                this.service.snapdirectaudience().subscribe(
                    res =>{
                        this.audiences = <SnapDirectAudience[]>res;

                        for(let aud of this.audiences){
                            for(let age of this.ages){
                                ageAudienceArray.push(this.fb.group({
                                    value: [0, this.isPositiveInt],
                                    snapDirectAgesId: age.id,
                                    snapDirectAudienceId: aud.id
                                }));                
                            }
                        }
                        this.audienceForm = this.fb.group({
                            snapDirectAgesAudienceValue: this.fb.array(ageAudienceArray)
                        });
                        this.audienceForm.patchValue({snapDirectAgesAudienceValue: this.selections});
                        this.loading = false;
                    },
                    err => this.errorMessages = <any>err
                );
            },
            err => this.errorMessages = <any>err
        );
        
    }

    writeValue(value: any) {
        if (value !== []) {
            this.selections = value;
            if(this.audienceForm != null){
                this.audienceForm.patchValue({snapDirectAgesAudienceValue: this.selections});
            }
        }
    }


    registerOnChange(fn) {
        this.propagateChange = fn;
    }

    registerOnTouched() {}


    changed(event){
        this.selections = this.audienceForm.value.snapDirectAgesAudienceValue; 
    }
    
    public validate(control: FormControl) {
        
        if(this.loading){
            return null;
        }else{
            if(this.audienceForm.invalid) return { reachedInvalid: {valid:false} };
            return  null;
        }
    }


    totalAudience(){
        var sum = 0;
        for( let contr of this.audienceForm.controls.snapDirectAgesAudienceValue.controls){
            sum += contr.value.value;
        }   
        return sum;
    }
    totalAudienceById(audId:number){
        var sum = 0;
        for( let contr of this.audienceForm.controls.snapDirectAgesAudienceValue.controls){
            if(contr.value.snapDirectAudienceId == audId){
                sum += contr.value.value;
            }
        }   
        return sum;
    }

    isPositiveInt(control:FormControl){
        
        if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
            return null;
        }
        return {"notInt":true};
    }

    rIndex(){
        if(this.ageaudienceindex == this.audienceForm.get("snapDirectAgesAudienceValue").length){
            this.ageaudienceindex = 0
        }
        var val = ''+this.ageaudienceindex++
        return val;
    }

}


import { Component, Input, forwardRef, OnInit } from '@angular/core';
import {    FormBuilder, ControlValueAccessor, AbstractControl, 
            NG_VALUE_ACCESSOR, 
            NG_VALIDATORS, 
            FormControl, 
            Validator
        } from '@angular/forms';
import { ServicelogService, SnapIndirectMethod, SnapIndirectReached, SnapIndirectReachedValue, SnapPolicyPartnerValue, SnapPolicyPartner } from "../servicelog.service";
import { Observable } from "rxjs/Observable";

@Component({
  selector: 'snap-policy-partners',
  template: `
  <loading *ngIf="loading"></loading>
    <div class="table-responsive" *ngIf="!loading" [formGroup]="partnerForm">
        <table class="table table-striped table-bordered" formArrayName="snapPolicyPartnerValue">
            <tbody>
                
                <tr *ngFor="let opt of partnerForm.controls.snapPolicyPartnerValue.controls; let i=index" [formGroupName]="i">
                    <td>{{partnerChoices[i].name}}</td>
                    <td><input id="{{partnerChoices[i].id}}" type="number" maxlength="5" (change)="changed($event)" formControlName="value"></td>
                </tr>
            
            
            </tbody>
        </table>
    </div>
  
  `,
    providers:[  {
                    provide: NG_VALIDATORS,
                    useExisting: forwardRef(() => SnapPolicyPartnersComponent),
                    multi: true,
                } ,
                { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => SnapPolicyPartnersComponent),
                    multi: true
                  } 
                ]
})
export class SnapPolicyPartnersComponent implements ControlValueAccessor, OnInit, Validator {

    @Input() _value:SnapPolicyPartnerValue[] = [];

    partnerForm = null;
    loading = true;
    partnerChoices: SnapPolicyPartner[];

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
        var formArray = []
        this.service.snappolicypartner().subscribe(
            res=>{
                this.partnerChoices = <SnapPolicyPartner[]>res;
                for(let choice of this.partnerChoices){
                    formArray.push( this.fb.group(
                        {
                            snapPolicyPartnerId: choice.id,
                            value: [0, this.isPositiveInt]
                        }
                    ))
                }
                this.partnerForm = this.fb.group({
                    snapPolicyPartnerValue: this.fb.array(formArray)
                });
                this.partnerForm.patchValue({snapPolicyPartnerValue: this.selections});
                this.loading = false;
            }
        );
        
    }

    writeValue(value: any) {
        if (value !== []) {
            this.selections = value;
            if(this.partnerForm != null){
                this.partnerForm.patchValue({snapPolicyPartnerValue: this.selections});
            }
        }
    }


    registerOnChange(fn) {
        this.propagateChange = fn;
    }

    registerOnTouched() {}


    changed(event){
        this.selections = this.partnerForm.value.snapPolicyPartnerValue; 
    }
    
    public validate(control: FormControl) {
        
        if(this.loading){
            return null;
        }else{
            if(this.partnerForm.invalid) return { partnerInvalid: {valid:false} };
            return  null;
        }
    }

    isPositiveInt(control:FormControl){
        
        if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
            return null;
        }
        return {"notInt":true};
    }

}


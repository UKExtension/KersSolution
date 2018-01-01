import { Component, Input, forwardRef, OnInit } from '@angular/core';
import {    FormBuilder, ControlValueAccessor, AbstractControl, 
            NG_VALUE_ACCESSOR, 
            NG_VALIDATORS, 
            FormControl, 
            Validator
        } from '@angular/forms';
import { ServicelogService, SnapIndirectMethod, SnapIndirectReached, SnapIndirectReachedValue } from "../servicelog.service";
import { Observable } from "rxjs/Observable";

@Component({
  selector: 'snap-indirect-reached',
  template: `
  <loading *ngIf="loading"></loading>
    <div class="col-md-9 col-sm-9 col-xs-12" *ngIf="!loading" [formGroup]="reachedForm">
        <table class="table table-striped table-bordered" formArrayName="reached">
            <tbody>
                
                <tr *ngFor="let opt of reachedForm.controls.reached.controls; let i=index" [formGroupName]="i">
                    <td>{{reachedChoices[i].name}}</td>
                    <td><input id="{{reachedChoices[i].id}}" type="number" maxlength="5" (change)="changed($event)" formControlName="value"></td>
                </tr>
            
            
            </tbody>
        </table>
    </div>
  
  `,
    providers:[  {
                    provide: NG_VALIDATORS,
                    useExisting: forwardRef(() => SnapIndirectReachedComponent),
                    multi: true,
                } ,
                { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => SnapIndirectReachedComponent),
                    multi: true
                  } 
                ]
})
export class SnapIndirectReachedComponent implements ControlValueAccessor, OnInit, Validator {

    @Input() _value:SnapIndirectReachedValue[] = [];

    reachedForm = null;
    loading = true;


    reachedChoices:SnapIndirectReached[] = [];


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
        var formArray = []
        this.service.snapindirectreached().subscribe(
            res=>{
                this.reachedChoices = <SnapIndirectReached[]>res;
                for(let choice of this.reachedChoices){
                    formArray.push( this.fb.group(
                        {
                            snapIndirectReachedId: choice.id,
                            value: [0, this.isPositiveInt]
                        }
                    ))
                }
                this.reachedForm = this.fb.group({
                    reached: this.fb.array(formArray)
                });
                this.reachedForm.patchValue({reached: this.selections});
                this.loading = false;
            }
        );
        
    }

    writeValue(value: any) {
        if (value !== []) {
            this.selections = value;
            if(this.reachedForm != null){
                this.reachedForm.patchValue({reached: this.selections});
            }
        }
    }


    registerOnChange(fn) {
        this.propagateChange = fn;
    }

    registerOnTouched() {}


    changed(event){
        /*
        console.log(this.reachedForm.value.reached);
        var element = this._value.filter(o => o.snapIndirectReachedId == event.target.id);
        if(element.length != 0){
            element[0].value = event.target.value;
        }else{
            this._value.push(<SnapIndirectReachedValue>{snapIndirectReachedId: event.target.id, value: event.target.value});
        }
        */
        this.selections = this.reachedForm.value.reached; 
    }
    
    public validate(control: FormControl) {
        
        if(this.loading){
            return null;
        }else{
            if(this.reachedForm.invalid) return { reachedInvalid: {valid:false} };
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


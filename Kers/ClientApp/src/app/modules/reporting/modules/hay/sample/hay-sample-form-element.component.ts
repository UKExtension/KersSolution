import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl, FormArray } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { SampleAttributeSampleInfoBundle, SampleAttributeType, SampleInfoBundle } from '../../soildata/sample/SampleInfoBundle';
import { SoilSampleService } from '../../soildata/sample/soil-sample.service';
import { HaySample } from './hay-sample';



@Component({
  selector: 'hay-sample-form-element',
  template: `
<div class="form-horizontal form-label-left sample-crop" [formGroup]="sampleForm">
    
  <div class="row" style="padding: 8px 0;">
    <div class="col-sm-11">

       Hay Sample
        
    </div>
    <div class="col-sm-1">
            <div *ngIf="index != 0 && !disabled" class="col-xs-1 ng-star-inserted text-right pull-right"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer; top: -18px;"><i class="fa fa-close"></i></a></span></div>
            <br>
        <small>&nbsp;</small>
    </div>
  </div>
</div>

  `,
  styles: [`
  .sample-crop{
    border-bottom: 1px solid #ccc
  }
  `]
  ,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => HaySampleFormElementComponent),
                  multi: true
                },
                {
                  provide: NG_VALIDATORS,
                  useExisting: forwardRef(() => HaySampleFormElementComponent),
                  multi: true
                } 
                ]
})
export class HaySampleFormElementComponent extends BaseControlValueAccessor<HaySample> implements ControlValueAccessor, OnInit { 
    sampleForm: FormGroup;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();



    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.sampleForm = formBuilder.group({
        
      });
  
      this.sampleForm.valueChanges.subscribe(val => {
        this.value = <HaySample>val;
        this.onChange(this.sampleForm.getRawValue());
      });

    }
    onRemove(){
        this.removeMe.emit(this.index);
    } 

    ngOnInit(){
      
    }


   
 




    writeValue(session: HaySample) {
      this.value = session;
      this.sampleForm.patchValue(this.value);

    }
    
    setDisabledState(){
      this.disabled = true;
      this.sampleForm.controls["typeFormId"].disable();
    }

    validate(c: AbstractControl): ValidationErrors | null{
      var returnValue = null;
      if(!this.sampleForm.valid){
        returnValue = { invalidForm: {valid: false, message: "Crop fields are invalid"}};
      }
      return returnValue;
    }


}
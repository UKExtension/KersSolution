import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TypeForm } from '../soildata.report';
import { SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';



@Component({
  selector: 'soil-crop-form-element',
  template: `
<div class="form-group" [formGroup]="sessionGroup">
    
  <div class="row" style="padding: 8px 0;">
    <div class="col-sm-10">
        <select name="typeFormId" formControlName="typeFormId" class="form-control col-md-7 col-xs-12">
            <option *ngFor="let formtype of typeForms | async" [value]="formtype.id">{{formtype.name}}</option>
        </select>


        <small>(Note)</small>
    </div>
    <div class="col-sm-2">
            <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer; top: -18px;"><i class="fa fa-close"></i></a></span></div>
            <br>
        <small>&nbsp;</small>
    </div>
  </div>
</div>

  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => SoilCropFormElementComponent),
                  multi: true
                } 
                /* 
                ,
                {
                  provide: NG_VALIDATORS,
                  useExisting: forwardRef(() => SoilCropFormElementComponent),
                  multi: true
                } */
                ]
})
export class SoilCropFormElementComponent extends BaseControlValueAccessor<SampleInfoBundle> implements ControlValueAccessor, OnInit { 
    sessionGroup: FormGroup;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();

    typeForms:Observable<TypeForm[]>;

    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.sessionGroup = formBuilder.group({
        typeFormId: ['']
      });
  
      this.sessionGroup.valueChanges.subscribe(val => {
        this.value = <SampleInfoBundle>val;
        this.onChange(this.value);
      });
      this.typeForms = service.formTypes();
    }
    onRemove(){
        //this.removeMe.emit(this.value.index);
    } 

    ngOnInit(){
    }


    onDateChanged($event){
      
    }
    writeValue(session: SampleInfoBundle) {
      this.value = session;
      this.sessionGroup.patchValue(this.value);
    }
/* 

    validate(c: AbstractControl): ValidationErrors | null{
      return this.sessionGroup.valid ? null : { invalidForm: {valid: false, message: "Session fields are invalid"}};
    }
 */

}
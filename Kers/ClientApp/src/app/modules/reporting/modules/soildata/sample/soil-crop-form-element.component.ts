import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TypeForm } from '../soildata.report';
import { SampleAttributeType, SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';



@Component({
  selector: 'soil-crop-form-element',
  template: `
<div class="form-horizontal form-label-left" [formGroup]="sessionGroup">
    
  <div class="row" style="padding: 8px 0;">
    <div class="col-sm-11">

        <div class="form-group">
            <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">Intended Use:</label>           
            <div class="col-md-9 col-sm-9 col-xs-12">
              <select name="typeFormId" formControlName="typeFormId" class="form-control col-md-7 col-xs-12" (change)="formTypeChange($event)">
                  <option value="">-- select form type --</option>
                  <option *ngFor="let formtype of typeForms | async" [value]="formtype.id">{{formtype.name}}</option>
              </select>
            </div>
        </div>
        <ng-container *ngIf="selectedFormType!=''" formArrayName="sampleAttributeSampleInfoBundles">
          <div class="form-group" *ngFor="let attributyType of attributeTypes | async ; let i=index">
              <soil-cropattribute-form-element [type]="attributyType" [formControlName]="i"></soil-cropattribute-form-element>
          </div>
        </ng-container>


        
    </div>
    <div class="col-sm-1">
            <div *ngIf="index != 0" class="col-xs-1 ng-star-inserted text-right pull-right"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer; top: -18px;"><i class="fa fa-close"></i></a></span></div>
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
    attributeTypes:Observable<SampleAttributeType[]>;

    get selectedFormType() {
      var formTypeControl =  this.sessionGroup.get('typeFormId') as FormControl;
      return formTypeControl.value;
    }

    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.sessionGroup = formBuilder.group({
        typeFormId: ['', Validators.required],
        sampleAttributeSampleInfoBundles:this.formBuilder.array([])
      });
  
      this.sessionGroup.valueChanges.subscribe(val => {
        this.value = <SampleInfoBundle>val;
        this.onChange(this.value);
      });
      this.typeForms = service.formTypes();
    }
    onRemove(){
        this.removeMe.emit(this.index);
    } 

    ngOnInit(){
      
    }


    formTypeChange(event){
      this.attributeTypes = this.service.attributeTypes(this.selectedFormType);
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
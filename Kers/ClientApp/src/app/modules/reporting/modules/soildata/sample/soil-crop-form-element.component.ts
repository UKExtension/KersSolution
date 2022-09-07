import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl, FormArray } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TypeForm } from '../soildata.report';
import { SampleAttributeSampleInfoBundle, SampleAttributeType, SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';



@Component({
  selector: 'soil-crop-form-element',
  template: `
<div class="form-horizontal form-label-left sample-crop" [formGroup]="sampleForm">
    
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
        </div><br>
        <ng-container *ngIf="selectedFormType!='' && !attributesLoading" formArrayName="sampleAttributeSampleInfoBundles">
          <div class="form-group" *ngFor="let attributyType of sampleAttributeSampleInfoBundles.controls ; let i=index">
              <soil-cropattribute-form-element [formControlName]="i" [connections]="connections"></soil-cropattribute-form-element>
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
  styles: [`
  .sample-crop{
    border-bottom: 1px solid #ccc
  }
  `]
  ,
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
    sampleForm: FormGroup;
    connections:SampleAttributeSampleInfoBundle[] = null;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();

    typeForms:Observable<TypeForm[]>;
    attributeTypes:Observable<SampleAttributeType[]>;
    attributesLoading = false;

    get selectedFormType() {
      var formTypeControl =  this.sampleForm.get('typeFormId') as FormControl;
      return formTypeControl.value;
    }
    get sampleAttributeSampleInfoBundles() {
      return this.sampleForm.get('sampleAttributeSampleInfoBundles') as FormArray;
    }

    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.sampleForm = formBuilder.group({
        typeFormId: ['', Validators.required],
        sampleAttributeSampleInfoBundles:this.formBuilder.array([]),
        purposeId: [1]
      });
  
      this.sampleForm.valueChanges.subscribe(val => {
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
      if(this.selectedFormType != '' ){
        this.attributesLoading = true;

        
        this.sampleAttributeSampleInfoBundles.clear();
        
        this.service.attributeTypes(this.selectedFormType).subscribe(
          res => {
            var types:SampleAttributeType[] = res;
            
            for( let type of types){
              this.sampleAttributeSampleInfoBundles.push(this.formBuilder.control({sampleAttribute: {sampleAttributeTypeId: type.id, sampleAttributeType:type}}));
            }
            this.attributesLoading = false;
            
          }
        );
      }
      
      
 
    }



    writeValue(session: SampleInfoBundle) {
      this.value = session;
      if( session.sampleAttributeSampleInfoBundles != null && session.sampleAttributeSampleInfoBundles.length > 0 ) this.connections = session.sampleAttributeSampleInfoBundles;
      this.sampleForm.patchValue(this.value);
      this.formTypeChange(null);
    }
/* 

    validate(c: AbstractControl): ValidationErrors | null{
      return this.sampleForm.valid ? null : { invalidForm: {valid: false, message: "Session fields are invalid"}};
    }
 */

}
import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TypeForm } from '../soildata.report';
import { SampleAttribute, SampleAttributeSampleInfoBundle, SampleAttributeType, SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';



@Component({
  selector: 'soil-cropattribute-form-element',
  template: `
<div class="form-horizontal form-label-left" [formGroup]="attributeGroup">
    
  <div class="row" style="padding: 8px 0;" *ngIf="value">
    <div class="col-sm-12">
        <div class="form-group">
            <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">{{type.name}}:</label>           
            <div class="col-md-9 col-sm-9 col-xs-12">
              <select name="typeFormId" formControlName="sampleAttributeId" class="form-control col-md-7 col-xs-12" (change)="formTypeChange($event)">
                  <option value="">-- select {{type.name}} --</option>
                  <option *ngFor="let attribute of attributes" [value]="attribute.id">{{attribute.name}}</option>
              </select>
            </div>
        </div>        
    </div>
  </div>
</div>

  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => SoilCropattributeFormElementComponent),
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
export class SoilCropattributeFormElementComponent extends BaseControlValueAccessor<SampleAttributeSampleInfoBundle> implements ControlValueAccessor, OnInit { 
    
    @Input() connections:SampleAttributeSampleInfoBundle[] = null;
  
    attributeGroup: FormGroup;
    type:SampleAttributeType;

    attributes:SampleAttribute[];

    get selectedFormType() {
      var formTypeControl =  this.attributeGroup.get('attributeId') as FormControl;
      return formTypeControl.value;
    }

    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.attributeGroup = formBuilder.group({
        sampleAttributeId: ['', Validators.required]
      });
  
      this.attributeGroup.valueChanges.subscribe(val => {
        this.value = <SampleAttributeSampleInfoBundle>val;
        this.onChange(this.value);
        //if( this.connections != null ) this.attributeGroup.patchValue( this.connections );
      });
      
    }
    

    ngOnInit(){
       
    }

    formTypeChange(event){

    }

    writeValue(attribute: SampleAttributeSampleInfoBundle) {
      if(attribute != null ){
        this.type = attribute.sampleAttribute.sampleAttributeType;
        this.service.attributes( attribute.sampleAttribute.sampleAttributeTypeId).subscribe(
          res =>{
            this.attributes  = res;
            if( this.connections != null){
              let attrIds = this.connections.map(a => a.sampleAttributeId);
              const filteredArray = this.attributes.filter(value => attrIds.includes(value.id));
              if(filteredArray.length > 0) this.attributeGroup.patchValue({sampleAttributeId:filteredArray[0].id});
            }
            
          } 
        );
        this.attributeGroup.patchValue(attribute);
      }
    }
/* 

    validate(c: AbstractControl): ValidationErrors | null{
      return this.attributeGroup.valid ? null : { invalidForm: {valid: false, message: "Session fields are invalid"}};
    }
 */

}
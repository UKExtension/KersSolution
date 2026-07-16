import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl, FormArray } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { SampleAttributeSampleInfoBundle, SampleAttributeType, SampleInfoBundle } from '../../soildata/sample/SampleInfoBundle';
import { SoilSampleService } from '../../soildata/sample/soil-sample.service';
import { HaySample, HayType, HayTypeDetails } from './hay-sample';



@Component({
  selector: 'hay-sample-form-element',
  template: `
<div class="form-horizontal form-label-left sample-crop" [formGroup]="sampleForm">
    
  <div class="row" style="padding: 8px 0;">
    <div class="col-sm-11">

       
        <div class="form-group">
            <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Sample Name:</label>           
            <div class="col-md-9 col-sm-9 col-xs-12">
                <input type="text" formControlName="name" id="name" class="form-control col-xs-12"/>
            </div>
        </div>




       <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="billingTypeId">Sample Type:</label>
            <div class="col-md-6 col-sm-6 col-xs-12">
                <select name="hayTypeId" formControlName="hayTypeId" class="form-control col-md-7 col-xs-12" (change)="SampleTypeChange($event)">
                    <option value="">-- select sample type --</option>
                    <option *ngFor="let type of types" [value]="type.id">{{type.name}}</option>
                </select>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="billingTypeId">Hay Type:</label>
            <div class="col-md-6 col-sm-6 col-xs-12">
                <select name="hayTypeDetailId" formControlName="hayTypeDetailId" class="form-control col-md-7 col-xs-12">
                    <option value="">-- select hay type --</option>
                    <option *ngFor="let selectedType of selectedTypeDetail" [value]="selectedType.id">{{selectedType.name}}</option>
                </select>
            </div>
        </div>


        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="billingTypeId"></label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                All samples run by Near Infrared Spectroscopy (NIR). $22
            </div>
        </div>



        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="optionalTests">Optional Tests:</label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                <ng-select
                    id="optionalTests"
                    formControlName="optionalTests"
                    [items]="testTypes"
                    [multiple]="true"
                    [hideSelected]="true"
                    placeholder = "(select any/all that apply)">
                </ng-select>
            </div>
        </div>
        <div class="form-group">
            <label class="control-label col-md-3 col-sm-3 col-xs-12" for="billingTypeId"></label>
            <div class="col-md-9 col-sm-9 col-xs-12">
                Kentucky producers who have their hay tested by a certified lab, such as the University of Kentucky Division of Agriculture Forage Program, may participate in the Kentucky Department of Agriculture’s hay grading program and have the hay included on KDA’s Hay For Sale listing. By checking the option to email the report to “KDA for Marketing” on page 1, you will receive this free service.
            </div>
        </div>
        
         <div class="form-group">
            <label for="SnapEdEligable" class="control-label col-md-3 col-sm-3 col-xs-12">Offer for sale through KDA:</label>           
            <div class="col-md-9 col-sm-9 col-xs-12">
                <label class="switch">
                    <input type="checkbox" id="isKda">
                    <div class="slider round" (click)="onKdaChecked()"></div>
                </label>
            </div>
        </div>
        <div *ngIf="kdaSelected">
        <h4>KDA Marketing Information</h4>
        <h5>Lot Information</h5>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Initial Number of Bales:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Cutting Date:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Bales sie:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Bale weight:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Binding (String, Net Wrap, other):</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Storage type:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Method of contact:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" id="name" class="form-control col-xs-12"/>
              </div>
          </div>
          
        </div>



        
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
    .fa{
    font-size: 16px;
}
    /* The switch - the box around the slider */
.switch {
  position: relative;
  display: inline-block;
  width: 60px;
  height: 34px;
}

/* Hide default HTML checkbox */
.switch input {display:none;}

/* The slider */
.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  -webkit-transition: .4s;
  transition: .4s;
}

.slider:before {
  position: absolute;
  content: "";
  height: 26px;
  width: 26px;
  left: 4px;
  bottom: 4px;
  background-color: white;
  -webkit-transition: .4s;
  transition: .4s;
}

input:checked + .slider {
  background-color: rgb(38, 185, 154);
  border-color: rgb(38, 185, 154); 
  box-shadow: rgb(38, 185, 154) 
}

input:focus + .slider {
  box-shadow: 0 0 1px rgb(38, 185, 154);
}

input:checked + .slider:before {
  -webkit-transform: translateX(26px);
  -ms-transform: translateX(26px);
  transform: translateX(26px);
}

/* Rounded sliders */
.slider.round {
  border-radius: 34px;
}

.slider.round:before {
  border-radius: 50%;
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

    types = HayType;
    typesDetail = HayTypeDetails;
    selectedTypeDetail:any[] = [];
      testTypes:Array<any> = [
    {value: 1, label: 'DA Mineral analysis ($12)'},
    {value: 2, label: 'MW Mineral analysis ($17)'}
  ]

    kdaSelected = false;
    date = new Date();
    constructor( 
      private formBuilder: FormBuilder,
      private service:SoilSampleService
    )   
    {
      super();
      this.sampleForm = formBuilder.group({
        name: '',
        hayTypeId: '',
        hayTypeDetailId: '',
        tests: ''
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

    SampleTypeChange(event:any){
      var selected = event.target.value;
      this.selectedTypeDetail = [];
      if( selected != undefined && selected != ''){
        this.selectedTypeDetail = this.typesDetail.filter( s => s.hayTypeId == selected);
      }
    }
   
    onKdaChecked(){
      this.kdaSelected = !this.kdaSelected;
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
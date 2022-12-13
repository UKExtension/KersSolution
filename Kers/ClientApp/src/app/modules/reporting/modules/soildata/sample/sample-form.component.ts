import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable } from 'rxjs';
import { SoilReportBundle } from '../soildata.report';
import { FarmerAddress } from '../soildata.service';
import { BillingType, OptionalTest, SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';

@Component({
  selector: 'soil-sample-form',
  templateUrl: './sample-form.component.html',
  styles: [
    `
    .index-border{
      border: 1px solid #1ABB9C;
      width: 20px;
      height: 20px;
      margin: 5px 10px 5px 0;
      border-radius: 50%;
      padding: 9px 12px;
    }

    `
  ]
})
export class SampleFormComponent implements OnInit {
  
  @Input() sample:SoilReportBundle;
  @Input() isThisACopy:boolean = false;
  @Input() isThisAltCrop:boolean = false;
  soilSampleForm:any;

  loading = false;
  addressBrowserOpen = true;
  testTypes:Array<any>;
  selectedAddress:FarmerAddress;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<SoilReportBundle>();
  private myDatePickerOptions: IAngularMyDpOptions = {
    // other options...
        dateFormat: 'mm/dd/yyyy',
        satHighlight: true,
        firstDayOfWeek: 'su'
    };

  get sampleInfoBundles() {
    return this.soilSampleForm.get('sampleInfoBundles') as FormArray;
  }
  billingTypes$:Observable<BillingType[]>;
  lastSampleNum$:Observable<number>;

  constructor(
    private fb: FormBuilder,
    private service:SoilSampleService
  ) { 
    let date = new Date();
    this.soilSampleForm = this.fb.group(
      { 
          farmerAddress: [null, Validators.required],
          ownerID: [""],
          sampleLabelCreated: [{
            isRange: false, singleDate: {jsDate: date}
                      }, Validators.required],
          billingTypeId: [1],
          coSamnum: ["", [Validators.maxLength(4), Validators.required]],
          optionalTests: '',
          acres: [""],
          optionalInfo: [""],
          privateNote: [""],
          sampleInfoBundles: this.fb.array([])
      }, { validator: sampleValidator });

  }

  ngOnInit(): void {
    
    this.billingTypes$ = this.service.billingtypes();
    if( this.sample != null && !this.isThisACopy ) this.soilSampleForm.controls["coSamnum"].disable();
    
    this.service.optionaltests().subscribe(
                          res => {
                              var tsts = <OptionalTest[]> res;
                              var ts = Array<any>();
                              tsts.forEach(function(element){
                                  ts.push(
                                      {value: element.id, label: element.name}
                                  );
                              });
                              this.testTypes = ts;



                              if( this.sample != null ){
                                this.soilSampleForm.patchValue(this.sample);
                                if(this.sample.farmerForReport != null){
                                  this.selectedAddress = this.sample.farmerForReport;
                                  this.addressBrowserOpen = false;
                                }
                        
                        
                                let date = new Date(this.sample.sampleLabelCreated);
                        
                                this.soilSampleForm.patchValue({sampleLabelCreated: {
                                      isRange: false, singleDate: {jsDate: date}
                                  }});
                        
                                var optTsts = [];
                                for( var tst of this.sample.optionalTestSoilReportBundles){
                                  optTsts.push(
                                    {value: tst.optionalTestId, label: tsts.filter(c => c.id == tst.optionalTestId )[0].name}
                                  );
                                }

                                this.soilSampleForm.patchValue({optionalTests:optTsts});
                                for( var plant of this.sample.sampleInfoBundles){
                                  this.addSegment(plant);
                                }
                                if(this.isThisACopy){
                                  this.service.lastsamplenum().subscribe(
                                    res => {
                                      var lastNumber:number = res;
                                      this.soilSampleForm.patchValue({coSamnum:(lastNumber + 1)});
                                    }
                                  );
                                }
                                if( this.isThisAltCrop) this.prepereAltCrop();
                              }else{
                                this.addSegment(null);
                                this.service.lastsamplenum().subscribe(
                                  res => {
                                    var lastNumber:number = res;
                                    this.soilSampleForm.patchValue({coSamnum:(lastNumber + 1)});
                                  }
                                );
                              }


                          }
                      );
      
  }
  prepereAltCrop(){
    this.soilSampleForm.controls["farmerAddress"].disable();
    this.soilSampleForm.controls["ownerID"].disable();
    this.soilSampleForm.controls["sampleLabelCreated"].disable();
    this.soilSampleForm.controls["billingTypeId"].disable();
    this.soilSampleForm.controls["coSamnum"].disable();
    this.soilSampleForm.controls["acres"].disable();
    this.soilSampleForm.controls["optionalTests"].disable();
    this.soilSampleForm.controls["optionalInfo"].disable();
    for( var smple of this.sampleInfoBundles.controls){
      if(smple.value["purposeId"] != 2) smple.disable();
    }
  }

  addSegment(sampleInfoBundles:SampleInfoBundle = null) {
    var group:FormControl; 

    if(sampleInfoBundles == null){
      if( this.isThisAltCrop ){
        group = this.fb.control(
          {
            typeFormId: '',
            purposeId: 2
          }
        );
      }else{
        group = this.fb.control(
          {
            typeFormId: ''
          }
        );
      }
      
    }else{
      group = this.fb.control(
        {
          typeFormId: sampleInfoBundles.typeFormId,
          sampleAttributeSampleInfoBundles: sampleInfoBundles.sampleAttributeSampleInfoBundles,
          purposeId: sampleInfoBundles.purposeId
        }
      );

    }
    
    this.sampleInfoBundles.push(group);
  }


  cropRemoved(event:number){
    this.sampleInfoBundles.removeAt(event);
  }


  addressSelected(event:FarmerAddress){
    this.selectedAddress = event;
    this.addressBrowserOpen = false;
  }

  addressSelectionCanceled(){

  }

  onSubmit(){
    var SampleDataToSubmit:SoilReportBundle;
    var opTsts = [];
    var rawValue = this.soilSampleForm.getRawValue();
    SampleDataToSubmit = rawValue;
    SampleDataToSubmit.sampleLabelCreated =rawValue.sampleLabelCreated.singleDate.jsDate;
    for( var tst of rawValue.optionalTests){
      opTsts.push({optionalTestId:tst.value})
    }

    if(this.selectedAddress != undefined){
      SampleDataToSubmit.farmerAddressId = this.selectedAddress.id;
    }
    
    
    SampleDataToSubmit.optionalTestSoilReportBundles = opTsts;
    SampleDataToSubmit.optionalTests = undefined;
    
    if( !this.sample ){
      this.service.addsample(SampleDataToSubmit).subscribe(
        res => {
          this.onFormSubmit.emit(res);
        }
      );

    }else{
      this.service.updateSample( this.sample.id, SampleDataToSubmit ).subscribe(
        res => {
          this.onFormSubmit.emit(res);
        }
      )
    }
    

  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

export const sampleValidator = (control: AbstractControl): {[key: string]: boolean} => {
/* 
  let start = control.get('start');
  let end = control.get('end');

  if( end.value != null && end.value.singleDate != null){
    let startDate = start.value.singleDate.jsDate;
    let endDate = end.value.singleDate.jsDate;
    if( startDate.getTime() > endDate.getTime()){
      return {"endDate":true};
    }
  }
 */

  return null;
}

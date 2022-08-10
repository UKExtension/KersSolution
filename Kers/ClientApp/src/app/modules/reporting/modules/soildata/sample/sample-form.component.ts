import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
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
    .address-browser{
      border: 1px solid #e5e5e5;
      padding: 15px;
    }


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
          ownerID: [""],
          sampleLabelCreated: [{
            isRange: false, singleDate: {jsDate: date}
                      }, Validators.required],
          billingTypeId: [1],
          coSamnum: [""],
          optionalTests: '',
          acres: [""],
          optionalInfo: [""],
          sampleInfoBundles: this.fb.array([])
      });

  }

  ngOnInit(): void {
    
    this.billingTypes$ = this.service.billingtypes();
    this.service.lastsamplenum().subscribe(
      res => {
        var lastNumber:number = res;
        this.soilSampleForm.patchValue({coSamnum:(lastNumber + 1)});
      }
    );

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
                        
                        
                              }else{
                                this.addSegment(null);
                              }


                          }
                      );
      
  }

  addSegment(sampleInfoBundles:SampleInfoBundle = null) {
    var group:FormControl; 

    if(sampleInfoBundles == null){
      group = this.fb.control(
        {
          typeFormId: ''
        }
      );
    }else{
      group = this.fb.control(
        {
          typeFormId: sampleInfoBundles.typeFormId
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
    console.log(event);
    this.addressBrowserOpen = false;
  }

  addressSelectionCanceled(){

  }

  onSubmit(){
    var SampleDataToSubmit = <SoilReportBundle> this.soilSampleForm.value;
    if(this.selectedAddress != undefined){
      SampleDataToSubmit.farmerAddressId = this.selectedAddress.id;
    }
    SampleDataToSubmit.sampleLabelCreated = this.soilSampleForm.value.sampleLabelCreated.singleDate.jsDate;
    var opTsts = [];
    for( var tst of this.soilSampleForm.value.optionalTests){
      opTsts.push({optionalTestId:tst.value})
    }
    SampleDataToSubmit.optionalTestSoilReportBundles = opTsts;
    SampleDataToSubmit.optionalTests = undefined;
    if( !this.sample ){
      this.service.addsample(SampleDataToSubmit).subscribe(
        res => {
          this.onFormSubmit.emit(res);
        }
      );

    }
    

  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

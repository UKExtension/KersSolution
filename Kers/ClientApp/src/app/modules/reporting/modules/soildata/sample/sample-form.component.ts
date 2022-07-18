import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable } from 'rxjs';
import { FarmerAddress } from '../soildata.service';
import { BillingType, SampleInfoBundle } from './SampleInfoBundle';
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
  
  @Input() note:SampleInfoBundle;
  soilSampleForm:any;

  loading = false;
  addressBrowserOpen = true;
  selectedAddress:FarmerAddress;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<SampleInfoBundle>();
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
          acres: [""],
          optionalInfo: [""],
          sampleInfoBundles: this.fb.array([])
      });

  }

  ngOnInit(): void {
    this.addSegment(null);
    this.billingTypes$ = this.service.billingtypes();
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
      // Populate new group
    }
    
    this.sampleInfoBundles.push(group);
  }


  cropRemoved(event:number){
    console.log(event);
  }


  addressSelected(event:FarmerAddress){
    this.selectedAddress = event;
    console.log(event);
    this.addressBrowserOpen = false;
  }

  addressSelectionCanceled(){

  }

}

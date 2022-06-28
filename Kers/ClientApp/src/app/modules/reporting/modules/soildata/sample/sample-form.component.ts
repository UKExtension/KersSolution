import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { FarmerAddress } from '../soildata.service';
import { SampleInfoBundle } from './SampleInfoBundle';
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


  get sampleInfoBundles() {
    return this.soilSampleForm.get('sampleInfoBundles') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private service:SoilSampleService
  ) { 

    this.soilSampleForm = this.fb.group(
      { 
          ownerID: [""],
          acres: [""],
          sampleInfoBundles: this.fb.array([])
      });

  }

  ngOnInit(): void {
    this.addSegment(null);
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

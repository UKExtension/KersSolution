import { ViewportScroller } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormArray, FormBuilder, FormControl, ValidationErrors, Validators } from '@angular/forms';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, mergeMap, switchMap, tap } from 'rxjs/operators';

import { ReportingService } from '../../../components/reporting/reporting.service';
import { SampleInfoBundle } from '../../soildata/sample/SampleInfoBundle';
import { CountyCode, FarmerAddress } from '../../soildata/soildata.service';

@Component({
  selector: 'hay-sample-form',
  templateUrl: './hay-sample-form.component.html',
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
export class HaySampleFormComponent implements OnInit {

  @Input() countyCode:CountyCode;
  @Input() isThisAltCrop:boolean = false;

  selectedAddress!: FarmerAddress;
  testTypes:Array<any> = [
    {value: 1, label: 'DA Mineral analysis ($12)'},
    {value: 2, label: 'MW Mineral analysis ($17)'}
  ]
  soilSampleForm:any;

  loading = true;
  addressBrowserOpen = true;
  @Output() onFormCancel = new EventEmitter<void>();
  private myDatePickerOptions: IAngularMyDpOptions = {
    // other options...
        dateFormat: 'mm/dd/yyyy',
        satHighlight: true,
        firstDayOfWeek: 'su'
    };

  get haySamples() {
    return this.soilSampleForm.get('haySamples') as FormArray;
  }

 

  constructor(
    private fb: FormBuilder,
    private viewportScroller: ViewportScroller,
    private reportingService: ReportingService
  ) { 
    

  }

  ngOnInit(): void {
    

      
          this.loading = false;
          this.initializeForm();
          this.viewportScroller.scrollToAnchor("topOfTheForm");
        
      
    }


  





  initializeForm(countyCodeId:number=0){
    let date = new Date();
    this.soilSampleForm = this.fb.group(
            { 
                farmerAddress: [null, Validators.required],
                ownerID: [""],
                sampleLabelCreated: [{
                  isRange: false, singleDate: {jsDate: date}
                            }, Validators.required],
                billingTypeId: [1],
                optionalTests: '',
                optionalInfo: [""],
                privateNote: [""],
                haySamples: this.fb.array([])
            }
          );




    
    
          var sampleControl = this.soilSampleForm.get('coSamnum') as FormControl;
         

  }


  prepereAltCrop(){
    
  }
  prepereEdit(){
    
  }

  addSegment() {
    var group:FormControl; 

        group = this.fb.control(
          {
            typeFormId: '',
            purposeId: 2
          }
        );
     
    
    this.haySamples.push(group);
  }


  cropRemoved(event:number){
    this.haySamples.removeAt(event);
  }


  addressSelected(event:FarmerAddress){
    this.selectedAddress = event;
    this.addressBrowserOpen = false;
  }

  addressSelectionCanceled(){

  }

  onSubmit(){

    

  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

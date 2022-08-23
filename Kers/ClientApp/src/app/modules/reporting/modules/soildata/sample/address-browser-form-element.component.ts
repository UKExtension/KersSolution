import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TypeForm } from '../soildata.report';
import { FarmerAddress } from '../soildata.service';
import { SampleAttribute, SampleAttributeSampleInfoBundle, SampleAttributeType, SampleInfoBundle } from './SampleInfoBundle';
import { SoilSampleService } from './soil-sample.service';



@Component({
  selector: 'address-browser-form-element',
  styles: [`
  .address-browser{
    border: 1px solid #e5e5e5;
    padding: 15px;
    border: 1px solid #CE5454;
  }


  `],
  template: `

  

                <div *ngIf="!addressBrowserOpen">
                  <br>
                  <soildata-list-address [address]="selectedAddress" [brief]="false"></soildata-list-address>
                  <a class="btn btn-info btn-xs" (click)="openBrowser()">change</a>
                </div>
                <div *ngIf="addressBrowserOpen" class="address-browser">
                    <h4>Select client from the list or enter a new one</h4><br>   
                    <soildata-address-browser [close]="false" (onSelected)="addressSelected($event)" (onCanceled)="addressSelectionCanceled()"></soildata-address-browser>
                </div>



  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => AddressBrowserFormElementComponent),
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
export class AddressBrowserFormElementComponent extends BaseControlValueAccessor<FarmerAddress> implements ControlValueAccessor, OnInit { 
    selectedAddress:FarmerAddress;
    addressBrowserOpen = true;
    constructor( 
      
    )   
    {
      super();

      
    }
    

    ngOnInit(){
       
    }




    openBrowser(){
        this.addressBrowserOpen = true;
        this.selectedAddress = null;
        this.onChange(null);
    }

    writeValue(address: FarmerAddress) {
        if( address != null){
            this.addressSelected(address);
        }
    }

    addressSelected(address: FarmerAddress){
        this.onChange(address);
        this.selectedAddress = address;
        this.addressBrowserOpen = false;
    }

    addressSelectionCanceled(){

    }
/* 

    validate(c: AbstractControl): ValidationErrors | null{
      return this.sampleForm.valid ? null : { invalidForm: {valid: false, message: "Session fields are invalid"}};
    }
 */

}
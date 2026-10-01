import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CountyCode, FarmerAddress } from '../soildata.service';

@Component({
    selector: 'soildata-farmer-address-detail',
    template: `
  <div class="ln_solid"></div>
  <div class="row">
    <div class="col-xs-10">
      @if (rowDefault) {
        <soildata-list-address [address]="address"></soildata-list-address>
      }
      @if (rowEdit) {
        <div class="col-xs-12">
          <soildata-farmer-address-form [address]="address" [selectedCounty]="selectedCounty" (onFormCancel)="default()" (onFormSubmit)="addressSubmitted($event)"></soildata-farmer-address-form>
        </div>
      }
  
  
    </div>
    <div class="col-xs-2 text-right">
      @if (rowDefault) {
        <a class="btn btn-info btn-xs" (click)="edit()">edit</a>
      }
      @if (!rowDefault) {
        <a class="btn btn-info btn-xs" (click)="default()">close</a>
      }
    </div>
  </div>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SoildataFarmerAddressDetailComponent implements OnInit {

  @Input() address: FarmerAddress;
  @Input() selectedCounty:CountyCode;

  rowDefault = true;
  rowEdit = false;

  @Output() onEdited = new EventEmitter<FarmerAddress>();

  constructor() { }

  ngOnInit() {
  }
  edit(){
    this.rowEdit = true;
    this.rowDefault = false;
  }
  default(){
    this.rowDefault = true;
    this.rowEdit = false;
  }

  addressSubmitted(event:FarmerAddress){
    this.address = event;
    this.default();
    this.onEdited.emit(event);
  }

}

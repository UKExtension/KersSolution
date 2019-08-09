import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FarmerAddress } from '../soildata.service';

@Component({
  selector: 'soildata-farmer-address-detail',
  template: `
  <div class="ln_solid"></div>
  <div class="row">
      <div class="col-xs-10">
          
          <article class="media event" *ngIf="rowDefault">
              <div class="media-body">
              <a class="title">{{address.first}} {{address.last}}</a>
              <p>Address: {{address.address}}, {{address.city}} {{address.st}}</p>
              </div>
          </article>
          <div class="col-xs-12" *ngIf="rowEdit">
              <soildata-farmer-address-form [address]="address" (onFormCancel)="default()" (onFormSubmit)="addressSubmitted($event)"></soildata-farmer-address-form>
          </div>

          
      </div>
      <div class="col-xs-2 text-right">
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault">edit</a>
          <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
      </div>  
  </div>
  `,
  styles: []
})
export class SoildataFarmerAddressDetailComponent implements OnInit {

  @Input() address: FarmerAddress;

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

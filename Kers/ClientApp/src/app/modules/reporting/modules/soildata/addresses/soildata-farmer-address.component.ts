import { Component, OnInit } from '@angular/core';
import { FarmerAddress, SoildataService } from './../soildata.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-soildata-farmer-address',
  template: `
    <br>
    <h3>Farmer Addresses</h3>
    <br>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newAddress" (click)="newAddress = true">+ new address</a>
    </div>
    <soildata-farmer-address-form *ngIf="newAddress" (onFormCancel)="newAddress=false" (onFormSubmit)="newAddressSubmitted($event)"></soildata-farmer-address-form>
    <soildata-farmer-address-detail *ngFor="let address of addresses | async" [address]="address"></soildata-farmer-address-detail>
  `,
  styles: []
})
export class SoildataFarmerAddressComponent implements OnInit {

  addresses:Observable<FarmerAddress[]>;
  newAddress = false;

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    this.addresses = this.service.addresses();
  }
  newAddressSubmitted(_:FarmerAddress){
    this.newAddress = false;
    this.addresses = this.service.addresses();
  }

}

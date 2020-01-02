import { Component, OnInit, Output, EventEmitter} from '@angular/core';
import { FarmerAddress, SoildataService } from '../soildata.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'soildata-address-browser',
  template: `
    <div class="ln_solid"></div>

    <div class="row">
      <div class="col-xs-12">
        <a class="btn btn-info btn-xs pull-right" (click)="canceled()">close</a>
      </div>
      <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let address of addresses | async"><br>
      {{address.first}} {{address.last}}<br>
      {{address.address}}<br>
      {{address.city}}, {{address.state}} {{address.zip}} <br><br>
      <a class="btn btn-info btn-xs" (click)="selected(address)">select</a>
      </div>
    </div>
    <div class="row"><br>
      <div class="col-xs-12">
        <a class="btn btn-info btn-xs pull-right" (click)="newAddress = true" *ngIf="!newAddress">+ new address</a>
      </div>
      <div class="col-xs-12" *ngIf="newAddress">
        <div class="ln_solid"></div>
        <soildata-farmer-address-form (onFormCancel)="newAddress = false" (onFormSubmit)="selected($event)"></soildata-farmer-address-form>
      </div>
    </div>
    
    <div class="ln_solid"></div>
  `,
  styles: []
})
export class SoildataAddressBrowserComponent implements OnInit {
  
  @Output() onSelected = new EventEmitter<FarmerAddress>();
  @Output() onCanceled = new EventEmitter<void>();
  addresses:Observable<FarmerAddress[]>;
  newAddress = false;

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    this.addresses = this.service.addresses();
  }
  selected(address:FarmerAddress){
    this.onSelected.emit(address);
  }
  canceled(){
    this.onCanceled.emit();
  }

}

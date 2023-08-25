import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FarmerAddress } from '../soildata.service';

@Component({
  selector: 'soildata-list-address',
  templateUrl: './soildata-list-address.component.html',
  styles: [
  ]
})
export class SoildataListAddressComponent implements OnInit {
  @Input() address: FarmerAddress;
  @Input() brief:boolean = true;
  @Input() select:boolean = false;
  @Input() edit:boolean = false;

  public isEdit:boolean = false;
  coppied = false;


  @Output() onSelected = new EventEmitter<FarmerAddress>();

  constructor() { }

  ngOnInit(): void {
  }
  selected(){
    this.onSelected.emit(this.address);
  }
  edited(newaddress:FarmerAddress){
    this.address = newaddress;
    this.isEdit = false;
    this.onSelected.emit(this.address);
  }
  public notify(payload: string) {
    this.coppied = true;
  }
  
}

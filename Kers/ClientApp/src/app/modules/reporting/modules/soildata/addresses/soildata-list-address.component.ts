import { Component, Input, OnInit } from '@angular/core';
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

  constructor() { }

  ngOnInit(): void {
  }

}

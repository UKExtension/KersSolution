import { Component, Input, OnInit } from '@angular/core';
import { ExtensionEventLocation, PhysicalAddress } from '../../../events/extension-event';

@Component({
  selector: 'address-details',
  template: `
  <div><strong>{{address.displayName}}</strong></div>
  <div *ngIf="address.address.building">{{address.address.building}}</div>
  <div>{{address.address.street}}</div>
  <div>{{address.address.city}}<span *ngIf="address.address.state && address.address.state != ''">, {{address.address.state}}</span> {{address.address.postalCode}}
  `,
  styles: [
  ]
})
export class AddressDetailsComponent implements OnInit {
  @Input() address:ExtensionEventLocation;

  constructor() { }

  ngOnInit(): void {
  }

}

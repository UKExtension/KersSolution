import { Component, Input, OnInit } from '@angular/core';
import { Mileage } from '../../../mileage/mileage';

@Component({
  selector: 'trip-details',
  template: `
    <div class="row">
      <div class="col-xs-12">
        <h4>Starting location:</h4>
        <address-details [address]="trip.startingLocation"></address-details>
      </div>
    </div>
    <div style="border-bottom: 2px solid #bbb; padding-bottom: 6px;">
      <div class="trip-segment" *ngFor="let segment of trip.segments">
        <trip-segment [segment]="segment"></trip-segment>
      </div>
    </div>
  `,
  styles: [
    `
    h4{
    border-bottom: 1px solid #ccc;
    padding: 20px 0 9px;
    }
    `
  ]
})
export class TripDetailsComponent implements OnInit {
  @Input() trip:Mileage;

  constructor() { }

  ngOnInit(): void {
  }

}

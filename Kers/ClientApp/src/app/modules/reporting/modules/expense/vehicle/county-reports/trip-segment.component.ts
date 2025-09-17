import { Component, Input, OnInit } from '@angular/core';
import { MileageSegment } from '../../../mileage/mileage';

@Component({
  selector: 'trip-segment',
  template: `
  <h4>{{segment.businessPurpose}}</h4>
   <div class="row">
   <div class="col-xs-12 col-sm-4">
   <address-details [address]="segment.location"></address-details>
   </div>
   <div class="col-xs-12 col-sm-4">
   {{segment.programCategory.shortName}}
   </div>
   <div class="col-xs-12 col-sm-4">
   <span style="font-size:1.5em;font-weight:bold;">{{segment.mileage}}</span> miles
   </div>
   </div>
  `,
  styles: [
    `
    h4{
    border-bottom: 1px solid #ccc;
    padding: 10px 0 9px;
    }
    `
  ]
})
export class TripSegmentComponent implements OnInit {
  @Input() segment:MileageSegment;

  constructor() { }

  ngOnInit(): void {
  }

}

import { Component, Input, OnInit } from '@angular/core';
import { Expense } from '../../expense.service';
import { MileageBundle } from '../../../mileage/mileage';

@Component({
  selector: 'county-vehicle-trip',
  template: `
    <article class="media event" style="padding-top: 29px;">
      <a class="pull-left date">
        <p class="month">{{expense.expenseDate | date: 'LLL'}}</p>
        <p class="day">{{expense.expenseDate | date: 'd'}}</p>
      </a>
      <div class="media-body">
        <div class="row">
          <div class="col-xs-10 col-sm-5">
          Driven by: <br><strong>{{expense.kersUser.rprtngProfile.name}}</strong>
          </div>
          <div class="col-xs-10 col-sm-5">
            <p><span style="font-size:1.5em;font-weight:bold;">{{totalMiles | number}}</span> miles</p>
          </div>
          <div class="col-xs-2 text-right">
            <button *ngIf="!showDetails" type="button" class="btn btn-secondary btn-sm" (click)="showDetails = !showDetails">trip details</button>
          </div>
          
        </div>
      </div>
    </article>
    <div class="row" *ngIf="showDetails">
      <div class="col-xs-10">
        <trip-details [trip]="expense.lastRevision"></trip-details>
      </div>
      <div class="col-xs-2 text-right">
        <button type="button" class="btn btn-secondary btn-sm" (click)="showDetails = !showDetails">close</button>
      </div>
    </div>
  `,
  styles: [
  ]
})
export class CountyVehicleTripComponent implements OnInit {
  @Input() expense:MileageBundle;
  showDetails:boolean = false;
  totalMiles:number = 0;

  constructor() { }

  ngOnInit(): void {
    for( let segment of this.expense.lastRevision.segments){
      this.totalMiles += segment.mileage;
    }
  }

}

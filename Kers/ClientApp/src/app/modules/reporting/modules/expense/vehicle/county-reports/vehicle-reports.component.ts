import { Component, Input, OnInit } from '@angular/core';
import { Vehicle, VehicleService } from '../vehicle.service';
import { Expense } from '../../expense.service';

@Component({
  selector: 'vehicle-reports',
  template: `
    <article class="media event">
              <div class="media-body">
              <a class="title" [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(35, 82, 124);' : '#ccc' }">{{vehicle.year}} {{vehicle.make}}<span *ngIf="vehicle.name != undefined && vehicle.name != ''"> ({{vehicle.name}})</span></a>
              <p [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(115, 135, 156)' : '#ccc' }">{{vehicle.model}}</p>
              </div>
     </article>
     <article>
     <h5>Trips</h5>
     {{trips | json}}
     </article>
  `,
  styles: [
  ]
})
export class VehicleReportsComponent implements OnInit {
  @Input() vehicle:Vehicle;
  trips:Expense[];
  constructor(
    private service:VehicleService
  ) { }

  ngOnInit(): void {
    this.service.trips(this.vehicle).subscribe(
      res => this.trips = res
    );
  }

}

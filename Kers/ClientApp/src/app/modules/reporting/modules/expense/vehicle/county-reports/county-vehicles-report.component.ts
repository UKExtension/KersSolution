import { Component, Input, OnInit } from '@angular/core';
import { PlanningUnit } from '../../../user/user.service';

@Component({
  selector: 'county-vehicles-reports',
  template: `
    <div class="row" *ngIf="county && county.vehicles && county.vehicles.length > 0">
        <div class="col-md-12">
            <div class="x_panel">
            <div class="x_title" style="border-bottom:none; margin-bottom:-20px;">
                <h2>County Vehicles</h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div *ngIf="county">
                  <vehicle-list-detail *ngFor="let vehicle of county.vehicles" [vehicle]="vehicle" [trips]="true"></vehicle-list-detail>
                </div> 
            </div>
        </div>
    </div>
  `,
  styles: [
  ]
})
export class CountyVehiclesReportComponent implements OnInit {

  @Input() county:PlanningUnit;
  constructor(
    
  ) { }

  ngOnInit(): void {
  }

}

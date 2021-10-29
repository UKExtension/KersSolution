import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../../modules/user/user.service';
import { Vehicle } from '../../../modules/expense/vehicle/vehicle.service';



@Component({
    selector: 'widget-activities-agent',
    template: `
    <div class="col-md-6 col-xs-12">
        <div class="x_panel">
          <div class="x_title">
            <h2>Report Activities</h2>
            <div class="clearfix"></div>
          </div>
          <div class="x_content" *ngIf="enabledVehicles">
            <a routerLink="/reporting/servicelog" class="btn btn-dark btn-lg btn-block">Service Log</a>
            <a routerLink="/reporting/indicators" class="btn btn-dark btn-lg btn-block">Program Indicators</a>
            <a routerLink="/reporting/mileage" *ngIf="!(enabledVehicles.length > 0)" class="btn btn-dark btn-lg btn-block">Mileage Records</a>
            <a routerLink="/reporting/mileage/bytype/new" *ngIf="enabledVehicles.length > 0" class="btn btn-dark btn-lg btn-block">Mileage Records Personal Vehicle</a>
            <a routerLink="/reporting/mileage/bytype/newcountyvehicle" *ngIf="enabledVehicles.length > 0" class="btn btn-dark btn-lg btn-block">Mileage Records County Vehicle</a>
            <a routerLink="/reporting/story" class="btn btn-dark btn-lg btn-block">Success Stories</a>
          </div>
        </div>
    </div>
  `
})
export class WidgetActivitiesAgentComponent { 
  @Input() enabledVehicles:Vehicle[];
  ngOnInit(){

  }

}
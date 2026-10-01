import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { Vehicle } from '../../../modules/expense/vehicle/vehicle.service';



@Component({
    selector: 'widget-staff-assistant',
    template: `
    <div class="col-md-6 col-xs-12">
      <div class="x_panel">
        <div class="x_title">
          <h2>Report Activities</h2>
          <div class="clearfix"></div>
        </div>
        <div class="x_content">
          <a routerLink="/reporting/servicelog" class="btn btn-dark btn-lg btn-block">Service Log</a>
          @if (!(enabledVehicles.length > 0)) {
            <a routerLink="/reporting/expense" class="btn btn-dark btn-lg btn-block">Mileage Records</a>
          }
          @if (enabledVehicles.length > 0) {
            <a routerLink="/reporting/expense/bytype/new" class="btn btn-dark btn-lg btn-block">Mileage Records Personal Vehicle</a>
          }
          @if (enabledVehicles.length > 0) {
            <a routerLink="/reporting/expense/bytype/newcountyvehicle" class="btn btn-dark btn-lg btn-block">Mileage Records County Vehicle</a>
          }
          <a routerLink="/reporting/story" class="btn btn-dark btn-lg btn-block">Success Stories</a>
        </div>
      </div>
    </div>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class WidgetStaffAssistantComponent { 
  @Input() enabledVehicles:Vehicle[];

}
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { VehicleService, Vehicle } from './vehicle.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { PlanningUnit } from '../../user/user.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { switchMap } from 'rxjs/operators';

@Component({
    selector: 'app-vehicle-county',
    template: `
  <div>
    <div class="text-right">
      @if (!newVehicle) {
        <a class="btn btn-info btn-xs" (click)="newVehicle = true">+ new vehicle record</a>
      }
    </div>
    @if (newVehicle) {
      <vehicle-form [county]="county" (onFormCancel)="newVehicle=false" (onFormSubmit)="newVehicleSubmitted($event)"></vehicle-form>
    }
  </div>
  @if (county) {
    <div>
      @for (vehicle of county.vehicles; track vehicle) {
        <vehicle-list-detail [vehicle]="vehicle" (onEdited)="vehicleEdited($event)"></vehicle-list-detail>
      }
    </div>
  }
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class VehicleCountyComponent implements OnInit {

  county:PlanningUnit;
  errorMessage: string;
  newVehicle:boolean = false;

  constructor(
    private route: ActivatedRoute,
    private planningUnitService: PlanningunitService,
    private service: VehicleService,
    private reportingService: ReportingService,
  ) { }

  ngOnInit() {
    this.route.params.pipe(
            switchMap( (params: Params) => this.planningUnitService.id(params['id'], true) )
          ).subscribe(
            res=>{
              this.county = res;
              this.defaultTitle();
            },
            err => this.errorMessage = <any>err
          );
  }
  newVehicleSubmitted(event:Vehicle){
    this.county.vehicles.unshift(event);
    this.newVehicle = false;
  }
  vehicleEdited(event:Vehicle){
    this.planningUnitService.id(this.county.id, true) 
          .subscribe(
            res=>{
              this.county = res;
            },
            err => this.errorMessage = <any>err
          );
  }
  defaultTitle(){
    this.reportingService.setTitle(this.county.name + " Vehicles");
  }

}

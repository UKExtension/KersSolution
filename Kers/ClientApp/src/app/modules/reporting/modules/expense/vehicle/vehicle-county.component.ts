import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { VehicleService, Vehicle } from './vehicle.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { PlanningUnit } from '../../user/user.service';

@Component({
  selector: 'app-vehicle-county',
  template: `
  <div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newVehicle" (click)="newVehicle = true">+ new vehicle record</a>
    </div>
    <vehicle-form *ngIf="newVehicle" [county]="county" (onFormCancel)="newVehicle=false" (onFormSubmit)="newVehicleSubmitted($event)"></vehicle-form>
  </div>
  <div *ngIf="county">
    <vehicle-list-detail *ngFor="let vehicle of county.vehicles" [vehicle]="vehicle"></vehicle-list-detail>
  </div>
    `,
  styles: []
})
export class VehicleCountyComponent implements OnInit {

  county:PlanningUnit;
  errorMessage: string;
  newVehicle:boolean = false;

  constructor(
    private route: ActivatedRoute,
    private planningUnitService: PlanningunitService,
    private service: VehicleService
  ) { }

  ngOnInit() {
    this.route.params
        .switchMap( (params: Params) => this.planningUnitService.id(params['id']) )
          .subscribe(
            res=>{
              this.county = res;
            },
            err => this.errorMessage = <any>err
          );
  }
  newVehicleSubmitted(event:Vehicle){
    this.county.vehicles.unshift(event);
    console.log(this.county);
    this.newVehicle = false;
  }

}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { VehicleService } from './vehicle.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';

@Component({
  selector: 'app-vehicle-county',
  template: `
    <p>
      vehicle-county works!
    </p>
  `,
  styles: []
})
export class VehicleCountyComponent implements OnInit {

  county:PlanningUnit;
  errorMessage: string;

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
              console.log(this.county)
            },
            err => this.errorMessage = <any>err
          );
  }

}

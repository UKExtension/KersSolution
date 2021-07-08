import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

import { ActivatedRoute, Params } from "@angular/router";
import { CountyService } from "./county.service";
import { PlanningUnit } from "../user/user.service";
import { Plan, PlansofworkService, PlanOfWork } from "../plansofwork/plansofwork.service";
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  template: `


  <div *ngIf="county">
    <div class="row x_title">
        <div class="col-md-6">
        <h3>Employees</h3>
        </div>
                  
   </div>

    <user-directory-list [county]="county" [showEmployeeSummaryButton]="true"></user-directory-list>


  </div>
    
  `
})
export class UnitHomeComponent { 

    id:number = 0;
    county: PlanningUnit;

    constructor( 
        private reportingService: ReportingService,
        private service:CountyService,
        private route: ActivatedRoute,
    )   
    {}

    ngOnInit(){
        
        
        this.route.params.pipe(
                    switchMap(  (params: Params) =>
                                    {
                                        if(params['id'] != undefined){
                                            this.id = params['id'];
                                        }
                                        return this.service.get(this.id);
                                    } 
                            )
                    )
                    .subscribe(
                        county => {
                            this.county = <PlanningUnit>county;
                            this.defaultTitle();
                            /*
                            var go = JSON.parse(this.county.geoFeature);
                            var center = this.service.geoCenter(go.geometry.coordinates[0][0]);
                            */
                        }
                    );

        
   
    }

    ngOnDestroy(){
        this.reportingService.setTitle( this.county.name );
        this.reportingService.setSubtitle('');
    }

    defaultTitle(){
        this.reportingService.setTitle(this.county.name );
    }
}
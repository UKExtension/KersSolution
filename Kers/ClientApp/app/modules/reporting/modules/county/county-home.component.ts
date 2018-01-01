import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

import { ActivatedRoute, Params } from "@angular/router";
import { CountyService } from "./county.service";
import { PlanningUnit } from "../user/user.service";
import { Plan, PlansofworkService, PlanOfWork } from "../plansofwork/plansofwork.service";
import { Observable } from "rxjs/Observable";

@Component({
  template: `



<!--

<div class="row tile_count" *ngIf="county">
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-user"></i> Population</span>
              <div class="count">{{county.population | number}}</div>
              <span class="count_bottom"><i class="green">4% </i> From last Week</span>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-clock-o"></i> Average Time</span>
              <div class="count">123.50</div>
              <span class="count_bottom"><i class="green"><i class="fa fa-sort-asc"></i>3% </i> From last Week</span>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-user"></i> Total Males</span>
              <div class="count green">2,500</div>
              <span class="count_bottom"><i class="green"><i class="fa fa-sort-asc"></i>34% </i> From last Week</span>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-user"></i> Total Females</span>
              <div class="count">4,567</div>
              <span class="count_bottom"><i class="red"><i class="fa fa-sort-desc"></i>12% </i> From last Week</span>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-user"></i> Total Collections</span>
              <div class="count">2,315</div>
              <span class="count_bottom"><i class="green"><i class="fa fa-sort-asc"></i>34% </i> From last Week</span>
            </div>
            <div class="col-md-2 col-sm-4 col-xs-6 tile_stats_count">
              <span class="count_top"><i class="fa fa-user"></i> Total Connections</span>
              <div class="count">7,325</div>
              <span class="count_bottom"><i class="green"><i class="fa fa-sort-asc"></i>34% </i> From last Week</span>
            </div>
          </div>



-->

  <div *ngIf="county">
    <div class="text-right"><a class="btn btn-default btn-xs" routerLink="/reporting/state/district/{{county.districtId}}">District Admin Dashboard</a></div>
    <div class="row x_title">
        <div class="col-md-6">
        <h3>Employees</h3>
        </div>
                  
   </div>

    <user-directory-list [county]="county" [showEmployeeSummaryButton]="true"></user-directory-list>



    <h2>Plans of Work</h2>
    <div *ngIf="plans">
        <table class="table table-striped">
            <tbody>
                <tr *ngFor="let planofwork of plans | async" [isReport]="true" [planofworkDetail]="planofwork" (onPlanofworkUpdated)="onPlanofworkUpdate()" (onPlanofworkDeleted)="onPlanofworkUpdate()" ></tr>
            </tbody>               
        </table>            
    </div>
    <h2>Affirmative Action Plan</h2>
    <table class="table table-striped">
        <tr *ngIf="!aa"><td class="text-right"><button class="btn btn-info btn-xs" (click)="aa = !aa" >view</button></td></tr>
        <tr *ngIf="aa"><td class="text-right"><button class="btn btn-info btn-xs" (click)="aa = !aa">close</button></td></tr>
        <tr *ngIf="aa"><td><affirmative-report [unitId]="county.id"></affirmative-report></td></tr>
    </table>
    

  </div>
    
  `
})
export class CountyHomeComponent { 

    id:number = 0;
    county: PlanningUnit;
    plans:Observable<PlanOfWork[]>;

    constructor( 
        private reportingService: ReportingService,
        private service:CountyService,
        private plansofworkService: PlansofworkService,
        private route: ActivatedRoute,
    )   
    {}

    ngOnInit(){
        
        
        this.route.params
            .switchMap(  (params: Params) =>
                            {
                                if(params['id'] != undefined){
                                    this.id = params['id'];
                                }
                                return this.service.get(this.id);
                            } 
                    )
                    .subscribe(
                        county => {
                            this.county = <PlanningUnit>county;
                            this.plans = this.plansofworkService.plansForCounty(this.county.id);
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
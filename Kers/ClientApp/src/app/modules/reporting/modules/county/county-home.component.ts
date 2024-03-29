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
    <div class="text-right" *ngIf="returnto == 'district'"><a class="btn btn-default btn-xs" routerLink="/reporting/state/district/{{county.districtId}}">District Admin Dashboard</a></div>
    <div class="text-right" *ngIf="returnto == 'area'"><a class="btn btn-default btn-xs" routerLink="/reporting/extensionarea">Area Admin Dashboard</a></div>
    <div class="text-right" *ngIf="returnto == 'region'"><a class="btn btn-default btn-xs" routerLink="/reporting/extensionregion">Region Admin Dashboard</a></div>
    <div class="row x_title">
        <div class="col-md-6">
        <h3>Employees</h3>
        </div>
                  
   </div>


    <user-directory-list [county]="county" [showEmployeeSummaryButton]="true"></user-directory-list>
    <div class="row" *ngIf="county">
        <div class="col-xs-12">
            <div class="x_panel">
                <div class="x_title">
                    <h2>Number of Submitted Activities per Agent</h2>
                    <div class="clearfix"></div>
                </div>
                <div class="x_content">
                    <district-employees [county]="county"></district-employees>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="x_panel">
            <div class="x_title">
                <h2>Plans of Work<small>by Fiscal Year</small></h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div *ngIf="county">
                    <plansofwork-reports [planningUnitId]="county.id"></plansofwork-reports>          
                </div>  
                
            </div>
        </div>
    </div>


        <div class="col-md-12">
            <div class="x_panel">
            <div class="x_title">
                <h2>Affirmative Action<small>Plans and Reports</small></h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div *ngIf="county">
                    <div class="text-right">
                        <div *ngIf="!affOpen"><button (click)="affOpen=true" class="btn btn-info btn-xs">view</button></div>
                        <div *ngIf="affOpen"><button (click)="affOpen=false" class="btn btn-info btn-xs">close</button></div>
                    </div>
                    <affirmative-report [unitId]="county.id" *ngIf="affOpen"></affirmative-report>       
                </div>  
                
            </div>
        </div>




    

  </div>

    










  












    
  `
})
export class CountyHomeComponent { 

    id:number = 0;
    county: PlanningUnit;
    onlyEnabled:boolean = true;
    returnto:string = 'area';

    constructor( 
        private reportingService: ReportingService,
        private service:CountyService,
        private plansofworkService: PlansofworkService,
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
                                        if(params['returnto'] != undefined){
                                            this.returnto = params['returnto'];
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

    includeLeftChecked(){
        this.onlyEnabled = !this.onlyEnabled;
    }

    ngOnDestroy(){
        this.reportingService.setTitle( this.county.name );
        this.reportingService.setSubtitle('');
    }

    defaultTitle(){
        this.reportingService.setTitle(this.county.name );
    }
}
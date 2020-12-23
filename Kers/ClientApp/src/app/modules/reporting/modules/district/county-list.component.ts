import { Component, Input } from '@angular/core';
import { DistrictService, County, District } from './district.service';

import {Observable} from 'rxjs/Observable';
import { ActivatedRoute, Params } from "@angular/router";
import { StateService } from '../state/state.service';
import { PlanningUnit } from '../user/user.service';
import { AreaService } from '../area/area.service';

@Component({
    selector: 'county-list',
    template: `
    <div class="col-xs-12">
        <div class="x_panel">
        <div class="x_title">
            <h2 *ngIf="district != null">Counties</h2>
            <h2 *ngIf="district == null">Units</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content" *ngIf="counties">

                <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let county of counties | async">
                        <a *ngIf="district != null || type=='area'" [routerLink]="['/reporting/county', county.id]" class="btn btn-dark btn-lg btn-block">{{county.name.substring(0, county.name.length - 11)}}</a>
                        <a *ngIf="district == null && type!='area'" [routerLink]="['/reporting/county/unit', county.id]" class="btn btn-dark btn-lg btn-block">{{county.name}}</a>
                </div>
                
            </div>
        </div>
    </div>

<!--
    <div class="col-md-6 col-sm-6 col-xs-12">
        <div class="x_panel">
        <div class="x_title">
            <h2>Counties without Affirmative Action Plan</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content" *ngIf="countiesNoAa">
            <ul class="to_do">
                <li *ngFor="let countyN of countiesNoAa" >
                    <a [routerLink]="['/county', countyN.id]"><p>
                       {{countyN.name.substring(0, countyN.name.length - 11)}} 
                    </p></a>
                </li>
                
            </ul>
            </div>
        </div>
    </div>


    <div class="col-md-6 col-sm-6 col-xs-12">
        <div class="x_panel">
        <div class="x_title">
            <h2>Counties Without Plans of Work Submission</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content" *ngIf="countiesNoPl">
            <ul class="to_do">
                <li *ngFor="let countyP of countiesNoPl" >
                    <a [routerLink]="['/county', countyP.id]"><p>
                       {{countyP.name.substring(0, countyP.name.length - 11)}} 
                    </p></a>
                </li>
                
            </ul>
            </div>
        </div>
    </div>

-->

    
  `
})
export class CountyListComponent { 

    @Input() district:District | null = null;
    @Input() type:string = 'district';
    @Input() areaId:number = 0;
    counties:Observable<PlanningUnit[]>;
    countiesNoAa:County[];
    countiesNoPl:County[];

    errorMessage:string;

    constructor( 
        private service:DistrictService,
        private stateService:StateService,
        private areaService:AreaService,
        private route: ActivatedRoute,
    )   
    {}

    ngOnInit(){
        if(this.type == 'area'){
            this.counties = this.areaService.counties(this.areaId);
        }else{
            if( this.district == null){
                this.counties = this.stateService.notCounties();
            }else{
                this.counties = this.service.counties(this.district.id);
            }
        }
        
        
        
/*

        this.route.params
            .switchMap((params: Params) => this.service.countiesNoPl(+params['id']))
            .subscribe(counties => this.countiesNoPl = <County[]>counties);



        this.route.params
            .switchMap((params: Params) => this.service.countiesNoAa(+params['id']))
            .subscribe(counties => this.countiesNoAa = <County[]>counties);
*/


        /*
    this.route.params
            .switchMap((params: Params) => {
                
                    console.log(params);
                return this.service.counties(+params['id']);
            })
            .subscribe(counties => this.counties = <County[]>counties);
 */

    }
       

}
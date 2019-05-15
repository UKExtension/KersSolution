import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

import { ActivatedRoute, Params, Router } from "@angular/router";
import { DistrictService, District } from "./district.service";
import { StateService } from "../state/state.service";

import * as echarts from 'echarts';
import { CountyService } from '../county/county.service';
import { PlanningUnit } from '../user/user.service';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';


@Component({
  template: `
    <div class="text-right"><a class="btn btn-default btn-xs" routerLink="/reporting/state">State Admin Dashboard</a></div>
    <div><reporting-display-help id="7"></reporting-display-help></div>
    <county-list [district]="district" *ngIf="district"></county-list>

    <div class="col-xs-12">
        <div class="x_panel">
            <div class="x_title">
                <h2>Number of Submitted Activities per Agent</h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <district-employees *ngIf="district" [district]="district"></district-employees>
            </div>
        </div>
    </div>


    <div class="col-xs-12" *ngIf="lastFiscalYear && currentFiscalYear">
        <div class="x_panel">
        <div class="x_title">
            <h2>Assignments</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content">

        
        <div class="row">
            <div class="col-xs-10" *ngIf="!assignmentPlansOfWorkOpen">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Plans of Work</a>
                    <p>List of counties with no FY{{lastFiscalYear.name}} Plans of Work</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right" *ngIf="!assignmentPlansOfWorkOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=true">show</a>                
            </div>
            <div class="col-xs-12 text-right" *ngIf="assignmentPlansOfWorkOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=false">close</a>                
            </div>
            <assignment-plans-of-work [districtId]="district.id" [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentPlansOfWorkOpen"></assignment-plans-of-work>  
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10"  *ngIf="!assignmentAffirmativeReportOpen">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Affirmative Action Report</a>
                    <p>List of counties with no FY{{currentFiscalYear.name}} Affirmative Action Report</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right" *ngIf="!assignmentAffirmativeReportOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentAffirmativeReportOpen=true">show</a>                
            </div>
            <div class="col-xs-12 text-right" *ngIf="assignmentAffirmativeReportOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentAffirmativeReportOpen=false">close</a>                
            </div>
            <assignment-affirmative-report [districtId]="district.id" [fiscalYearId]="currentFiscalYear.name"  *ngIf="assignmentAffirmativeReportOpen"></assignment-affirmative-report>
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10" *ngIf="!assignmentAffirmativePlanOpen">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Affirmative Action Plan</a>
                    <p>List of counties with no FY{{lastFiscalYear.name}} Affirmative Action Plan</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right" *ngIf="!assignmentAffirmativePlanOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentAffirmativePlanOpen=true">show</a>                
            </div>
            <div class="col-xs-12 text-right" *ngIf="assignmentAffirmativePlanOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentAffirmativePlanOpen=false">close</a>                
            </div>
            <assignment-affirmative-plan [districtId]="district.id" [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentAffirmativePlanOpen"></assignment-affirmative-plan>  
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10" *ngIf="!assignmentProgramIndicatorsOpen">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Program Indicators</a>
                    <p>List of counties that submitted no Program Indicators for FY{{currentFiscalYear.name}}.</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right" *ngIf="!assignmentProgramIndicatorsOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentProgramIndicatorsOpen=true">show</a>                
            </div>
            <div class="col-xs-12 text-right" *ngIf="assignmentProgramIndicatorsOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentProgramIndicatorsOpen=false">close</a>                
            </div>
            <assignment-program-indicators [districtId]="district.id" [fiscalYearId]="currentFiscalYear.name" *ngIf="assignmentProgramIndicatorsOpen"></assignment-program-indicators>  
        </div>




        </div>
        </div>
    </div>

    <div class="col-xs-12">
        <div class="x_panel">
        <div class="x_title">
            <h2>District population</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content">

            <div [ts-chart]="option" style="height:400px;" (chartClick)="countyClick($event)"></div>
        </div>
        </div>
    </div>



    <router-outlet></router-outlet>
    
  `
})
export class DistrictHomeComponent { 

    errorMessage:string;
    district: District;
    counties:PlanningUnit[];
    id:number = 0;

    districtMap = '{"type": "FeatureCollection", "features": ';
    populationData = [];
    option;
    geoCoordMap = {};
    data = [];
    categoryData = [];
    barData = [];

    assignmentPlansOfWorkOpen = false;
    assignmentAffirmativeReportOpen = false;
    assignmentAffirmativePlanOpen = false;
    assignmentProgramIndicatorsOpen = false;

    lastFiscalYear:FiscalYear;
    currentFiscalYear:FiscalYear;


    constructor( 
        private reportingService: ReportingService,
        private service:DistrictService,
        private stateService:StateService,
        private fiscalYearService:FiscalyearService,
        private countyService:CountyService,
        private route: ActivatedRoute,
        private router:Router
    )   
    {}

    convertedData = [
        this.convertData(this.data),
        this.convertData(this.data.sort(function(a, b) {
            return b.value - a.value;
        }).slice(0, 6))
    ];

    convertData(data) {
        var res = [];
        for (var i = 0; i < data.length; i++) {
            var geoCoord = this.geoCoordMap[data[i].name];
            if (geoCoord) {
                res.push({
                    name: data[i].name,
                    value: geoCoord.concat(data[i].value)
                });
            }
        }
        return res;
    }

    countyClick(event){
        var cnty = this.counties.filter( c => c.name.substring(0,c.name.length - 11 ) == event.region.name);
        if(cnty.length > 0){
            this.router.navigate(['/reporting/county/'+cnty[0].id])
        }
    }

    ngOnInit(){
        
        this.fiscalYearService.last().subscribe(
            res => this.lastFiscalYear = res
        )
        this.fiscalYearService.current().subscribe(
            res => this.currentFiscalYear = res
        );
        this.route.params
            .switchMap(  (params: Params) => {
                if(params['id'] != undefined){
                    this.id = params['id'];
                }
                return this.service.get(params['id']) 
            }
                            )
            .subscribe(
                district => {
                    this.district = <District>district;





                    var ftrs = [];
                    this.service.counties(this.district.id).subscribe(
                        res => {
                            this.counties = <PlanningUnit[]> res;
                            for(let cnt of res){
                                if(cnt.name.substring(cnt.name.length - 3) == "CES"){
                                    var countyName = cnt.name.substring(0, cnt.name.length - 11);
                                    
                                    this.data.push(
                                        {
                                            name: countyName,
                                            value: cnt.population
                                        }
                                    );
                                    
                                    if(cnt.geoFeature != undefined){
                                        var go = JSON.parse(cnt.geoFeature);
                                        ftrs.push(cnt.geoFeature);
                                        var center = this.countyService.geoCenter(go.geometry.coordinates[0][0]);
                                        this.geoCoordMap[countyName] = center;
                                    }else{
                                        this.geoCoordMap[countyName] = [0,0];
                                    }
                                }
                            }
                            this.data.sort(function(a, b) {
                                return a.value - b.value;
                            });
                            for( var d of this.data){
                                this.categoryData.push(d.name);
                                this.barData.push(d.value/1000);
                            }
                            
                            this.districtMap += "[" + ftrs.join(", ") + "]}";
                            
                            this.stateService.kyMap().subscribe(
                                res => {

                                    echarts.registerMap('KY', this.districtMap);
                                    
                                    this.option = {
                                        /*
                                        tooltip : {
                                            trigger: 'item',
                                            formatter: "{a} <br/>{b} : {c} ({d}%)"
                                        },
                                        */
                                        title: [{
                                            text: 'Population by County',
                                            textStyle: {color: '#eee'},
                                            subtext: 'Data from www.census.gov',
                                            x: '25%',
                                            y: '5',
                            
                                            textAlign: 'center'
                                        }, {
                                            text: 'Population Estimates',
                                            textStyle: {color: '#eee'},
                                            subtext: 'in thousands',
                                            x: '75%',
                                            y: '5',
                                            textAlign: 'center'
                                        }],                                        
                                        backgroundColor: '#394D5F',
                                        animation: true,
                                        animationDuration: 1000,
                                        animationEasing: 'cubicInOut',
                                        animationDurationUpdate: 1000,
                                        animationEasingUpdate: 'cubicInOut',
                                        tooltip:{
                                            trigger: 'item',
                                            formatter: function(params){
                                                if(params.seriesId == 'bar'){
                                                    return params.name + ': ' + Math.round(params.data * 1000);
                                                }else{
                                                    return params.data.name + ': ' + params.data.value[2] ;
                                                }
                                                
                                                
                                            }
                                        },
                                        
                                        "geo": {
                                        
                                            "map": "KY",
                                            left: '-10',
                                            right: '55%',
                                            zoom: 0.7,
                                            label: {
                                                emphasis: {
                                                    show: false
                                                }
                                            },
                                            
                                            roam: true,
                                            itemStyle: {
                                                normal: {
                                                    areaColor: '#323c48',
                                                    borderColor: '#111'
                                                },
                                                emphasis: {
                                                    areaColor: '#2a333d'
                                                }
                                            }
                                        },

                                        
                                        toolbox: {
                                            show : true,
                                            feature : {
                                                mark : {show: true},
                                                dataView : {
                                                    show: true, 
                                                    readOnly: false, 
                                                    title: 'Data',
                                                    lang: ['Data', 'Close', 'Refresh']
                                            },
                                                saveAsImage : {show: true, title: 'Image'}
                                            }
                                        },
                                        grid: {
                                            right: 40,
                                            top: 100,
                                            bottom: 40,
                                            width: '30%'
                                        },
                                        xAxis: {
                                            type: 'value',
                                            scale: true,
                                            position: 'top',
                                            boundaryGap: false,
                                            splitLine: {
                                                show: false
                                            },
                                            axisLine: {
                                                show: false
                                            },
                                            axisTick: {
                                                show: false
                                            },
                                            axisLabel: {
                                                margin: 2,
                                                textStyle: {
                                                    color: '#aaa'
                                                }
                                            },
                                        },
                                        yAxis: {
                                            type: 'category',
                                            //  name: 'TOP 20',
                                            nameGap: 16,
                                            axisLine: {
                                                show: true,
                                                lineStyle: {
                                                    color: '#ddd'
                                                }
                                            },
                                            axisTick: {
                                                show: false,
                                                lineStyle: {
                                                    color: '#ddd'
                                                }
                                            },
                                            axisLabel: {
                                                interval: 0,
                                                textStyle: {
                                                    color: '#ddd'
                                                }
                                            },
                                            data: this.categoryData
                                        },

                                        series: [
                                            
                                            {
                                                type: 'effectScatter',
                                                coordinateSystem: 'geo',
                                                symbolSize: function(val) {
                                                    return Math.max(val[2] / 3500, 8);
                                                },
                                                showEffectOn: 'render',
                                                rippleEffect: {
                                                    brushType: 'stroke'
                                                },
                                                hoverAnimation: true,
                                                label: {
                                                    normal: {
                                                        formatter: '{b}',
                                                        position: 'right',
                                                        show: true
                                                    }
                                                },
                                                itemStyle: {
                                                    normal: {
                                                        color: '#f4e925',
                                                        shadowBlur: 50,
                                                        shadowColor: '#EE0000'
                                                    }
                                                },
                                                zlevel: 1,
                                                data:this.convertData(this.data)                            
                                            },
                                            {
                                                id: 'bar',
                                                zlevel: 2,
                                                type: 'bar',
                                                symbol: 'none',
                                                itemStyle: {
                                                    normal: {
                                                        color: '#ddb926'
                                                    }
                                                },
                                        
                                                data: this.barData
                                            }
                                        ]
                                    };
                                },
                                err => this.errorMessage = <any> err
                            )
                        }
                    );
                    
                    this.defaultTitle();
                }
            );


    }

    ngOnDestroy(){
        this.reportingService.setTitle( 'Kentucky Extension Reporting System' );
        this.reportingService.setSubtitle('');
    }

    defaultTitle(){
        this.reportingService.setTitle(this.district.areaName );
        this.reportingService.setSubtitle(this.district.name);
    }

}
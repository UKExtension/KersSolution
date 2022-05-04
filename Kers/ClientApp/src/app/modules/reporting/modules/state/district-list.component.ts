import { Component } from '@angular/core';
import {StateService} from './state.service';

import {Observable} from 'rxjs';
import { District } from "../district/district.service";
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `
    <div class="col-md-12">
        <div class="x_panel">
            

            <navigate-counties></navigate-counties>
            
        </div>
    </div>
    <div class="col-md-12">
        <div class="x_panel">
            <div class="x_content">
                <div class="row">
                    <div class="col-lg-4 col-md-6 col-xs-12" >
                        <a [routerLink]="['/reporting/state/district/notcounties']" class="btn btn-dark btn-lg btn-block">Not County Units</a>
                    </div>
                    <div class="col-lg-4 col-md-6 col-xs-12" >
                        <a [routerLink]="['/reporting/county/ksu']" class="btn btn-dark btn-lg btn-block">KSU Employees</a>
                    </div>
                </div>
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
            <assignment-plans-of-work [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentPlansOfWorkOpen"></assignment-plans-of-work>  
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
            <assignment-affirmative-report  [fiscalYearId]="currentFiscalYear.name" *ngIf="assignmentAffirmativeReportOpen"></assignment-affirmative-report>
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
            <assignment-affirmative-plan [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentAffirmativePlanOpen"></assignment-affirmative-plan>  
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
            <assignment-program-indicators [fiscalYearId]="currentFiscalYear.name" *ngIf="assignmentProgramIndicatorsOpen"></assignment-program-indicators>  
        </div>




        </div>
        </div>
    </div>




    
  `
})
export class DistrictListComponent { 

    districts:Observable<District[]>;

    errorMessage:string;


    assignmentPlansOfWorkOpen = false;
    assignmentAffirmativeReportOpen = false;
    assignmentAffirmativePlanOpen = false;
    assignmentProgramIndicatorsOpen = false;

    lastFiscalYear:FiscalYear;
    currentFiscalYear:FiscalYear;

    constructor( 
        private fiscalYearService:FiscalyearService,
        private service:StateService
    )   
    {}

    ngOnInit(){
        this.fiscalYearService.last().subscribe(
            res => this.lastFiscalYear = res
        )
        this.fiscalYearService.current().subscribe(
            res => this.currentFiscalYear = res
        );
        this.districts = this.service.districts();
    }

}
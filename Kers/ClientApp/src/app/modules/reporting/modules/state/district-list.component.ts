import { Component } from '@angular/core';
import {StateService} from './state.service';

import {Observable} from 'rxjs/Observable';
import { District } from "../district/district.service";

@Component({
  template: `
    <div class="col-md-12">
        <div class="x_panel">
            <div class="x_title">
                <h2>Districts</h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div class="row">
                    <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let district of districts | async">
                        <a [routerLink]="['district', district.id]" class="btn btn-dark btn-lg btn-block">{{district.areaName}}<br><small>({{district.name}})</small></a>
                    </div>
                </div>
            </div>
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




    <div class="col-xs-12">
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
                    <p>Displays list of counties with no Plans of Work for the next fiscal year</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right" *ngIf="!assignmentPlansOfWorkOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=true">show</a>                
            </div>
            <div class="col-xs-12 text-right" *ngIf="assignmentPlansOfWorkOpen">
                <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=false">close</a>                
            </div>
            <assignment-plans-of-work *ngIf="assignmentPlansOfWorkOpen"></assignment-plans-of-work>  
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Affirmative Action Report</a>
                    <p>Displays list of counties with no Affirmative Action Report for the current fiscal year</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right">
                <a class="btn btn-info btn-xs">show</a>                
            </div>
            <assignment-affirmative-report></assignment-affirmative-report>  
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Affirmative Action Plan</a>
                    <p>Displays list of counties with no Affirmative Action Plan for the next fiscal year</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right">
                <a class="btn btn-info btn-xs">show</a>                
            </div>
            <assignment-affirmative-plan></assignment-affirmative-plan>  
        </div>
        <div class="row">
            <div class="ln_solid"></div>
            <div class="col-xs-10">
                <article class="media event ng-star-inserted">
                    <div class="media-body">
                    <a class="title">Program Indicators</a>
                    <p>Displays list of counties with no program indicators submitted for the currect fiscal year</p>
                    </div>
                </article>
            </div>
            <div class="col-xs-2 text-right">
                <a class="btn btn-info btn-xs">show</a>                
            </div>
            <assignment-program-indicators></assignment-program-indicators>  
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

    constructor( 
        private service:StateService
    )   
    {}

    ngOnInit(){
        this.districts = this.service.districts();
    }

}
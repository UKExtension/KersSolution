import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ActivityService, Activity, ActivityOption} from '../activity.service';

import { Router } from "@angular/router";
import { User } from "../../user/user.service";


@Component({
    selector: 'user-activities',
    template: `
        
        <div class="accordion">
            <activity-reports-year *ngFor="let year of years | async; let i = index" [year]="year" [index]="i" [user]="user"></activity-reports-year>
        
        </div><loading *ngIf="!(years | async)"></loading><br><br>
        <div class="text-right" *ngIf="!user"><a class="btn btn-default btn-xs" href="https://kers.ca.uky.edu/kers_mobile/ReportActivityMain.aspx">Meetings/Activities Reports Archive</a></div>
        `
})
export class ActivityReportsHomeComponent { 


    @Input() user:User;
    errorMessage: string;

    years;

    constructor( 
        private reportingService: ReportingService,
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        
        
        if(this.user != null){
            this.years = this.service.yearsWithActivities(this.user.id);
        }else{
            this.years = this.service.yearsWithActivities();
            this.defaultTitle();
        }
        
        
    }


    defaultTitle(){
        this.reportingService.setTitle("Meeting/Activity Records Report");
    }

















}
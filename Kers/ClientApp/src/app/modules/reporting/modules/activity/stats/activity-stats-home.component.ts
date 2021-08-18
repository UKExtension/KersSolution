import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ActivityService, Activity, ActivityOption} from '../activity.service';

import { Router } from "@angular/router";
import { User } from "../../user/user.service";


@Component({
  template: `
        <a [routerLink]="['/reporting/activity/stats/']" routerLinkActive="active" [routerLinkActiveOptions]="{exact:
true}">All Statistical Contact Records</a> | 
        <a [routerLink]="['/reporting/activity/stats/month']" routerLinkActive="active">Summary By Month</a> | 
        <a [routerLink]="['/reporting/activity/stats/program']" routerLinkActive="active">Summary By Major Program</a> | 
        <div>
        <router-outlet></router-outlet>
        </div>
        `,
    styles: [
        `
            .active{
                font-weight:bold;
            }
        `
    ]
})
export class ActivityStatsHomeComponent { 


    errorMessage: string;

    @Input() user:User;

    years;

    constructor( 
        private reportingService: ReportingService,
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
        
        
    }


    defaultTitle(){
        this.reportingService.setTitle("Statistical Contact Reports");
    }

















}
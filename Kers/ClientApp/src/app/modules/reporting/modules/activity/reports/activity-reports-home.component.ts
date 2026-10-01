import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ActivityService, Activity, ActivityOption} from '../activity.service';

import { Router } from "@angular/router";
import { User } from "../../user/user.service";


@Component({
    selector: 'user-activities',
    template: `

<div class="accordion">
  @for (year of years | async; track year; let i = $index) {
    <activity-reports-year [year]="year" [index]="i" [user]="user"></activity-reports-year>
  }

</div>@if (!(years | async)) {
<loading></loading>
}<br><br>
`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
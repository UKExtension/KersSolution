import { Component, Input } from '@angular/core';
import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { User } from "../../user/user.service";


@Component({
    selector: 'contact-activity-summary-month',
    templateUrl: 'activity-stats-month.component.html'
})
export class ActivityStatsMonthComponent { 

    @Input() user:User;


    errorMessage: string;

    activities:Observable<{}[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]> 

    constructor( 
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        if(this.user == null){
            this.activities = this.service.summaryPerMonth();
        }else{
            this.activities = this.service.summaryPerMonth(this.user.id);
        }

        
        this.races = this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        
        
    }


    
}
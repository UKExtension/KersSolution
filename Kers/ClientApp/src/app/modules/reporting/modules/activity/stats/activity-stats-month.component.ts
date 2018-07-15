import { Component, Input } from '@angular/core';
import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { User } from "../../user/user.service";
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';


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
        

        
        this.races = this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        
        
    }

    fiscalYearSwitched(event:FiscalYear){
        if(this.user == null){
            this.activities = this.service.summaryPerMonth(0,event.name);
        }else{
            this.activities = this.service.summaryPerMonth(this.user.id, event.name);
        }
    }


    
}
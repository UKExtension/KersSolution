import { Component, ViewEncapsulation } from '@angular/core';
import { ActivityService, Activity, ActivityOption, Race } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from 'rxjs';

import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';


@Component({
  templateUrl: 'activity-stats-all.component.html',
  styles: [`
    .mydrp .selectiongroup .selection{
        color:rgb(189, 189, 189);
    }


  `],
  encapsulation: ViewEncapsulation.None,
})
export class ActivityStatsAllComponent { 


    errorMessage: string;

    activities:Observable<Activity[]>;
    races:Observable<Race[]>;

    model = {beginDate: {year: 2018, month: 10, day: 9},
                             endDate: {year: 2018, month: 10, day: 19}};

    myDateRangePickerOptions: IAngularMyDpOptions = {
        // other options...
        dateFormat: 'mmm dd, yyyy',
        firstDayOfWeek: 'su'
    };

    constructor( 
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        var end = new Date();
        var start = new Date();
        start.setMonth(end.getMonth()-1)
        this.model.beginDate = {year: start.getFullYear(), month: start.getMonth(), day: start.getDate()};
        this.model.endDate = {year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()};

        this.activities = this.service.perPeriod(start, end);
        this.activities.subscribe(
            res=>{
                return res;
            },
            err => this.errorMessage = <any> err
        );
        this.races = this.service.races();
        
    }

    dateCnanged(event: IMyDateModel){
        this.activities = this.service.perPeriod(event.dateRange.beginJsDate, event.dateRange.endJsDate);
    }















}
import { Component, ViewEncapsulation } from '@angular/core';
import { ActivityService, ActivityOption, Race, ActivityOptionNumber } from '../../activity/activity.service';
import { ContactService, Contact} from '../contact.service';

import { Router } from "@angular/router";
import { IMyDrpOptions, IMyDateRangeModel } from "mydaterangepicker";
import { Observable } from "rxjs/Observable";


@Component({
  templateUrl: 'contact-stats-all.component.html',
  styles: [`
    .mydrp .selectiongroup .selection{
        color:rgb(189, 189, 189);
    }


  `],
  encapsulation: ViewEncapsulation.None,
})
export class ContactStatsAllComponent { 


    errorMessage: string;

    activities:Observable<Contact[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>

    private model = {beginDate: {year: 2018, month: 10, day: 9},
                             endDate: {year: 2018, month: 10, day: 19}};

    private myDateRangePickerOptions: IMyDrpOptions = {
        // other options...
        dateFormat: 'mmm dd, yyyy',
        showClearBtn: false,
        showApplyBtn: false,
        showClearDateRangeBtn: false
    };

    constructor( 
        private router: Router,
        private service:ContactService,
        private activityService: ActivityService
    )   
    {}

    ngOnInit(){
        var end = new Date();
        var start = new Date();
        start.setMonth(end.getMonth()-1)
        this.model.beginDate = {year: start.getFullYear(), month: start.getMonth(), day: start.getDate()};
        this.model.endDate = {year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()};

        this.activities = this.service.perPeriod(start, end);
        this.races = this.activityService.races();
        this.optionNumbers = this.activityService.optionnumbers();
    }

    dateCnanged(event: IMyDateRangeModel){
        this.activities = this.service.perPeriod(event.beginJsDate, event.endJsDate);
    }















}
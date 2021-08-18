import { Component, ViewEncapsulation } from '@angular/core';
import { ActivityService, ActivityOption, Race, ActivityOptionNumber } from '../../activity/activity.service';
import { ContactService, Contact} from '../contact.service';

import { Router } from "@angular/router";
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable } from 'rxjs';


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

    model: IMyDateModel = null;

    myDpOptions: IAngularMyDpOptions = {
        dateRange: true,
        dateFormat: 'mmm dd, yyyy',
        alignSelectorRight: true
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
        start.setMonth(end.getMonth()-1);
        this.model = {
            isRange: true, 
            singleDate: null, 
            dateRange: {
              beginDate: {
                year: start.getFullYear(), month: start.getMonth(), day: start.getDate()
              },
              endDate: {
                year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()
              }
            }
          };

        this.activities = this.service.perPeriod(start, end);
        this.races = this.activityService.races();
        this.optionNumbers = this.activityService.optionnumbers();
    }

    onDateChanged(event: IMyDateModel){
        this.activities = this.service.perPeriod(event.dateRange.beginJsDate, event.dateRange.endJsDate);
    }















}
import { Component } from '@angular/core';
import { ActivityService, Race, ActivityOptionNumber } from '../../activity/activity.service';
import {ContactService, Contact} from '../contact.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";


@Component({
  templateUrl: 'contact-stats-month.component.html'
})
export class ContactStatsMonthComponent { 


    errorMessage: string;

    activities:Observable<{}[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]> 

    constructor( 
        private router: Router,
        private service:ActivityService,
        private contactService:ContactService
    )   
    {}

    ngOnInit(){
        

        this.activities = this.contactService.summaryPerMonth();
        this.races = this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        
        
    }


    
}
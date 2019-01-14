import { Component, Input } from '@angular/core';
import { ActivityService, Race, ActivityOptionNumber, Activity } from '../../activity/activity.service';
import {ContactService, Contact} from '../contact.service';

import { Observable } from "rxjs/Observable";
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';
import { User } from '../../user/user.service';


@Component({
  templateUrl: 'contact-stats-month.component.html'
})
export class ContactStatsMonthComponent { 
    @Input() user:User;

    errorMessage: string;

    activities:Observable<{}[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]> 

    constructor(
        private service:ActivityService,
        private contactService:ContactService
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

    totalHours( actvts:Activity[]){
        var total = actvts.map( a => a["hours"]).reduce( (one, two) => one + two);
        return total;
    }
    totalMultistate( actvts){
        var total = actvts.map( a => a["multistate"]).reduce( (one, two) => one + two);
        return total;
    }
    totalContacts( actvts){
        var total = actvts.map( a => a["males"] + a["females"]).reduce( (one, two) => one + two);
        return total;
    }
    raceValue(race:Race, actvts){

        var filtered = actvts.map( a => a["raceEthnicityValues"] );
        filtered = [].concat.apply([], filtered);
        filtered = filtered.filter( a => a["raceId"]== race.id);
        var total = filtered.map( r => r["amount"]).reduce( (one, two) => one + two);
        return total;
    }

    ethnicityValue( ethnicityId, actvts){

        var filtered = actvts.map( a => a.raceEthnicityValues );
        filtered = [].concat.apply([], filtered);
        filtered = filtered.filter( a => a.ethnicityId== ethnicityId);
        var total = filtered.map( r => r.amount).reduce( (one, two) => one + two);
        return total;
    }
    totalMales( actvts){
        var total = actvts.map( a => a.males ).reduce( (one, two) => one + two);
        return total;
    }
    totalFemales( actvts){
        var total = actvts.map( a => a.females).reduce( (one, two) => one + two);
        return total;
    }

    optionNumberValue( optionNumber:ActivityOptionNumber, actvts){

        var filtered = actvts.map( a => a.optionNumberValues );
        filtered = [].concat.apply([], filtered);
        filtered = filtered.filter( a => a.activityOptionNumberId== optionNumber.id);
        var total = filtered.map( r => r.value).reduce( (one, two) => one + two);
        return total;
    }


    
}
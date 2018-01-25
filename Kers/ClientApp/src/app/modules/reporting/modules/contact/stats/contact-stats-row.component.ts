import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { ActivityService, Race, ActivityOptionNumber } from '../../activity/activity.service';
import { Observable } from "rxjs/Observable";
import {Contact} from '../contact.service';

@Component({
    selector: '[contactStatsRow]',
    template: `
        <td>{{activity.contactDate | date:'MMM, y'}}</td>
        <td>{{activity.majorProgram.name}}</td>
        <td>{{activity.days}}</td>
        <td>{{activity.multistate}}</td>
        <td>{{(activity.male + activity.female)}}</td>
        <td *ngFor="let race of races | async">{{raceValue(race)}}</td>
        <td>{{ethnicity(2)}}</td>
        <td>{{activity.female}}</td>
        <td>{{activity.male}}</td>
        <td *ngFor="let opt of optionNumbers | async">{{optionValue(opt)}}</td>
    `
})
export class ContactStatsRow implements OnInit {

    @Input ('contactStatsRow') activity: Contact;

    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        this.races= this.service.races();
        this.optionNumbers = this.service.optionnumbers();
    }



    raceValue(race:Race){
        var val = 0;
        var valse = this.activity.contactRaceEthnicityValues.filter(v => v.raceId == race.id);
        for(var v of valse){
            val += v.amount;
        }
        return val;
    }
    ethnicity(ethnct:number){
        var val = 0;
        var valse = this.activity.contactRaceEthnicityValues.filter(v => v.ethnicityId == ethnct);
        for(var v of valse){
            val += v.amount;
        }
        return val;
    }

    optionValue(op:ActivityOptionNumber){
        for (var r of this.activity.contactOptionNumbers){
            if(r.activityOptionNumberId == op.id){
                return r.value;
            }
        }
        return 0;
    }

}
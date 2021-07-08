import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { ActivityService, Activity, ActivityOption, Race } from '../activity.service';
import { Observable } from "rxjs";

@Component({
    selector: '[activityStatsRow]',
    template: `
        <td>{{activity.activityDate | date:'shortDate'}}</td>
        <td>{{activity.title}}</td>
        <td>{{activity.majorProgram.name}}</td>
        <td>{{activity.hours}}</td>
        <td>{{optionExists('Multistate effort?')}}</td>
        <td>{{activity.male + activity.female}}</td>
        <td *ngFor="let race of races | async">{{raceValue(race)}}</td>
        <td>{{ethnicity(2)}}</td>
        <td>{{activity.female}}</td>
        <td>{{activity.male}}</td>
        <td>{{optionValue('Number of Youth Participants (18 and under)')}}</td>
        <td>{{optionValue('Number of Adult Volunteers')}}</td>
        <td>{{optionValue('Number of Indirect Contacts')}}</td>
    `
})
export class ActivityStatsRow implements OnInit {

    @Input ('activityStatsRow') activity: Activity;

    races:Observable<Race[]>;

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        this.races= this.service.races();
    }

    optionExists(name){
        if(this.activity.activityOptionSelections.filter(a => a.activityOption.name == name).length > 0){
             return 'Yes';
        }
        return '';
    }

    raceValue(race:Race){
        var val = 0;
        var valse = this.activity.raceEthnicityValues.filter(v => v.raceId == race.id);
        for(var v of valse){
            val += v.amount;
        }
        return val;
    }
    ethnicity(ethnct:number){
        var val = 0;
        var valse = this.activity.raceEthnicityValues.filter(v => v.ethnicityId == ethnct);
        for(var v of valse){
            val += v.amount;
        }
        return val;
    }
    optionValue(nm:string){
        var val = 0;
        var valse = this.activity.activityOptionNumbers.filter(v => v.activityOptionNumber.name == nm);
        for(var v of valse){
            val += v.value;
        }
        return val;
    }
}
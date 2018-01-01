import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber } from '../../activity/activity.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: '[contactStatsMonthRow]',
    template: `
        <td nowrap>{{activity.year}}-{{activity.month}}</td>
        <td>{{activity.days}}</td>
        <td>{{activity.multistate}}</td>
        <td>{{activity.males + activity.females}}</td>
        <td *ngFor="let race of races | async">{{raceValue(race)}}</td>
        <td>{{ethnicity(2)}}</td>
        <td>{{activity.females}}</td>
        <td>{{activity.males}}</td>
        <td *ngFor="let opt of optionNumbers | async">{{optionValue(opt)}}</td>
        
    `
})
export class ContactStatsMonthRow implements OnInit {

    @Input ('contactStatsMonthRow') activity;

    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>
    errrorMessage:string;


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
        for (var r of this.activity.races){
            var valse = r.revs.filter(v => v.raceId == race.id);
            for(var v of valse){
                val += v.amount;
            }
        }
        return val;
    }
    ethnicity(ethnct:number){
        var val = 0;
        for (var r of this.activity.races){
            var valse = r.revs.filter(v => v.ethnicityId == ethnct);
            for(var v of valse){
                val += v.amount;
            }
        }
        return val;
    }
    optionValue(op:ActivityOptionNumber){
        var val = 0;
        for (var r of this.activity.optionNumbers){
            var valse = r.revs.filter(v => v.activityOptionNumberId == op.id);
            for(var v of valse){
                val += v.value;
            }
        } 
        return val;
    }
}
import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber } from '../activity.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: '[activityStatsProgramRow]',
    template: `
        <td>{{activity.majorProgram.name}}</td>
        <td>{{activity.hours}}</td>
        <td>{{activity.audience}}</td>
        <td *ngFor="let race of races | async">{{raceValue(race)}}</td>
        <td>{{ethnicity(2)}}</td>
        <td>{{females()}}</td>
        <td>{{males()}}</td>
        <td *ngFor="let opt of optionNumbers | async">{{optionValue(opt)}}</td>
        
    `
})
export class ActivityStatsProgramhRow implements OnInit {

    @Input ('activityStatsProgramRow') activity;

    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>
    errrorMessage:string;


    constructor( 
        private service:ActivityService
    )   
    {
        
    }

    ngOnInit(){
        this.races= this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        
    }

    ngAfterViewInit(){
        
    }


    raceValue(race:Race){
        var val = 0;
        for (var r of this.activity.revisions){
            var valse = r.raceEthnicityValues.filter(v => v.raceId == race.id);
            for(var v of valse){
                val += v.amount;
            }
        }
        return val;
    }
    ethnicity(ethnct:number){
        var val = 0;
        for (var r of this.activity.revisions){
            var valse = r.raceEthnicityValues.filter(v => v.ethnicityId == ethnct);
            for(var v of valse){
                val += v.amount;
            }
        }
        return val;
    }
    optionValue(op:ActivityOptionNumber){
        var val = 0;
        for (var r of this.activity.revisions){
            var valse = r.activityOptionNumbers.filter(v => v.activityOptionNumberId == op.id);
            for(var v of valse){
                val += v.value;
            }
        }
        return val;
    }

    males(){
        var val = 0;
        for (var r of this.activity.revisions){
            val += r.male;
        }
        return val;
    }
    females(){
        var val = 0;
        for (var r of this.activity.revisions){
            val += r.female;
        }
        return val;
    }
}
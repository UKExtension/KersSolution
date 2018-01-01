import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber } from '../activity.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: '[activityStatsMonthRow]',
    template: `
        <td nowrap>{{activity.year}}-{{activity.month}}</td>
        <td>{{activity.hours}}</td>
        <td>{{multistate}}</td>
        <td>{{activity.audience}}</td>
        <td *ngFor="let race of races | async">{{raceValue(race)}}</td>
        <td>{{ethnicity(2)}}</td>
        <td>{{females()}}</td>
        <td>{{males()}}</td>
        <td *ngFor="let opt of optionNumbers | async">{{optionValue(opt)}}</td>
        
    `
})
export class ActivityStatsMonthRow implements OnInit {

    @Input ('activityStatsMonthRow') activity;

    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>
    errrorMessage:string;
    multistate:number;

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        this.races= this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        this.service.options().share().subscribe(
            res => {
                var multistateId = res.filter(a => a.name == 'Multistate effort?');
                if(multistateId.length > 0){
                    var hours = 0;
                    var id = multistateId[0].id
                    for( var i = 0; i < this.activity.revisions.length; i++ ){
                        var opts = this.activity.revisions[i].activityOptionSelections.filter( o => o.activityOptionId == id);
                        if(opts.length > 0){
                            hours += this.activity.revisions[i].hours;
                        }
                    }
                    this.multistate = hours;
                }
                
            },
            err => this.errrorMessage = <any>err
        );
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
}
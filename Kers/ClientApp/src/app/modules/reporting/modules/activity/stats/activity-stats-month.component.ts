import { Component, Input } from '@angular/core';
import { ActivityService, Activity, ActivityOption, Race, ActivityOptionNumber, Ethnicity, PerMonthActivities, PerMonthContacts } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { User } from "../../user/user.service";
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';
//import { arch } from 'os';


@Component({
    selector: 'contact-activity-summary-month',
    templateUrl: 'activity-stats-month.component.html'
})
export class ActivityStatsMonthComponent { 

    @Input() user:User;


    errorMessage: string;

    activities:PerMonthContacts[] = [];
    races:Race[];
    optionNumbers:ActivityOptionNumber[];

    allActivities:Activity[] = [];


    numActivities:number = 0;
    amountPerBatch: number = 3;
    currentBarch:number = 0;
    loading:boolean = false;
    timePerBatch = 100;
    averageBatchTime = 0;

    yearSwitched:boolean = false;
    fiscalYear:FiscalYear;


    constructor( 
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        
         this.service.races().subscribe(
            res => this.races = res
        );
        this.service.optionnumbers().subscribe(
            res => this.optionNumbers =  res
        );
    }



    getNumRows(){
        this.resetRequest();
        this.service.numberActivitiesPerYear(this.fiscalYear.id, (this.user == null ? 0 : this.user.id)).subscribe(
            res => {
                this.numActivities = res;
                this.getBatch(this.fiscalYear.id);
            }
        )
    }
    resetRequest(){
        this.numActivities = 0;
        this.currentBarch = 0;
        this.allActivities = [];
        this.activities = undefined;
        this.activities = [];
    }

    getBatch(fiscalYaerId:number){
        this.loading = true;
        var startTime = new Date();
        this.service.GetActivitiesBatch(this.currentBarch, this.amountPerBatch,fiscalYaerId, this.user == null ? 0 : this.user.id ).subscribe(
            res => {
                var endTime = new Date();
                var seconds = (endTime.getTime() - startTime.getTime()) / 1000;
                this.averageBatchTime = (this.averageBatchTime == 0 ? seconds : (this.averageBatchTime + seconds) /2)
                this.timePerBatch = endTime.getTime() - startTime.getTime();
                this.processBatch(res);
                if(this.yearSwitched){
                    this.yearSwitched = false;
                    this.getNumRows();
                }else if( this.currentBarch <= this.numActivities ){
                    this.currentBarch += this.amountPerBatch;
                    this.getBatch( fiscalYaerId );
                }else{
                    this.activities.sort(
                        function( a, b ){
                            return (a.year - b.year) * 100 + ( a.month - b.month)
                        }
                    );
                    this.loading = false;
                }
            }
        )
    }

    processBatch( batch:Activity[]){
        for( let a of batch ) this.addTheActivity( a );
    }

    addTheActivity( activity:Activity ){
        var dt = new Date(activity.activityDate);
        var year = dt.getFullYear();
        var month = dt.getMonth();
        // find if row exists
        var filteredRow = this.activities.filter( a => a.month == month && a.year == year);
        if(filteredRow.length > 0){
            var row = filteredRow[0];
            row.hours += activity.hours;
            if( activity.activityOptionSelections.filter( n => n.activityOption.name == "Multistate effort?").length > 0 ){
                row.multistate += activity.hours;
            }
            row.males += activity.male;
            row.females += activity.female;
            row.raceEthnicityValues = row.raceEthnicityValues.concat(activity.raceEthnicityValues);
            row.optionNumberValues = row.optionNumberValues.concat(activity.activityOptionNumbers);
        }else{
            var row = new PerMonthContacts;
            row.month = month;
            row.year = year;
            row.hours = activity.hours;
            row.males = activity.male;
            row.females = activity.female;
            if( activity.activityOptionSelections.filter( n => n.activityOption.name == "Multistate effort?").length > 0 ){
                row.multistate = activity.hours;
            }else{
                row.multistate = 0;
            }
            row.raceEthnicityValues = activity.raceEthnicityValues;
            row.optionNumberValues = activity.activityOptionNumbers;
            this.activities.push(row);
        }

    }



/* 
    processActivities(){
        for( let act of this.allActivities){
            this.addTheActivity(act);
        }
    }


    addTheActivity(activity:Activity){
        var dt = new Date(activity.activityDate);
        var year = dt.getFullYear();
        var month = dt.getMonth();
        // find if row exists
        var filteredRow = this.activities.filter( a => a.month == month && a.year == year);
        if(filteredRow.length > 0){
            filteredRow[0].revisions.push(activity);
            filteredRow[0].audience += (activity.male + activity.female);
            filteredRow[0].hours += activity.hours;
        }else{
            var actvt = new PerMonthActivities;
            actvt.month = month;
            actvt.year = year;
            actvt.audience = activity.male + activity.female;
            actvt.hours = activity.hours;
            actvt.revisions = [];
            actvt.revisions.push(activity);
            this.activities.push(actvt);
        }

    }
 */

    fiscalYearSwitched(event:FiscalYear){
        this.fiscalYear = event;
        if(this.loading){
            this.yearSwitched = true;
        }else{
            this.getNumRows();
        }
        
        //this.getNumRows(event);
    }

    totalHours( actvts:Activity[]){
        var total = actvts.length > 0 ? actvts.map( a => a.hours).reduce( (one, two) => one + two) : 0;
        return total;
    }
    totalMultistate( actvts){
        var total = actvts.length > 0 ? actvts.map( a => a.multistate).reduce( (one, two) => one + two) : 0;
        return total;
    }
    totalContacts( actvts){
        var total = actvts.length > 0 ? actvts.map( a => a.males + a.females).reduce( (one, two) => one + two) : 0;
        return total;
    }
    raceValue(race:Race, actvts){

        var filtered = actvts.map( a => a.raceEthnicityValues );
        filtered = [].concat.apply([], filtered);
        filtered = filtered.filter( a => a.raceId== race.id);
        var total = (filtered.length > 0 ? filtered.map( r => r.amount).reduce( (one, two) => one + two) : 0);
        return total;
    }

    ethnicityValue( ethnicityId, actvts){
        
        var filtered = actvts.map( a => a.raceEthnicityValues );
        filtered = [].concat.apply([], filtered);
        filtered = filtered.filter( a => a.ethnicityId== ethnicityId);
        var total = (filtered.length > 0 ? filtered.map( r => r.amount).reduce( (one, two) => one + two) : 0)   ;
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
import { Component, OnInit } from '@angular/core';
import {    ActivityService, Activity, 
            ActivityOption, ActivityOptionNumber, 
            ActivityOptionNumberValue, ActivityOptionSelection,
            Race, Ethnicity, RaceEthnicityValue,
            ActivityMonth
        } from './activity.service';
import { Router } from '@angular/router';

@Component({
  templateUrl: 'activity-home.component.html'
})
export class ActivityHomeComponent implements OnInit { 
    
    newActivity = false;
    latest = [];
    numbActivities = 0;
    byMonth:ActivityMonth[] = [];

    errorMessage:string;

    constructor( 
        private service:ActivityService,
        private router:Router
    )   
    {}

    ngOnInit(){
        this.router.navigate(['/reporting/servicelog']);
        this.service.latest().subscribe(
            res=>{
                    this.latest = <Activity[]>res; 
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
        this.service.num().subscribe(
            res => {
                this.numbActivities = <number>res;
            },
            err => this.errorMessage = <any>err
        );
       
    }


    populateByMonth(){
        var bm = this.byMonth;
        this.latest.forEach(function(element){
            
                var exDt = new Date(element.activityDate);
                var exMonth = bm.filter(f=>f.month==exDt.getMonth() && f.year == exDt.getFullYear());
                if(exMonth.length == 0){
                    var ob = <ActivityMonth>{
                        month : exDt.getMonth(),
                        year : exDt.getFullYear(),
                        date: exDt,
                        activities : [element]
                    };
                    bm.push(ob);
                }else{
                    exMonth[0].activities.push(element);
                }
            }); 
    }

    newActivitySubmitted(activity:Activity){
        this.latest.unshift(activity);
        this.byMonth = [];
        this.populateByMonth();
        this.newActivity = false;
        this.numbActivities++;
    }

    loadMore(){
        var lt = this.latest;
        this.service.latest(this.latest.length, 5).subscribe(
            res=>{
                    var batch = <Activity[]>res; 
                    batch.forEach(function(element){
                        lt.push(element);
                    });
                    this.byMonth = [];
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
    }

    deleted(activity:Activity){
        let index: number = this.latest.indexOf(activity);
        if (index !== -1) {
            this.latest.splice(index, 1);
            this.numbActivities--;
        }
        this.byMonth = [];
        this.populateByMonth();
    }
    edited(activity:Activity){

        this.latest = this.latest.map(function(item) { return item.activityId == activity.activityId ? activity : item; });
        this.latest.sort(
                    function(a, b) {

                        var dateA = new Date(a.activityDate);
                        var dateB = new Date(b.activityDate);
                        if( dateA  > dateB ){
                            return -1;
                        }else{
                            return 1;
                        }
                    }
                 );
        this.byMonth = [];
        this.populateByMonth();
    }


}
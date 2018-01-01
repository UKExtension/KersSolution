import { Component } from '@angular/core';
import { ActivityService, ActivityOptionNumber, Race } from '../../activity/activity.service';
import {Contact, ContactService} from '../contact.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";


@Component({
  templateUrl: 'contact-stats-program.component.html'
})
export class ContactStatsProgramComponent { 


    errorMessage: string;
    activities:Observable<{}[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]>;


    option;




    constructor( 
        private router: Router,
        private service:ActivityService,
        private contactService:ContactService
    )   
    {}

    ngOnInit(){
        
        this.activities = this.contactService.summaryPerProgram().share();
        this.races = this.service.races();
        this.optionNumbers = this.service.optionnumbers();
        this.activities.subscribe(
            res=>{
                this.createChart(res);
            },
            err => this.errorMessage = <any> err
        );
        
        
    }

    createChart(activities){
        
        this.option =  {
            tooltip : {
                trigger: 'item',
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                x : 'center',
                y : 'top',
                data:this.legendData(activities)
            },
            toolbox: {
                show : true,
                feature : {
                    mark : {show: true},
                    dataView : {
                        show: true, 
                        readOnly: false, 
                        title: 'Data',
                        lang: ['Data', 'Close', 'Refresh']
                },
                    saveAsImage : {show: true, title: 'Image'}
                }
            },
            calculable : true,
            series : [  this.daysSeries(activities),
                        this.audienceSeries(activities)
            ]
        };


    }


    legendData(activities){
        var pieData = [];

        for( var dt of activities){
            pieData.push(dt.program[0].progr.name);
        }

        return pieData;
    }


    daysSeries(activities){


        var pieData = [];

        for( var dt of activities){
            var d = {value: dt.days, name: dt.program[0].progr.name};
            pieData.push(d);
        }


        var serie = {
                    name:'Days per Major Program',
                    type:'pie',
                    radius : [30, 110],
                    center : ['25%', '50%'],
                    roseType : 'radius',
                    data: pieData
                }


        return serie;


    }

    audienceSeries(activities){

        var pieData = [];

        for( var dt of activities){
            var d = {value: (dt.females + dt.males), name: dt.program[0].progr.name};
            pieData.push(d);
        }




        var serie = {
                    name:'Audience by Major Program',
                    type:'pie',
                    radius : [30, 110],
                    center : ['75%', '50%'],
                    roseType : 'radius',
                    data: pieData
                };



        return serie;
    }




}
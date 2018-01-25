import { Component, Input } from '@angular/core';
import { ActivityService, Activity, ActivityOption, ActivityOptionNumber, Race } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from "rxjs/Observable";
import { User } from "../../user/user.service";


@Component({
    selector: 'contact-activity-summary-program',
    templateUrl: 'activity-stats-program.component.html'
})
export class ActivityStatsProgramComponent { 

    @Input() user:User;
    errorMessage: string;
    activities:Observable<{}[]>;
    races:Observable<Race[]>;
    optionNumbers:Observable<ActivityOptionNumber[]> 

    rowsRendered = 0;

    option;

    constructor( 
        private router: Router,
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        if(this.user == null){
            this.activities = this.service.summaryPerProgram().share();
        }else{
            this.activities = this.service.summaryPerProgram(this.user.id).share();
        }
        
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
            title: [{
                text: 'Hours',
                subtext: 'by Major Program',
                x: '25%',
                y: '20%',

                textAlign: 'center'
            }, {
                text: 'Audience',
                subtext: 'by Major Program',
                x: '75%',
                y: '20%',
                textAlign: 'center'
            }],
            series : [  this.daysSeries(activities),
                        this.audienceSeries(activities)
            ]
        };


    }


    legendData(activities){
        var pieData = [];

        for( var dt of activities){
            pieData.push(dt.majorProgram.name);
        }

        return pieData;
    }


    daysSeries(activities){


        var pieData = [];

        for( var dt of activities){
            var d = {value: dt.hours, name: dt.majorProgram.name};
            pieData.push(d);
        }


        var serie = {
                    name:'Hours per Major Program',
                    tooltip : {
                        trigger: 'item',
                        formatter: "{b}<br/>{c} hours ({d}%)"
                    },
                    label: {
                        normal: {
                            show: false
                        }
                    },
                    type:'pie',
                    radius : [20, 90],
                    center : ['25%', '70%'],
                    roseType : 'radius',
                    data: pieData
                }


        return serie;


    }

    audienceSeries(activities){

        var pieData = [];

        for( var dt of activities){
            var d = {value: (dt.audience), name: dt.majorProgram.name};
            pieData.push(d);
        }




        var serie = {
                    name:'Audience by Major Program',
                    type:'pie',
                    label: {
                        normal: {
                            show: false
                        }
                    },
                    tooltip : {
                        trigger: 'item',
                        formatter: "{b}<br/>Audience {c} ({d}%)"
                    },
                    radius : [20, 90],
                    center : ['75%', '70%'],
                    roseType : 'radius',
                    data: pieData
                };



        return serie;
    }




}
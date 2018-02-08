import { Component, Input } from '@angular/core';
import {ActivityService, Activity, Race} from '../activity.service';
import { User } from "../../user/user.service";



@Component({
    selector: 'activity-reports-summary',
    template: `
  


<loading *ngIf="loading"></loading>
    <div class="fa-hover col-md-3 col-sm-4 col-xs-12"><a (click)="showChart = !showChart"><i class="fa fa-bar-chart" style="cursor:pointer;"></i></a>
    </div>
    <div *ngIf="showChart">
        <div class="ln_solid"></div>
        <h3>Meeting/Activity Hours and Contacts by Date</h3>
        <div [ts-chart]="option"></div>
        <div class="ln_solid"></div>
    </div>
     <div class="col-md-12 col-sm-12 col-xs-12" *ngIf="!loading">

            <div class="table-responsive">
                <table class="table table-striped" style="background-color: white">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Title</th>
                            <th>Description</th>
                            <th>Major Program</th>
                            <th>Hours</th>
                            <th>Contacts</th>
                            <th>Details</th>
                            <th>Snap-Ed</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr *ngFor="let activity of monthActivities">
                            <td>{{activity.activityDate| date:'shortDate'}}</td>
                            <td>{{activity.title}}</td>
                            <td innerHtml="{{activity.description}}"></td>
                            <td>{{activity.majorProgram.name}}</td>
                            <td>{{activity.hours}}</td>
                            <td>{{attendance(activity)}}</td>
                            <td><div *ngFor="let opt of activity.activityOptionSelections">{{opt.activityOption.name.substring(0, opt.activityOption.name.length -1 )}}</div></td>
                            <td><div *ngIf="activity.snapAdmin && activity.isSnap">Admin</div><div *ngIf="activity.snapPolicyId != null">Community/PSE</div> <div *ngIf="activity.snapDirectId != null">Direct</div> <div *ngIf="activity.snapIndirectId != null">Indirect</div></td>
                        </tr>
                        
                    </tbody>
                </table>
            </div>
            <br><br>

                           
                         </div>




        `
})
export class ActivityReportsSummaryComponent { 

    showChart = false;
    loading = false;
    pdfLoading = false;

    errorMessage: string;
    @Input() month;
    @Input() year;
    @Input() user:User;
    monthActivities: Activity[];

    races:Race[];

    option;

    mileageRate;

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        this.loading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.activitiesPerMonth(this.month.month, this.year.year, userid).subscribe(
            res=> {
                this.monthActivities = <Activity[]>res;
                this.loading = false;
                this.createChart();
            },
            err => this.errorMessage = <any>err
        );
        
        this.service.races().subscribe(
            res=> this.races = <Race[]> res,
            err => this.errorMessage = <any>err
        );
        
    }

    attendance(activity: Activity){
        var sum = 0;
        for(var r of activity.raceEthnicityValues){
            sum += r.amount;
        }
        return sum;
    }





    createChart(){

        var data = [];
        var series = [];
        var legend = [];

        var colors = [
            "#6E9B74", "#31b89a", "#35495d", "#bdc3c7", "#3a99d8", "#53697A", "#8bba72", "#193809", 
        ];
 
        

        var serieHours = {
                            "name": "Hours",
                            "type": "line",
                            "stack": "Hours",
                            symbolSize:10,
                            symbol:'circle',
                            yAxisIndex: 1,
                            "itemStyle": {
                                "normal": {
                                    "color": "#ccc",
                                    "barBorderRadius": 0,
                                    "label": {
                                        "show": true,
                                        "position": "top",
                                        formatter: function(p) {
                                            return p.value > 0 ? (p.value) : '';
                                        }
                                    }
                                }
                            },
                            "data": []
                    };


        var colorIndex = 0;
        for(let race of this.races){
                legend.push(race.name);
                series.push(this.serie(race, colors[colorIndex]));
                colorIndex++;
        }


        for(let activity of this.monthActivities){
            var options = { month: 'numeric', day: 'numeric' };
            let date = new Date(activity.activityDate);
            data.push(date.toLocaleDateString("en-US", options));
            
            this.serieData(activity, series);

            serieHours.data.push(activity.hours);

        }


        series.push(serieHours);
        legend.push("Hours")

         this.option = {
                    backgroundColor: "#fff",
                    "title": {
                        x: "4%",

                        textStyle: {
                            color: '#fff',
                            fontSize: '22'
                        },
                        subtextStyle: {
                            color: '#90979c',
                            fontSize: '16',

                        },
                    },
                    "tooltip": {
                        "trigger": "axis",
                        "axisPointer": {
                            "type": "shadow",
                            textStyle: {
                                color: "#fff"
                            }

                        },
                    },
                    "grid": {
                        "borderWidth": 0,
                        "top": 110,
                        "bottom": 95,
                        textStyle: {
                            color: "#fff"
                        }
                    },
                    "legend": {
                        x: '4%',
                        top: '11%',
                        textStyle: {
                            color: '#90979c',
                        },
                        "data": legend
                    },
                    

                    "calculable": true,
                    "xAxis": [{
                        "type": "category",
                        "axisLine": {
                            lineStyle: {
                                color: '#90979c'
                            }
                        },
                        "splitLine": {
                            "show": false
                        },
                        "axisTick": {
                            "show": false
                        },
                        "splitArea": {
                            "show": false
                        },
                        "axisLabel": {
                            "interval": 0,

                        },
                        "data": data,
                    }],
                    "yAxis": [{
                        "type": "value",
                        "name": "Contacts",
                        "splitLine": {
                            "show": false
                        },
                        "axisLine": {
                            lineStyle: {
                                color: '#90979c'
                            }
                        },
                        "axisTick": {
                            "show": false
                        },
                        "axisLabel": {
                            "interval": 0,

                        },
                        "splitArea": {
                            "show": false
                        },

                    }, 
                    {
                        "type": "value",
                        "name": "Hours",
                        "splitLine": {
                            "show": false
                        },
                        "axisLine": {
                            lineStyle: {
                                color: '#90979c'
                            }
                        },
                        "axisTick": {
                            "show": false
                        },
                        "axisLabel": {
                            "interval": 0,

                        },
                        "splitArea": {
                            "show": false
                        },

                    }],
                    "dataZoom": [{
                        "show": true,
                        "height": 30,
                        "xAxisIndex": [
                            0
                        ],
                        bottom: 30,
                        "start": 0,
                        "end": 60,
                        handleIcon: 'path://M306.1,413c0,2.2-1.8,4-4,4h-59.8c-2.2,0-4-1.8-4-4V200.8c0-2.2,1.8-4,4-4h59.8c2.2,0,4,1.8,4,4V413z',
                        handleSize: '110%',
                        handleStyle:{
                            color:"#ddd",
                            
                        },
                        textStyle:{
                            color:"#bbb"},
                        borderColor:"#eee"
                        
                        
                    }, {
                        "type": "inside",
                        "show": true,
                        "height": 15,
                        "start": 1,
                        "end": 35
                    }],
                    "series": series
                }











    }


    serie(race:Race, color:string):Object{
        var serie  = {
                "name": race.name,
                "type": "bar",
                "stack": "Race",
                "itemStyle": {
                    "normal": {
                        "color": color,
                        "barBorderRadius": 0,
                        "label": {
                            "show": false,
                            "position": "top",
                            formatter: function(p) {
                                return p.value > 0 ? (p.value) : '';
                            }
                        }
                    }
                },
                "data": []
            };
        return serie;
    }
    serieData(activity, series){
        var index = 0;
        for(let race of this.races){
            var raceData = activity.raceEthnicityValues.filter(a => a.raceId == race.id);
            var raceAmounts = 0;
            for( let raceVal of raceData){
                raceAmounts += raceVal.amount;
            }
            series[index].data.push(raceAmounts);
            index++;
        }
    }




 













}


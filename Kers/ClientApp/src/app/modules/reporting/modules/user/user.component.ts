import { Component, Input } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import { UserService, User } from './user.service';
import { ActivityService, Activity } from '../activity/activity.service';
import { StoryService, Story } from '../story/story.service';
import { ActivatedRoute, Router, Params } from "@angular/router";
import {Location} from '@angular/common';
import { Observable } from "rxjs/Observable";

var echarts = require('echarts');


@Component({
    selector: 'user-profile-details',
    templateUrl: 'user.component.html',
    styles: [`
        ul.messages li img.avtr, img.avtr {
                height: 92px;
                width: 92px;
                float: left;
                display: inline-block;
                -webkit-border-radius: 2px;
                -moz-border-radius: 2px;
                border-radius: 2px;
                padding: 2px;
                background: #f7f7f7;
                border: 1px solid #e6e6e6;
            }
            ul.messages li .message_wrapper {
                margin-left: 105px;
                margin-right: 45px;
            }
    `]

})
export class UserComponent { 

    @Input('id') id: number;

    public user: User;
    profilePicSrc:string;

    latestStories:Observable<Story[]>;
    latestActivities:Observable<Activity[]>;
    errorMessage:string;
    timeWithExtension:string = "";

    public data = [];

    public option = null;
    public option1;
    cellSize = [25, 25];
    serieActivity;
    serieStory;

    storiesOpen = true;
    aboutOpen = false;
    activityOpen = false;

    constructor( 
        private reportingService: ReportingService,
        private route: ActivatedRoute,
        private router: Router,
        private service: UserService,
        private activityService: ActivityService,
        private storyService: StoryService,
        private location:Location
    )   
    {
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
        //this.data = this.getVirtulData(2016);
        //console.log(this.data);
        
    }

    ngOnInit(){
        

        this.route.params
            .switchMap((params: Params) => this.service.byId(params['id']))
            .subscribe((user: User) => 
                {
                    this.user = user;
                    this.latestStories = this.storyService.latestByUser(this.user.id, 5);
                    this.latestActivities = this.activityService.latestByUser(this.user.id, 25);
                    this.reportingService.setTitle(this.user.personalProfile.firstName + " " +this.user.personalProfile.lastName);
                    var n = new Date();
                    n.setMonth(n.getMonth() - 3);
                    this.activityService.perDay(this.user.id, n, new Date()).subscribe(
                        res => {
                            
                            for(var actvt of res){
                                var acg = [actvt.day, actvt.count];
                                this.data.push(acg);
                            }

                            this.storyService.perDay(this.user.id, n, new Date()).subscribe(
                                res => {

                                    this.initOptions();


                                    this.serieActivity = {
                                            name: 'Meeting/Activity',
                                            type: 'scatter',
                                            coordinateSystem: 'calendar',
                                            symbolSize: function (val) {
                                                    return val[1]*10;
                                                },
                                            data: this.data,
                                            itemStyle: {
                                                normal: {
                                                    color: 'rgb(91, 192, 222)'
                                                }
                                            }
                                        }
                                    this.option.series.push(this.serieActivity);
                                    var storyData = [];

                                    for(var actvt of res){
                                        var acg = [actvt.day, actvt.count];
                                        storyData.push(acg);
                                    }
                                    var serie = {
                                            name: 'Success Story',
                                            type: 'scatter',
                                            coordinateSystem: 'calendar',
                                            symbolSize: function (val) {
                                                    return val[1]*10;
                                                },
                                            data: storyData,
                                            itemStyle: {
                                                normal: {
                                                    color: 'rgb(26, 187, 156)'
                                                }
                                            }
                                        }
                                    this.option.series.push(serie);
                                }
                            );
                            /*
                            this.service.startDate(user.rprtngProfile.linkBlueId).subscribe(
                                res => {
                                    var dt = new Date(res);
                                    this.timeWithExtension = this.calcDate(new Date(), dt);
                                },
                                err => this.errorMessage = <any>err
                            )
                            */


                        }
                    )
                    
                    if(this.user.personalProfile.uploadImage){
                        this.profilePicSrc = this.location.prepareExternalUrl('/image/350/' + this.user.personalProfile.uploadImage.uploadFile.name);
                    }           
                });


    }

    openStories(){
        this.storiesOpen = true;
        this.aboutOpen = false;
        this.activityOpen = false;
    }
    openAbout(){
        this.storiesOpen = false;
        this.aboutOpen = true;
        this.activityOpen = false;
    }
    openActivity(){
        this.storiesOpen = false;
        this.aboutOpen = false;
        this.activityOpen = true;
    }


    initOptions(){

        var today = new Date();
        // Get the last day of the month minus 2 days. There is some error if the actual end of the month is set.
        // This should be investigated and fixed.
        var lastDayOfTheMonth = new Date(today.getFullYear(), today.getMonth() + 1, -1);
        var endString = lastDayOfTheMonth.toISOString().split('T')[0];
        var threeMonthsAgo = new Date(today.getFullYear(), today.getMonth() - 3, 1);
        var startString = threeMonthsAgo.toISOString().split('T')[0];
        this.option = {
                tooltip : {
                    formatter: '{a0}: {c}',
                },
                title: {
                    top: 30,
                    text: '',
                    subtext: '',
                    left: 'center',
                    textStyle: {
                        color: '#ccc'
                    }
                },
                calendar: {
                    range: [startString, endString],
                    //range: ['2017-09-01', '2017-11-10'],
                    top: 'middle',
                    left: '90px',
                    orient: 'horizontal',
                    cellSize: this.cellSize,
                    yearLabel: {
                        show: false,
                        textStyle: {
                            fontSize: 30
                        }
                    },
                    splitLine: {
                        show: true,
                        lineStyle: {
                            color: 'rgb(115, 135, 156)',
                            width: 1,
                            type: 'solid'
                        }
                    },
                    dayLabel: {
                        margin: 20,
                        firstDay: 1,
                        nameMap: ['Sun', 'Mon', 'Tue', 'Wed', 'Thr', 'Fri', 'Sat'],
                        textStyle: {
                            color: 'rgb(115, 135, 156)'
                        }
                    },
                    monthLabel: {
                        show: true,
                        textStyle: {
                            color: 'rgb(115, 135, 156)'
                        }
                    },
                    
                },
                series: []
            };
    }


    htmlToPlaintext(text) {
        var result = text ? String(text).replace(/<[^>]+>/gm, '') : ''
        return result.substring(0, 200);
    }

    day(dateString){
        return new Date(dateString).getDate();
    }


    month(dateString){


        return new Date(dateString).toLocaleString("en-us", { month: "long" });
    }


    externalUrl(url){
        return this.location.prepareExternalUrl(url);
    }

    calcDate(date1,date2) {
        var diff = Math.floor(date1.getTime() - date2.getTime());
        var day = 1000 * 60 * 60 * 24;

        var days = Math.floor(diff/day);
        var months = Math.floor(days/31);
        var years = Math.floor(months/12);

        var message = "";

        var mnt = months % 12;
        if( years >= 0 && years < 40){
            message += "<h4>With KY Extension for</h4>";
            message +=  '<ul class="list-unstyled user_data">';
            message +=  '<li><p>';
            message += years + " years ";
            message += mnt + " months ";
            message +=  '</p></li>';
            message +=  ' </ul>';
        }

        return message
    }



}
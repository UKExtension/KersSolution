import { Component, Input } from '@angular/core';
import {Location} from '@angular/common';
import { ActivatedRoute, Router, Params } from "@angular/router";

import { Observable } from "rxjs/Observable";
import { User, UserService } from "../user.service";
import { Activity, ActivityService } from "../../activity/activity.service";
import { Story, StoryService } from "../../story/story.service";
import { ReportingService } from "../../../components/reporting/reporting.service";
import { ExpenseService, ExpenseSummary } from "../../expense/expense.service";
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';


@Component({
    selector: 'user-summary',
    templateUrl: 'user-summary.component.html',
    styles: [`
        .view-row{
            border-top: 1px solid rgb(221, 221, 221);
            background-color: rgb(249, 249, 249);
            padding: 8px 4px;
        }
        .close-row{
            border-top: 1px solid rgb(221, 221, 221);
            padding: 8px 4px;
        }
        
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
export class UserSummaryComponent { 

    @Input('id') id: number;

    public user: User;
    profilePicSrc:string;

    latestStories:Observable<Story[]>;
    latestActivities:Observable<Activity[]>;
    expenseSummaries: Observable<ExpenseSummary>[];
    inServiceEnrolment;
    expenseFiscalYear:FiscalYear = null;
    hoursAttended = 0;


    errorMessage:string;
    

    constructor( 
        private reportingService: ReportingService,
        private location:Location,
        private route: ActivatedRoute,
        private router: Router,
        private service: UserService,
        private activityService: ActivityService,
        private expenseService: ExpenseService,
        private storyService: StoryService,
    )   
    {
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
        
    }

    ngOnInit(){
        

        this.route.params
            .switchMap((params: Params) => this.service.byId(params['id']))
            .subscribe((user: User) => 
                {
                    this.user = user;
                    
                    
                    
                    
                    
                    
                    this.reportingService.setSubtitle(this.user.personalProfile.firstName + " " +this.user.personalProfile.lastName);
                    this.reportingService.setTitle("Employee Summary");
                    if(this.user.personalProfile && this.user.personalProfile.uploadImage){
                        this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/250/250/' + this.user.personalProfile.uploadImage.uploadFile.name);
                    }

                    


            }
        )
                    
                   


    }



    htmlToPlaintext(text) {
        var result = text ? String(text).replace(/<[^>]+>/gm, '') : ''
        return result.substring(0, 200);
    }

    day(dateString){
        return new Date(dateString).getDay();
    }


    month(dateString){


        return new Date(dateString).toLocaleString("en-us", { month: "long" });
    }

    externalUrl(url){
        return this.location.prepareExternalUrl(url);
    }

    storiesFySwitched(event:FiscalYear){
        this.latestStories = this.storyService.latestByUser(this.user.id, 85, event.name);
    }

    expenseTotalsFySwitched(event:FiscalYear){
        this.expenseFiscalYear = event;
        this.expenseSummaries = this.expenseService.fiscalYearSummaries(this.user.id,event.name);
    }

    inServiceFiscalYearSwitched(event:FiscalYear){
        this.service.InServiceEnrolment(this.user.id, event.name).subscribe(
            res => {
                this.inServiceEnrolment = res;
                this.hoursAttended = 0;
                for( let el of this.inServiceEnrolment){
                    if(el[3] == 'Yes') this.hoursAttended += +el[2];
                }
            },
            err => this.errorMessage = <any>err
        )
    }

    ngOnDestroy(){
        this.reportingService.setTitle( 'Kentucky Extension Reporting System' );
        this.reportingService.setSubtitle('');
    }

    

}
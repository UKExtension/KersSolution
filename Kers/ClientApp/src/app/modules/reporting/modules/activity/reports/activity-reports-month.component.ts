import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ActivityService, Activity} from '../activity.service';

import { Router } from "@angular/router";
import { UserService, User, PersonalProfile } from '../../user/user.service';


@Component({
    selector: 'activity-reports-month',
    template: `
    
    <div class="expense-row" [class.row-even]="isIndexEven()" *ngIf="user">
        <div class="row">
            
            <div class="col-xs-8" *ngIf="rowDefault">
                
                <article class="media event">
                    <div class="media-body">
                        <a class="title" style="font-size:1.1em;">{{date | date:'MMMM'}} <small style="font-weight:normal;">({{year.year}})</small></a>
                    </div>
                </article>

            </div>
            <div class="col-xs-11" *ngIf="rowSummary">
                <h3>{{date | date:'MMMM, y'}} <small>{{user.personalProfile.firstName}} {{user.personalProfile.lastName}}</small></h3>Monthly Meetings/Activities Summary<br><br>
                <activity-reports-summary [month]="month" [year]="year" [user]="user"></activity-reports-summary>

            </div>
            <div class="col-xs-11" *ngIf="rowDetails" style="padding-bottom: 40px;">
                    
                    
                    <h3>{{date | date:'MMMM, y'}} <small>{{user.personalProfile.firstName}} {{user.personalProfile.lastName}}</small></h3>Detailed Meetings/Activities Records<br><br>


                    <activity-reports-details [month]="month" [year]="year" [user]="user"></activity-reports-details>
            </div>
                

            

            <div class="col-xs-4 text-right" *ngIf="rowDefault">
                <a class="btn btn-info btn-xs" (click)="summary()" ><i class="fa fa-cog"></i> Summary</a>
                <a class="btn btn-info btn-xs" (click)="details()" *ngIf="rowDefault"><i class="fa fa-cogs"></i> Details</a>
            </div>
            <div class="col-xs-1 text-right" *ngIf="!rowDefault">
                <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
            </div>  
        </div>
    </div>
        `,
        styles: [`
            .row-even{
                background-color: #f9f9f9;
            }
            .expense-row{
                padding: 10px 7px;
                border-top: 1px solid #ddd;
            }
        `]
})
export class ActivityReportsMonthComponent { 

    errorMessage: string;
    @Input() month;
    @Input() index;
    @Input() year;
    
    date:Date;
    @Input() user:User;

    rowDefault =true;
    rowSummary = false;
    rowDetails = false;
    
    loading = false;
    pdfLoading = false;


    constructor( 
        private service:ActivityService,
        private userService: UserService
    )   
    {}

    ngOnInit(){
        this.date = new Date();
        this.date.setDate(15);
        this.date.setMonth(this.month.month - 1);
        if(this.user == null){
            this.userService.current().subscribe(
                res => this.user = <User> res,
                err => this.errorMessage = <any>err
            );
        }
    }


    summary(){
        this.rowDefault = false;
        this.rowSummary = true;
        this.rowDetails = false;
    }
    details(){
        this.rowDefault = false;
        this.rowSummary = false;
        this.rowDetails = true;
    }
    default(){
        this.rowDefault = true;
        this.rowSummary = false;
        this.rowDetails = false;
    }




    isIndexEven() {
        this.index = Number(this.index);
        return this.index === 0 || !!(this.index && !(this.index%2));
    }

}
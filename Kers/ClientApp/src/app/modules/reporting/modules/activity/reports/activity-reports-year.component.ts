import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ActivityService, Activity } from '../activity.service';

import { Router } from "@angular/router";
import { Observable } from 'rxjs';
import { User } from "../../user/user.service";

@Component({
    selector: 'activity-reports-year',
    template: `
    <div class="panel-year">
        <a class="panel-heading" (click)="toggle()">{{year.year}}</a>
        <div class="panel-collapse" *ngIf="condition">
            <activity-reports-month *ngFor="let month of months | async; let i = index" [month]="month" [index]="i" [year]="year" [user]="user"></activity-reports-month>
        </div>
    </div>
        `,
    styles: [`
        .panel-year{
            border-bottom: 1px solid #efefef;
        }
        .panel-heading{
            cursor:pointer;
        }
    `]
})
export class ActivityReportsYearComponent { 


    errorMessage: string;

    months:Observable<string[]>

    condition = false;

    @Input() year;
    @Input() index;
    @Input() user:User;

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
        if(this.user == null){
            this.months = this.service.monthsWithActivities(this.year.year);
        }else{
            this.months = this.service.monthsWithActivities(this.year.year, this.user.id);
        }

        if(this.index == 0){
            this.toggle();
        }
    }

    toggle(){
        if(this.condition){
            this.close();
        }else{
            this.open();
        }
    }
    open(){
        this.condition = true;
    }
    close(){
        this.condition = false;
    }
    

}
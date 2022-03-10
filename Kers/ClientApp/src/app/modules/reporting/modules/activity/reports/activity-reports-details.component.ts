import { Component, Input } from '@angular/core';
import {ActivityService, Activity} from '../activity.service';
import { User } from "../../user/user.service";
import { saveAs } from 'file-saver';

@Component({
    selector: 'activity-reports-details',
    template: `
  <loading *ngIf="loading"></loading>
  <div *ngIf="!loading">
    <div class="text-right">
        <loading [type]="'bars'" *ngIf="pdfLoading"></loading>
        <a class="btn btn-info btn-xs" (click)="print()" *ngIf="!pdfLoading"><i class="fa fa-download"></i> Pdf</a>
        <a class="btn btn-info btn-xs" (click)="csv()"><i class="fa fa-download"></i> Csv</a>
    </div>
        <div class="col-md-12 col-sm-12 col-xs-12" *ngFor="let activity of monthActivities">
            <div class="ln_solid"></div>
                <div class="row">
                    <div class="col-md-6"><h3>{{activity.activityDate| date:'EEEE,  MMMM d, y'}}</h3></div>
                    <div class="col-md-6">Submitted: {{activity.created| date:'MMMM d, y'}}</div>
                </div>
                
                <p><strong>Title: </strong>{{activity.title}}</p>

                <div class="row invoice-info">
                    <div class="col-sm-6 col-xs-12">
                        <strong>Description:</strong>
                        <p innerHtml="{{replaceImageTag(activity.description)}}"></p>
                    </div>
                    <div class="col-sm-6 col-xs-12">
                        <strong>Major Program: </strong>{{activity.majorProgram.name}}<br>
                        <strong>Attendance: </strong>{{ attendance(activity) }}<br>
                        <div *ngFor="let opt of activity.activityOptionSelections">{{opt.activityOption.name.substring(0, opt.activityOption.name.length -1 )}}</div>
                    </div>

                </div>

        </div>
    </div>
        `
})
export class ActivityReportsDetailsComponent { 


    errorMessage: string;
    @Input() month;
    @Input() year;
    @Input() user:User;
    monthActivities: Activity[];
    loading = false;
    pdfLoading = false;
    csvLoading = false;

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
        this.service.activitiesPerMonth(this.month.month, this.year.year, userid, "asc").subscribe(
            res=> {
                this.monthActivities = <Activity[]>res;
                this.loading = false;
            },
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
    replaceImageTag(s:string):string{
        var re = /<img/gi; 
        var str = "Apples are round, and apples are juicy.";
        var newstr = s.replace(re, "<img width=50"); 
        return newstr;
      }

    print(){
        this.pdfLoading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        
        this.service.pdf(this.year.year, this.month.month, userid).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ActivitiesReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfLoading = false;
            },
            err => console.error(err)
        )
    }
    csv(){
        this.csvLoading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        
        this.service.csv(this.year.year, this.month.month, userid).subscribe(
            data => {
                //console.log(data);
                var blob = new Blob([data], {type: 'text/csv'});
                saveAs(blob, "ActivitiesReport_" + this.year.year + "_" + this.month.month + ".csv");
                this.csvLoading = false;
            },
            err => console.error(err)
        )
    }

}
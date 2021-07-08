import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {UserService, User} from '../user.service';
import { Observable } from "rxjs";
import {Router} from '@angular/router';


@Component({
  template: `
    <user-reporting-form [userObservable]="user" (onFormSubmit)="reportingSubmit($event)" (onFormCancel)="reportingCancel()"></user-reporting-form>
    
  `
})
export class UserReportingHomeComponent { 


    user:Observable<User>;
    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private userService: UserService,
        private router:Router 
    )   
    {}

    ngOnInit(){

        this.user = this.userService.current();
        

        this.defaultTitle();
    }

    reportingSubmit(event){
        this.reportingService.setAlert("Reporting Profile Updated");
        this.router.navigate(['/reporting']);
    }
    reportingCancel(){
        this.router.navigate(['/reporting']);
    }

    defaultTitle(){
        this.reportingService.setTitle("Reporting Profile Management");
    }
}
import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { Observable } from "rxjs/Observable";
import { UserService, User } from "../user.service";
import {Router} from '@angular/router'

@Component({
  template: `
    <user-personal-form [userObservable]="user" (onFormSubmit)="personalSubmit($event)"></user-personal-form>
    
  `
})
export class UserPersonalHomeComponent { 

    user:Observable<User>;
    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private userService: UserService,
        private router: Router 
    )   
    {}

    ngOnInit(){
        
        this.user = this.userService.current();

        this.defaultTitle();
    }


    personalSubmit(event){
        this.reportingService.setAlert("Personal Profile Updated");
        this.router.navigate(['/reporting']);
    }

    defaultTitle(){
        this.reportingService.setTitle("Personal Profile Management");
    }
}
import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ContactService, Contact} from '../contact.service';

import { Router } from "@angular/router";


@Component({
  template: `
        <a [routerLink]="['/reporting/contact/stats/']" routerLinkActive="active" [routerLinkActiveOptions]="{exact:
true}">All Statistical Contact Records</a> | 
        <a [routerLink]="['/reporting/contact/stats/month']" routerLinkActive="active">Summary By Month</a> | 
        <a [routerLink]="['/reporting/contact/stats/program']" routerLinkActive="active">Summary By Major Program</a> | 
        <a href="https://kers.ca.uky.edu/kers_mobile/ReportSpecStatsMain.aspx">Statistical Contacts Archive</a>
        <div>
        <router-outlet></router-outlet>
        </div>
        `,
    styles: [
        `
            .active{
                font-weight:bold;
            }
        `
    ]
})
export class ContactStatsHomeComponent { 


    errorMessage: string;


    constructor( 
        private reportingService: ReportingService,
        private router: Router,
        private service:ContactService
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
        
        
    }


    defaultTitle(){
        this.reportingService.setTitle("Statistical Contact Reports");
    }

















}
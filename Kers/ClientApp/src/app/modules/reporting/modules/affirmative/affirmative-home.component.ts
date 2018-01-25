import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {    AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity
    } from './affirmative.service';
import { Router } from "@angular/router";

@Component({
  templateUrl: 'affirmative-home.component.html'
})
export class AffirmativeHomeComponent { 

    
    plan:AffirmativePlan;


    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private service: AffirmativeService,
        private router: Router
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();

        this.plan = this.service.get().map(res=><AffirmativePlan>res);
        //this.populateData();
        
    }

    populateData(){
        this.service.get().subscribe(
            res => {
                this.plan = res;
            },
            error =>  this.errorMessage = <any>error
        );

        
    }

    onFormSubmit(){
        this.reportingService.setAlert("Affirmative Action Plan Submitted");
        this.router.navigate(['/']);
    }
    onFormCancel(){
        this.router.navigate(['/']);
    }

    defaultTitle(){
        this.reportingService.setTitle("Affirmative Action Plan for 2017-2018");
    }
}
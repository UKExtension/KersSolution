import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {    AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity
    } from './affirmative.service';
import { Router, ActivatedRoute } from "@angular/router";
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `<div>This is a total county plan and report. A single Plan is required per year per county.</div>
  <br><reporting-display-help id="2"></reporting-display-help><br>
  <affirmative-form [affirmativePlan]="plan" [isReport]="true" (onFormSubmit)="onFormSubmit()" (onFormCancel)="onFormCancel()"></affirmative-form>`
})
export class AffirmativeHomeReportComponent { 

    
    plan:AffirmativePlan;

    fy:FiscalYear;


    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private service: AffirmativeService,
        private fiscalYearService: FiscalyearService,
        private router: Router,
        private route: ActivatedRoute,
    )   
    {}

    ngOnInit(){
        
        

        
        const fiscalyearid = this.route.snapshot.paramMap.get('fy');
        if( fiscalyearid == null){
            this.fiscalYearService.current("serviceLog").subscribe(
                res =>{
                    this.fy = <FiscalYear> res;
                    this.plan = this.service.get(0, this.fy.name).map(res=><AffirmativePlan>res);
                    this.defaultTitle();
                },
                err => this.errorMessage = <any> err 
            );
        }

        
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
        this.reportingService.setAlert("Affirmative Action Report Submitted");
        this.router.navigate(['/']);
    }
    onFormCancel(){
        this.router.navigate(['/']);
    }

    defaultTitle(){
        this.reportingService.setTitle("Affirmative Action Report for FY " + this.fy.name);
    }
}
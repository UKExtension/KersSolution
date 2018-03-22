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
  templateUrl: 'affirmative-home.component.html'
})
export class AffirmativeHomeComponent { 

    
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
        
        

        this.plan = this.service.get().map(res=><AffirmativePlan>res);
        const fiscalyearid = this.route.snapshot.paramMap.get('fy');
        if( fiscalyearid == null){
            this.fiscalYearService.next("serviceLog").subscribe(
                res =>{
                    this.fy = <FiscalYear> res;
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
        this.reportingService.setAlert("Affirmative Action Plan Submitted");
        this.router.navigate(['/']);
    }
    onFormCancel(){
        this.router.navigate(['/']);
    }

    defaultTitle(){
        this.reportingService.setTitle("Affirmative Action Plan for FY " + this.fy.name);
    }
}
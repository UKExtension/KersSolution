import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import {PlansofworkService, Map, PlanOfWork} from './plansofwork.service';

@Component({
    selector: 'plansofwork-reports',
    templateUrl: 'plansofwork-reports.component.html' 
})
export class PlansofworkReportsComponent implements OnInit{

    plans:PlanOfWork[];

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService,
            private reportingService: ReportingService    
                ){
                    
                }
   
    ngOnInit(){
        this.plansofworkService.listPlansDetails().subscribe(
            plans => {
                this.plans = plans;
                
        },
            error =>  this.errorMessage = <any>error
        );
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Plans of Work Report for 2017-2018");
    }
    

}
import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { ActivatedRoute, Router, Params } from '@angular/router';

@Component({
  template: `
  <div><reporting-display-help id="12"></reporting-display-help></div>
  <div *ngIf="fiscalYear">
    <plansofwork-maps [fy]="fiscalYear"></plansofwork-maps>
    <hr />
    <plansofwork [fy]="fiscalYear"></plansofwork>
  </div>  

  `
})
export class PlansofworkHomeComponent { 

    fiscalYear: FiscalYear;
    fyId: number;
    hasFy: boolean;

    constructor( 
        private reportingService: ReportingService,
        private fiscalYearService: FiscalyearService,
        private route: ActivatedRoute,
        private router: Router
    )   
    {}

    ngOnInit(){
        
        
            this.getNextFiscalYear();
        
    }
    getNextFiscalYear(){
        this.fiscalYearService.next("serviceLog").subscribe(
            res => {
                
                this.fiscalYear = res;
                this.defaultTitle();
            }
        );
    }

    defaultTitle(){
        this.reportingService.setTitle("Plans of Work for FY "+this.fiscalYear.name);
    }
}
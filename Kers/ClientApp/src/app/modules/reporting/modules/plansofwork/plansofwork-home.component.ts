import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `
  <div *ngIf="nextFiscalYear">
    <plansofwork-maps [fy]="nextFiscalYear"></plansofwork-maps>
    <hr />
    <plansofwork [fy]="nextFiscalYear"></plansofwork>
  </div>  

  `
})
export class PlansofworkHomeComponent { 

    nextFiscalYear: FiscalYear;

    constructor( 
        private reportingService: ReportingService,
        private fiscalYearService: FiscalyearService
    )   
    {}

    ngOnInit(){
        
        this.getNextFiscalYear();
    }
    getNextFiscalYear(){
        this.fiscalYearService.next("serviceLog").subscribe(
            res => {
                this.nextFiscalYear = res;
                this.defaultTitle();
            }
        );
    }

    defaultTitle(){
        this.reportingService.setTitle("Plans of Work for FY "+this.nextFiscalYear.name);
    }
}
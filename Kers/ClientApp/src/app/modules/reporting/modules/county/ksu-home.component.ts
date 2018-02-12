import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';


@Component({
  template: `


  <div>
    <div class="row x_title">
        <div class="col-md-6">
        <h3>KSU Employees</h3>
        </div>
                  
   </div>

    <user-directory-list [onlyKSU]="true" [showEmployeeSummaryButton]="true"></user-directory-list>


  </div>
    
  `
})
export class KsuHomeComponent { 


    constructor( 
        private reportingService: ReportingService,
    )   
    {}

    ngOnInit(){
        
        
        
   
    }

    ngOnDestroy(){
        this.reportingService.setTitle( '' );
        this.reportingService.setSubtitle('');
    }

    defaultTitle(){
        this.reportingService.setTitle("KSU Employees" );
    }
}
import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

import { Router } from "@angular/router";

@Component({
  template: `<router-outlet></router-outlet>`
})
export class ExpenseComponent { 


    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private router: Router,
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
        
        
        
    }
    

    defaultTitle(){
        this.reportingService.setTitle("Expense Records");
    }
}
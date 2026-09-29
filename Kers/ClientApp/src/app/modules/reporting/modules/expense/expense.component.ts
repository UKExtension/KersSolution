import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

import { Router } from "@angular/router";

@Component({
    selector: 'expense-root',
    template: `<router-outlet></router-outlet>`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
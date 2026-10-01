import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

@Component({
    template: `
    <router-outlet></router-outlet>
    
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ServicelogComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Service Log");
    }
}
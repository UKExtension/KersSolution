import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

@Component({
    selector: 'activity-root',
    template: `
    <router-outlet></router-outlet>
    
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ActivityComponent { 

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
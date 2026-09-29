import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';

@Component({
    selector: 'snaped-root',
    template: `
    <router-outlet></router-outlet>
    
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SnapedComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Snap-Ed Admin");
    }
}
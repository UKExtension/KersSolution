import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';

@Component({
    selector: 'navigation-home-root',
    template: `
    <router-outlet></router-outlet>
    

  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NavigationHomeComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Navigation Management");
    }
}
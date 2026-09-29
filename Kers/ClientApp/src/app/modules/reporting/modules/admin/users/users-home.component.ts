import { Component, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';

@Component({
    selector: 'users-home-root',
    template: `
    <router-outlet></router-outlet>
    

  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class UsersHomeComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Users Management");
    }
}
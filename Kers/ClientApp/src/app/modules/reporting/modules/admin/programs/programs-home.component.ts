import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';

@Component({
  template: `
    <router-outlet></router-outlet>
    

  `
})
export class ProgramsHomeComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Major Programs Management");
    }
}
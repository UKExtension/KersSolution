import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

@Component({
  template: `
    <router-outlet></router-outlet>
    
  `
})
export class StoryComponent { 

    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){ 
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Success Stories");
    }
}
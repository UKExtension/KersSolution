import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';

@Component({
  template: `
    <plansofwork-maps></plansofwork-maps>
    <hr />
    <plansofwork></plansofwork>
    

  `
})
export class PlansofworkHomeComponent { 

    constructor( 
        private reportingService: ReportingService
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Plans of Work for 2017-2018");
    }
}
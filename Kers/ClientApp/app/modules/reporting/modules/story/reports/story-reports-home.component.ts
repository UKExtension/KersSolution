import { Component, Input } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';


@Component({
    template: `<br><br>
    <success-story-list></success-story-list>
    <div class="text-right"><a class="btn btn-default btn-xs" href="https://kers.ca.uky.edu/kers_mobile/ReportSuccessStoriesMain.aspx">Success Stories Reports Archive</a></div>
    
    `
})
export class StoryReportsHomeComponent { 

   

    constructor(  
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){ 
        this.reportingService.setTitle("Success Stories Report");
     
    }

    
}
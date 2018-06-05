import { Component, Input } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';


@Component({
    template: `
    <fiscal-year-switcher [initially]="'current'" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    <br><br>
    <success-story-list *ngIf="fiscalYear != null" [fiscalYear]="fiscalYear"></success-story-list>
    <div class="text-right"><a class="btn btn-default btn-xs" href="https://kers.ca.uky.edu/kers_mobile/ReportSuccessStoriesMain.aspx">Success Stories Reports Archive</a></div>
    
    `
})
export class StoryReportsHomeComponent { 

    public fiscalYear:FiscalYear;

    constructor(  
        private reportingService: ReportingService 
    )   
    {}

    fiscalYearSwitched(event:FiscalYear){
        this.fiscalYear = null;
        var self = this;
        setTimeout(function(){
            self.fiscalYear = event;
        }, 4);
        
    }
    
    

    ngOnInit(){ 
        this.reportingService.setTitle("Success Stories Report");
     
    }

    
}
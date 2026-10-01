import {Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy} from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';



@Component({
    template: `<programindicators-initiatives-admin></programindicators-initiatives-admin>`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ProgramindicatorsHomeComponent{
    constructor( 
        private reportingService: ReportingService 
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Program Indicators Management");
    }

}
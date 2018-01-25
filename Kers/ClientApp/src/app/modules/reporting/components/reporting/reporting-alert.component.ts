import { Component, OnInit } from '@angular/core';
import {ReportingService} from './reporting.service';

@Component({
    selector: 'reporting-alert',
    template: `
<div class="alert alert-success" *ngIf="alert.name != ''">
<button type="button" class="close" (click)="dismiss()"><span>&times;</span></button>

        <i class="fa fa-info-circle fa-lg"></i> {{alert.name}}

    
</div>
  `
})
export class ReportingAlertComponent implements OnInit { 
  public alert;

  constructor( 
        private reportingService: ReportingService
        ) 
    {
        
    }

    ngOnInit(){
        this.alert = this.reportingService.alert;
    }

    dismiss(){
        this.reportingService.setAlert("");
    }


}

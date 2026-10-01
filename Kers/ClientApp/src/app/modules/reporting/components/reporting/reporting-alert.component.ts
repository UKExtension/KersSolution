import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from './reporting.service';
import { MessageService } from '../../core/services/message.service';

@Component({
    selector: 'reporting-alert',
    template: `


@if (messageService.messages.length) {
  <div class="alert alert-danger">
    <button type="button" class="close" (click)="messageService.clear()"><span>&times;</span></button>
    @for (message of messageService.messages; track message) {
      <div> {{message}} </div>
    }
  </div>
}

@if (alert.name != '') {
  <div class="alert alert-success">
    <button type="button" class="close" (click)="dismiss()"><span>&times;</span></button>
    <i class="fa fa-info-circle fa-lg"></i> {{alert.name}}
  </div>
}
`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ReportingAlertComponent implements OnInit { 
  public alert;

  constructor( 
        private reportingService: ReportingService,
        public messageService: MessageService
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

import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
    selector: 'meeting-home',
    template: `
    <router-outlet></router-outlet>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MeetingHomeComponent implements OnInit {

  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("Cooperative Extension Events");
    this.reportingService.setSubtitle("");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  templateUrl: './calendar-home.component.html',
  styleUrls: ['./calendar-home.component.css']
})
export class CalendarHomeComponent implements OnInit {

  constructor(
    private reportingService: ReportingService 
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("Calendar of Activities");
  }

  ngOnDestroy(){
    this.reportingService.setTitle( '' );
    this.reportingService.setSubtitle('');
  }

}

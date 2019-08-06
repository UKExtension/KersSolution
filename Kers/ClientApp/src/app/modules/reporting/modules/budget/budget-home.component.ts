import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'app-budget-home',
  templateUrl: './budget-home.component.html',
  styles: []
})
export class BudgetHomeComponent implements OnInit {

  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }
  defaultTitle(){
    this.reportingService.setTitle("County Budget");
    this.reportingService.setSubtitle("County Cooperative Extension Service Budget Plan");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

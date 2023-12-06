import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'exempt',
  templateUrl: './exempt.component.html'
})
export class ExemptComponent implements OnInit {
  newExempt:boolean = false;

  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit(): void {
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("Tax Exempt/Volunteer Entities Management");
    //this.reportingService.setSubtitle("For specific In-Service related questions or assistance, please email: agpsd@lsv.uky.edu");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

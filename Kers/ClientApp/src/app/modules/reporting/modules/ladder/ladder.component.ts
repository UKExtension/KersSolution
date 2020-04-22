import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'ladder-home',
  template: `
  <ladder-application-form></ladder-application-form>
    <p>
      ladder works!
    </p>
  `,
  styles: []
})
export class LadderComponent implements OnInit {

  constructor(
    private reportingService: ReportingService
  ) {
    
   }

  ngOnInit() {
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("Professional Promotion Application");
    this.reportingService.setSubtitle("For Outstanding Job Performance and Experiences Gained Through Program Development");
  }

}

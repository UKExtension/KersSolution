import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'demo',
  template: `
    <h4>Experimental functionality to be considered for implementation in the future.</h4>
    <p>It serves demonstration purpose and won't work or save entered test data.</p>
    <a [routerLink]="['/reporting/demos/indicators']" routerLinkActive="active" class="btn btn-round btn-success"  [routerLinkActiveOptions]="{exact:
      true}">Program Indicators</a>
    <a [routerLink]="['/reporting/demos/plans']" routerLinkActive="active" class="btn btn-round btn-success">Plan of Work</a>
    <router-outlet></router-outlet>
  `,
  styles: [
  ]
})
export class DemoComponent implements OnInit {

  constructor(
    private reportingService:ReportingService
  ) { }

  ngOnInit(): void {
    this.defaultTitle()
  }
  

  defaultTitle(){
    this.reportingService.setTitle("Demos");
  }

}

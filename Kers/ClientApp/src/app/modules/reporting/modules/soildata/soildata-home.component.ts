import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'app-soildata-home',
  template: `
    <div>
      <p>
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/reports']" routerLinkActive="active">Reports</a> 
        <a class="btn btn-default" [routerLink]="['/reporting/soildata/addresses']" routerLinkActive="active">Client Addresses</a> 
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/notes']" routerLinkActive="active">Notes</a> 
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/signees']" routerLinkActive="active">Signees</a> 
      </p>
    </div>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class SoildataHomeComponent implements OnInit {

  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }
  defaultTitle(){
    this.reportingService.setTitle("Soil Testing");
    this.reportingService.setSubtitle("");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

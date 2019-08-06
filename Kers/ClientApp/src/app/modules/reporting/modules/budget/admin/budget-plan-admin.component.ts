import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'app-budget-plan-admin',
  template: `
  
    <p>
    <a class="btn btn-default" [routerLink]="['/reporting/budget/admin/operations']" routerLinkActive="active">Office Operations</a>
    </p>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class BudgetPlanAdminComponent implements OnInit {

  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("County Budget Planning");
    this.reportingService.setSubtitle("Administration");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

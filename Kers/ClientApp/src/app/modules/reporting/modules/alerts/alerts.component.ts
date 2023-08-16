import { Component, OnInit } from '@angular/core';
import { Alert } from './Alert';
import { AlertsService } from './alerts.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'alerts',
  templateUrl: './alerts.component.html'
})
export class AlertsComponent implements OnInit {
  newAlert = false;
  condition = false;

  activeAlerts$:Observable<Alert[]>;
  pastAlerts$:Observable<Alert[]>;
  refresh$ = new BehaviorSubject(true);

  constructor(
    private reportingService: ReportingService,
    service: AlertsService
  ) {
    this.activeAlerts$ = this.refresh$
      .pipe(
          switchMap(() => service.getAlerts(1))
      );
    this.pastAlerts$ = this.refresh$
      .pipe(
          switchMap(() => service.getAlerts(2))
      );
   }

  ngOnInit(): void {
    this.defaultTitle();
  }

  onSubmit(event:Alert){
    this.newAlert = false;
    this.refreshData();
  }

  refreshData() {
    this.refresh$.next(true);
  }
  defaultTitle(){
    this.reportingService.setTitle("Alerts Management");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }

}

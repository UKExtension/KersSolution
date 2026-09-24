import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AlertsService } from '../../modules/alerts/alerts.service';
import { Observable } from 'rxjs';
import { Alert } from '../../modules/alerts/Alert';

@Component({
    selector: 'alert-widget',
    template: `
  <alert-banner *ngFor="let alert of alerts$ | async " [alert]="alert"></alert-banner>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class AlertWidgetComponent implements OnInit {
  alerts$:Observable<Alert[]>;

  constructor(
    private router: Router,
    private service:AlertsService
  ) {

    router.events.pipe(
        filter(event => event instanceof NavigationEnd)
      ).subscribe(event => {
          this.alerts$ = service.getPage(event["url"]);
      });
   }

  ngOnInit(): void {
    
  }

}

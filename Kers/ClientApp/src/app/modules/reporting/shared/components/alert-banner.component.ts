import { Component, Input, OnInit } from '@angular/core';
import { Alert, AlertTypes } from '../../modules/alerts/Alert';

@Component({
  selector: 'alert-banner',
  template: `
  <div class="alert {{alertClass}} alert-dismissible" role="alert" *ngIf="!hidden">
    <button (click)="hidden=true" type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
    </button>
    {{alert.message}}
    <div *ngIf="alert.moreInfoUrl != ''"><br>
    <a href="{{alert.moreInfoUrl}}" target="_blank">Visit for More Info</a>
    </div>
  </div>
  `,
  styles: [`
  .alert-danger a{
    color:#333;
  }
  
  `
  ]
})
export class AlertBannerComponent implements OnInit {
  hidden = false;
  @Input() alert:Alert;
  alertClass="";

  constructor() { }

  ngOnInit(): void {
    this.alertClass = AlertTypes.filter( a => a.id == this.alert.alertType)[0].class;
  }


}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { AlertsRoutingModule } from './alerts-routing.module';
import { AlertsComponent } from './alerts.component';
import { AlertsFormComponent } from './alerts-form.component';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';



@NgModule({
  declarations: [
    AlertsComponent,
    AlertsFormComponent
  ],
  imports: [
    CommonModule,
    AlertsRoutingModule,
    SharedModule,
    AngularMyDatePickerModule,
  ]
})
export class AlertsModule { }

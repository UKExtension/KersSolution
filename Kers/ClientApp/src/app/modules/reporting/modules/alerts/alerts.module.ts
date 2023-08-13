import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { AlertsRoutingModule } from './alerts-routing.module';
import { AlertsComponent } from './alerts.component';
import { AlertsFormComponent } from './alerts-form.component';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';
import { UsersModule } from '../admin/users/users.module';
import { AlertListRowComponent } from './alert-list-row.component';



@NgModule({
  declarations: [
    AlertsComponent,
    AlertsFormComponent,
    AlertListRowComponent
  ],
  imports: [
    CommonModule,
    AlertsRoutingModule,
    SharedModule,
    AngularMyDatePickerModule,
    UsersModule
  ]
})
export class AlertsModule { }

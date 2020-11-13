import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { FormsModule } from '@angular/forms';

import { CalendarRoutingModule } from './calendar-routing.module';
import { KersCalendarComponent } from './kers-calendar.component';
import { CalendarService } from './calendar-service.service';
import { SharedModule } from '../../shared/shared.module';
import { ServicelogModule } from '../servicelog/servicelog.module';
import { ServicelogFormComponent } from '../servicelog/servicelog-form.component';
import { CalendarDayEventsComponent } from './calendar-day-events.component';
import { CalendarHeaderComponent } from './calendar-header.component';
import { ExpenseModule } from '../expense/expense.module';
import { MileageModule } from '../mileage/mileage.module';
import { CalendarEventDetailComponent } from './calendar-event-detail.component';
import { CalendarHomeComponent } from './calendar-home.component';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';


@NgModule({
  imports: [
    SharedModule,
    CommonModule,
    FormsModule,
    CalendarRoutingModule,
    ServicelogModule,
    ExpenseModule,
    MileageModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
  ],
  declarations: [
    KersCalendarComponent,
    CalendarDayEventsComponent,
    CalendarHeaderComponent,
    CalendarEventDetailComponent,
    CalendarHomeComponent
  ],
  providers: [
    CalendarService
  ],
  exports: [
    KersCalendarComponent
  ]
})
export class KersCalendarModule { }

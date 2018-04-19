import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule } from 'angular-calendar';
import { FormsModule } from '@angular/forms';

import { CalendarRoutingModule } from './calendar-routing.module';
import { KersCalendarComponent } from './kers-calendar.component';
import { CalendarService } from './calendar-service.service';
import { SharedModule } from '../../shared/shared.module';
import { ServicelogModule } from '../servicelog/servicelog.module';
import { ServicelogFormComponent } from '../servicelog/servicelog-form.component';
import { CalendarDayEventsComponent } from './calendar-day-events.component';
import { CalendarHeaderComponent } from './calendar-header.component';


@NgModule({
  imports: [
    SharedModule,
    CommonModule,
    FormsModule,
    CalendarRoutingModule,
    ServicelogModule,
    CalendarModule.forRoot(),
  ],
  declarations: [
    KersCalendarComponent,
    CalendarDayEventsComponent,
    CalendarHeaderComponent
  ],
  providers: [CalendarService]
})
export class KersCalendarModule { }

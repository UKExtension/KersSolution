import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FullCalendarModule } from 'ng-fullcalendar';

import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarHomeComponent } from './calendar-home.component';

@NgModule({
  imports: [
    CommonModule,
    CalendarRoutingModule,
    FullCalendarModule
  ],
  declarations: [CalendarHomeComponent]
})
export class CalendarModule { }

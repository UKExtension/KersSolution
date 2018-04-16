import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule } from 'angular-calendar';
import { FormsModule } from '@angular/forms';

import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarHomeComponent } from './calendar-home.component';
import { CalendarService } from './calendar-service.service';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    CalendarRoutingModule,
    CalendarModule.forRoot(),
  ],
  declarations: [CalendarHomeComponent],
  providers: [CalendarService]
})
export class KersCalendarModule { }

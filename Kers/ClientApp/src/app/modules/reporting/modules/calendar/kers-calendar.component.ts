import {
  Component,
  ChangeDetectionStrategy,
  ViewChild,
  TemplateRef,
  OnInit
} from '@angular/core';
import {
  startOfMonth,
  endOfDay,
  subDays,
  addDays,
  endOfMonth,
  startOfWeek,
  endOfWeek,
  startOfDay,
  format,
  isSameDay,
  isSameMonth,
  addHours
} from 'date-fns';
import { Subject } from 'rxjs/Subject';
import {
  CalendarEvent,
  CalendarEventAction,
  CalendarEventTimesChangedEvent
} from 'angular-calendar';
import { Observable } from 'rxjs/Observable';
import { Activity } from '../activity/activity.service';
import { CalendarService } from './calendar-service.service';

@Component({
  selector: 'kers-calendar',
  templateUrl: './kers-calendar.component.html',
  styleUrls: ['./kers-calendar.component.css']
})
export class KersCalendarComponent implements OnInit {
  
  view: string = 'month';

  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();

  events$: Observable<Array<CalendarEvent<{ id: number, type: string }>>>;

  activeDayIsOpen: boolean = true;


  constructor(
    private service:CalendarService
  ) {}

  ngOnInit(): void {
    this.fetchEvents();
  }

  fetchEvents(): void {
    const getStart: any = {
      month: startOfMonth,
      week: startOfWeek,
      day: startOfDay
    }[this.view];

    const getEnd: any = {
      month: endOfMonth,
      week: endOfWeek,
      day: endOfDay
    }[this.view];
    var start = format(getStart(this.viewDate), 'YYYY-MM-DD');
    var end = format(getEnd(this.viewDate), 'YYYY-MM-DD');
    this.events$ = this.service.eventsPerPeriod(getStart(this.viewDate), getEnd(this.viewDate));

  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    
    if (isSameMonth(date, this.viewDate)) {
      if (
        isSameDay(this.viewDate, date) && this.activeDayIsOpen === true
      ) {
        this.activeDayIsOpen = false;
      } else {     
        this.viewDate = date;
        this.activeDayIsOpen = true;
      }
    }
    
  }

  viewDateChange(){
    this.activeDayIsOpen = false;
    this.fetchEvents();
  }


}

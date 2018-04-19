import { Component, OnInit, Input } from '@angular/core';
import { CalendarEvent } from 'angular-calendar';

@Component({
  selector: 'calendar-day-events',
  templateUrl: './calendar-day-events.component.html',
  styleUrls: ['./calendar-day-events.component.css']
})
export class CalendarDayEventsComponent implements OnInit {


  private _viewDate:Date;

  get viewDate(){
    return this._viewDate;
  }

  @Input() 
  set viewDate( viewDate: Date){
    this.logOpened = false;
    this._viewDate = viewDate;
  }
  @Input() events: Array<CalendarEvent<{ id: number, type: string }>>;
  today:Date = new Date();
  logOpened = false;

  constructor() { }

  ngOnInit() {
  }

  logSubmit(event){
    this.logOpened = false;
  }

}

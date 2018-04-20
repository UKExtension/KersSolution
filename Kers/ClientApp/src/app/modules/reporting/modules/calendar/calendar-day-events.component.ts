import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
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
  @Output() changed: EventEmitter<void> = new EventEmitter();





  today:Date = new Date();
  logOpened = false;
  expenseOpened = false;

  constructor() { }

  ngOnInit() {
  }

  logSubmit(event){
    this.logOpened = false;
    this.changed.emit();
  }
  expenseSubmit(event){
    this.expenseOpened = false;
    this.changed.emit();
  }

}

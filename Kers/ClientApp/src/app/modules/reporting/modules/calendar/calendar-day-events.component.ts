import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CalendarEvent } from 'angular-calendar';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';


@Component({
  selector: 'calendar-day-events',
  templateUrl: './calendar-day-events.component.html',
  styleUrls: ['./calendar-day-events.component.css']
})
export class CalendarDayEventsComponent implements OnInit {


  private _viewDate:Date;
  private displayServiceLogEdits = false;
  currentFiscalYear:FiscalYear | null = null;

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

  constructor(
    private fiscalYearService:FiscalyearService
  ) { }

  ngOnInit() {
    this.fiscalYearService.current().subscribe(
      res =>{
          this.currentFiscalYear = res;

          if( 
              new Date(this.currentFiscalYear.start) <= new Date(this._viewDate) 
              && 
              new Date(this.currentFiscalYear.end) >= new Date(this._viewDate)
          ){
              this.displayServiceLogEdits = true;
          }
      } 
 );
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

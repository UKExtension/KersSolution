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
  displayNewMileage = false;

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
    var viewDate = new Date(this._viewDate);
    var novFirst = new Date(2020,9, 31);
    if( viewDate < novFirst ){
        this.displayNewMileage = true;
    }
    this.fiscalYearService.current('serviceLog', true).subscribe(
      res =>{
          this.currentFiscalYear = res;
          
          var start = new Date(this.currentFiscalYear.start);
          var end = new Date(this.currentFiscalYear.end);
          var extendedTo = new Date(this.currentFiscalYear.extendedTo);
          if(extendedTo > start) end = extendedTo;

          if( 
              start <  viewDate
              && 
              end > viewDate
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

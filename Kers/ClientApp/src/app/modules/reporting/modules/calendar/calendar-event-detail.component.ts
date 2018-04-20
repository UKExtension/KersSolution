import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { CalendarEvent } from 'angular-calendar';
import { Expense, ExpenseService } from '../expense/expense.service';
import { ServicelogService, Servicelog } from '../servicelog/servicelog.service';

@Component({
  selector: 'calendar-event-detail',
  templateUrl: './calendar-event-detail.component.html',
  styleUrls: ['./calendar-event-detail.component.css']
})
export class CalendarEventDetailComponent implements OnInit {
  @Input() event:CalendarEvent;
  @Output() change: EventEmitter<void> = new EventEmitter;

  expenseEdit = false;
  activityEdit = false;

  expense:Expense;
  activity:Servicelog;

  loading=false;

  errorMessage:string;

  constructor(
    private expenseService:ExpenseService,
    private activityService:ServicelogService
  ) { }

  ngOnInit() {
    
  }

  edit(){
    this.loading = true;
    if(this.event.meta.type == "expense"){
      this.expenseService.byRevId(this.event.meta.id).subscribe(
        res => {
          this.expense = <Expense> res;
          this.expenseEdit = true;
          this.loading = false;
        },
        err => this.errorMessage = <any> err
      );
    }else if( this.event.meta.type == "activity"){
      this.activityService.byId(this.event.meta.id).subscribe(
        res => {
          this.activity = <Servicelog> res;
          this.activityEdit = true;
          this.loading = false;
        },
        err => this.errorMessage = <any> err
      );
      
    }
  }

  formCancel(){
    this.expenseEdit = false;
    this.activityEdit = false;
  }

  formSubmit(){
    
    this.expenseEdit = false;
    this.activityEdit = false;
    this.change.emit();
  }
}

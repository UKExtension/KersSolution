import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, combineLatest } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Activity } from '../activity/activity.service';
import {CalendarEvent} from 'angular-calendar';
import { Expense } from '../expense/expense.service';

import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';

@Injectable()
export class CalendarService {
  private expenseBaseUrl = '/api/expense/';
  private activityBaseUrl = '/api/activity/';
  private handleError: HandleError;

  constructor( 
    private http: HttpClient, 
    private location:Location,
    httpErrorHandler: HttpErrorHandler
    ) {
        this.handleError = httpErrorHandler.createHandleError('CalendarService');
    }

  activitiesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var url = this.activityBaseUrl + 'perPeriodLite/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
    return this.http.get<Activity[]>(this.location.prepareExternalUrl(url))
      .pipe(
        map(res =>{
          var activities = res;
          var events:CalendarEvent[] = [];
          for( let activity of activities){
            var event:CalendarEvent = <CalendarEvent>{};
            var dt = new Date(activity.activityDate);
            event.start = dt;
            event.allDay = true;
            event.title = activity.title;
            event.color = calendarColors.activity;
            event.meta = {id:activity.id, type: 'activity'}
            events.push(event);
          }
          return events;
        }),
        catchError(this.handleError('activitiesPerPeriod', []))
      );

  }


  expensesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var url = this.expenseBaseUrl + 'perPeriodLite/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
    return this.http.get<Expense[]>(this.location.prepareExternalUrl(url))
    .pipe(
      map(res =>{
            var expenses = res;
            var events:CalendarEvent[] = [];
            for( let expense of expenses){
              var event:CalendarEvent = <CalendarEvent>{};
              var dt = new Date(expense.expenseDate);
              event.start = dt;
              event.allDay = true;
              event.title = expense.segments == null ? expense.expenseLocation : expense.segments[0].location.address.building + ", " + expense.segments[0].location.address.street + ", " + expense.segments[0].location.address.city;
              event.color = calendarColors.expense;
              event.meta = {id:expense.id, type: 'expense'}
              events.push(event);
            }
            return events;
      }),
      catchError(this.handleError('expensesPerPeriod', []))
    );
  }

  eventsPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var expenses = this.expensesPerPeriod(start, end, userId);
    var activity = this.activitiesPerPeriod(start, end, userId);

    var merged = combineLatest(expenses, activity)
                    .pipe(
                      map(([bT, sT]) => [...bT, ...sT])
                    );

    return merged;
  }

}

export const calendarColors: any = {
  activity: {
    primary: '#394D5F',
    secondary: '#394D5F'
  },
  expense: {
    primary: '#169F85',
    secondary: '#169F85'
  },
  training: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  }
};

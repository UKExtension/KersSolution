import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { AuthHttp } from '../../../authentication/auth.http';
import { Http, Response, Headers, RequestOptions, URLSearchParams, ResponseContentType } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import { Activity } from '../activity/activity.service';
import {CalendarEvent} from 'angular-calendar';
import { Expense } from '../expense/expense.service';


import { concat } from 'rxjs/observable/concat';
import { merge } from 'rxjs/observable/merge';
import { forkJoin } from 'rxjs/observable/forkJoin';
import 'rxjs/add/operator/mergeMap';
import { combineLatest } from 'rxjs/observable/combineLatest';

@Injectable()
export class CalendarService {
  private expenseBaseUrl = '/api/expense/';
  private activityBaseUrl = '/api/activity/';

  constructor(
      private http:AuthHttp, 
        private location:Location
  ) { }

  activitiesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var url = this.activityBaseUrl + 'perPeriodLite/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
    return this.http.get(this.location.prepareExternalUrl(url))
            .map(res =>{
              var activities = <Activity[]>res.json();
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

            } )
            .catch(this.handleError);
  }


  expensesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var url = this.expenseBaseUrl + 'perPeriodLite/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
    return this.http.get(this.location.prepareExternalUrl(url))
            .map(res =>{
              var activities = <Expense[]>res.json();
              var events:CalendarEvent[] = [];
              for( let expense of activities){
                var event:CalendarEvent = <CalendarEvent>{};
                var dt = new Date(expense.expenseDate);
                event.start = dt;
                event.allDay = true;
                event.title = expense.expenseLocation;
                event.color = calendarColors.expense;
                event.meta = {id:expense.id, type: 'expense'}
                events.push(event);
              }
              
              return events;

            } )
            .catch(this.handleError);
  }

  eventsPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent<{ id: number, type: string }>[]>{
    var expenses = this.expensesPerPeriod(start, end, userId);
    var activity = this.activitiesPerPeriod(start, end, userId);

    var merged = combineLatest(expenses, activity)
                    .map(([bT, sT]) => [...bT, ...sT]);

    return merged;
  }


  getRequestOptions(){
    return new RequestOptions(
      {
          headers: new Headers({
              "Content-Type": "application/json; charset=utf-8"
          })
      }
    )
  }

  handleError(err:Response){
      console.error(err);
      return Observable.throw(err.json().error || 'Server error');
  }

}

export const calendarColors: any = {
  activity: {
    primary: '#394D5F',
    secondary: '#286090'
  },
  expense: {
    primary: '#169F85',
    secondary: '#ec971f'
  },
  training: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  }
};

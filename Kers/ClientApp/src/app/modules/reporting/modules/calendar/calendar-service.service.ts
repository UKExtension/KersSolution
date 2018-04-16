import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { AuthHttp } from '../../../authentication/auth.http';
import { Http, Response, Headers, RequestOptions, URLSearchParams, ResponseContentType } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import { Activity } from '../activity/activity.service';
import {CalendarEvent} from 'angular-calendar';

@Injectable()
export class CalendarService {
  private expenseBaseUrl = '/api/expense/';
  private activityBaseUrl = '/api/activity/';

  constructor(
      private http:AuthHttp, 
        private location:Location
  ) { }

  activitiesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<CalendarEvent[]>{
    var url = this.activityBaseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
    return this.http.get(this.location.prepareExternalUrl(url))
            .map(res =>{
              var activities = <Activity[]>res.json();
              var events:CalendarEvent[] = [];
              for( var activity of activities){
                var event:CalendarEvent;
                event.start = activity.activityDate;
                event.allDay = true;
                event.title = activity.title;
                event.color = {primary: '#ccc', secondary: '#cca'};
                event.meta = {activity:activity}
                events.push(event);
              }

              return events;

            } )
            .catch(this.handleError);
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

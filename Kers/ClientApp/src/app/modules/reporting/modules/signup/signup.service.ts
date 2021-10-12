import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { Ethnicity, Race } from '../activity/activity.service';

@Injectable({
  providedIn: 'root'
})
export class SignupService {

  private baseUrl = '/api/SignUp/';
  private handleError: HandleError;

  constructor( 
      private http:HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('SignUp');
      }




    hasAttendance( activityId:number ):Observable<boolean>{
        var url = this.baseUrl + 'hasattendance/' + activityId;
            return this.http.get<boolean>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('hasattendance', false))
                );
    }

    attendedBy( activityId:number ):Observable<ActivitySignUpEntry[]>{
        var url = this.baseUrl + 'attendedby/' + activityId;
            return this.http.get<ActivitySignUpEntry[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('attendedby', []))
                );
    }

    /*****************************/
    // CRUD operations
    /*****************************/

    add( signup:ActivitySignUpEntry ):Observable<ActivitySignUpEntry>{
        return this.http.post<ActivitySignUpEntry>(this.location.prepareExternalUrl(this.baseUrl + "add"), signup)
            .pipe(
                catchError(this.handleError('add', <ActivitySignUpEntry>{}))
            );
    }

    update(id:number, unit:ActivitySignUpEntry):Observable<ActivitySignUpEntry>{
        var url = this.baseUrl + "update/" + id;
        return this.http.put<ActivitySignUpEntry>(this.location.prepareExternalUrl(url), unit)
            .pipe(
                catchError(this.handleError('update', <ActivitySignUpEntry>{}))
            );
    }
    delete(id:number):Observable<{}>{
        var url = this.baseUrl + "delete/" + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }


    csv(activityId:number): Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/SignUp/attendiescsv/' + activityId + '/data.csv'), {responseType: 'blob'})
        .pipe(
            catchError(this.handleError('assistantReimbursments', <Blob>{}))
        );
    }

}


export class ActivitySignUpEntry{
  id:number;
  name:string;
  address:string;
  email:string;
  gender:number;
  race:Race;
  raceId?:number;
  ethnicity:Ethnicity;
  ethnicityId?:number;
  activityId:number;
}
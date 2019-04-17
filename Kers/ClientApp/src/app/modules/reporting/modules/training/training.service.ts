import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import {Training, TainingRegisterWindow, TainingInstructionalHour, TrainingCancelEnrollmentWindow} from './training'

@Injectable({
  providedIn: 'root'
})
export class TrainingService {

  private baseUrl = '/api/Trainings/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('TrainingService');
      }

      range(skip:number = 0, take:number = 10, order = "start"):Observable<Training[]>{
        var url = this.baseUrl + "rangetrainings/"+skip+"/"+take+"/"+order;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('range', []))
            );
      }
      perPeriod(start:Date, end:Date, order:string = "start"):Observable<Training[]>{
        var url = this.baseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + order  ;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perPeriod', []))
            );
    }

      registerWindows() : Observable<TainingRegisterWindow[]>{
        var url = this.baseUrl + "RegisterWindows/";
        return this.http.get<TainingRegisterWindow[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('registerWindows', []))
            );
      }

      instructionalHours() : Observable<TainingInstructionalHour[]>{
        var url = this.baseUrl + "InstructionalHours/";
        return this.http.get<TainingInstructionalHour[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('instructionalHour', []))
            );
      }
      cancelEnrollmentWindows() : Observable<TrainingCancelEnrollmentWindow[]>{
        var url = this.baseUrl + "CancelEnrollmentWindows/";
        return this.http.get<TrainingCancelEnrollmentWindow[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('cancelEnrollmentWindows', []))
            );
      }

      add( training:Training ):Observable<Training>{
          console.log(training);
        return this.http.post<Training>(this.location.prepareExternalUrl(this.baseUrl + 'addtraining/'), training)
            .pipe(
                catchError(this.handleError('add', <Training>{}))
            );
      }
      delete(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
      }

      update(id:number, training:Training):Observable<Training>{
        var url = this.baseUrl + 'updatetraining/' + id;
        return this.http.put<Training>(this.location.prepareExternalUrl(url), training)
                .pipe(
                    catchError(this.handleError('update', training))
                );
      }  

}









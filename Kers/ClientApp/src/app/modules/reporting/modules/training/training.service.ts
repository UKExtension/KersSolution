import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap, retry } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import {Training, TainingRegisterWindow, TainingInstructionalHour, TrainingCancelEnrollmentWindow, TrainingEnrollment, TrainingSurveyResult} from './training'
import { User } from '../user/user.service';

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
      getCustom(searchParams?:{}):Observable<Training[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
      }
      getTraining(id:number):Observable<Training>{
        var url = this.baseUrl + "get/" + id;
        return this.http.get<Training>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('registerWindows', <Training>{}))
            );
      }
      enrolledByUser(userId:number = 0, year:number = 0):Observable<Training[]>{
        var url = this.baseUrl + "byuser/" + userId + '/' + year;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('enrolledByUser', []))
            );
      }
      proposals():Observable<Training[]>{
        var url = this.baseUrl + "proposalsawaiting/";
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('proposals', []))
            );
      }

      trainingsbystatus(year:number, status:string):Observable<Training[]>{
        var url = this.baseUrl + "trainingsbystatus/" + year + "/" + status;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('trainingsbystatus', []))
            );
      }

      proposedByUser(userId:number = 0, year:number = 0):Observable<Training[]>{
        var url = this.baseUrl + "proposedbyuser/" + userId + '/' + year;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('proposedByUser', []))
            );
      }
      updateAttendance(id:number, training:Training):Observable<Training>{
        var url = this.baseUrl + 'postattendance/' + id;
        return this.http.put<Training>(this.location.prepareExternalUrl(url), training)
                .pipe(
                    catchError(this.handleError('updateAttendance', training))
                );
      } 
      upcommingByUser(userId:number = 0):Observable<Training[]>{
        var url = this.baseUrl + "upcomming/" + userId ;
        return this.http.get<Training[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('enrolledByUser', []))
            );
      }

      usersWithTrainings(year:number = 0):Observable<User[]>{
        var url = this.baseUrl + "userswithtrainings/" + year ;
        return this.http.get<User[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('usersWithTrainings', []))
            );
      }

      getServices(amount:number = 20, notConverted:boolean = true, order:string = "DESC"): Observable<Object[]>{
        var url = this.baseUrl + "getservices/" + amount + "/" + notConverted + "/" + order;
        return this.http.get<Object[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getServices', []))
            );
      }
      migrate( id:number ):Observable<Training>{
        var url = this.baseUrl + "migrate/" + id;
        return this.http.get<Training>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('migrate', <Training>{}))
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

      addSurvey( survey:TrainingSurveyResult ):Observable<TrainingSurveyResult>{
        return this.http.post<TrainingSurveyResult>(this.location.prepareExternalUrl(this.baseUrl + 'addsurvey/'), survey)
            .pipe(
                catchError(this.handleError('addSurvey', <TrainingSurveyResult>{}))
            );
      }

      add( training:Training ):Observable<Training>{
        return this.http.post<Training>(this.location.prepareExternalUrl(this.baseUrl + 'addtraining/'), training)
            .pipe(
                catchError(this.handleError('add', <Training>{}))
            );
      }
      enroll( training:Training ):Observable<TrainingEnrollment>{
        var url = this.baseUrl + "enroll/" + training.id;
        return this.http.post<TrainingEnrollment>(this.location.prepareExternalUrl(url), training)
          .pipe(
              retry(3),
              catchError(this.handleError('enroll', <TrainingEnrollment>{}))
          );
      }
      unenroll( training:Training ):Observable<Training>{
        var url = this.baseUrl + "unenroll/" + training.id;
        return this.http.post<Training>(this.location.prepareExternalUrl(url), training)
          .pipe(
              catchError(this.handleError('unenroll', <Training>{}))
          );
      }
      delete(id:number):Observable<{}>{
        var url = this.baseUrl + 'deletetraining/' + id;
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
      updateSessions(id:number, training:Training):Observable<Training>{
        var url = this.baseUrl + 'updatesessionstraining/' + id;
        return this.http.put<Training>(this.location.prepareExternalUrl(url), training)
                .pipe(
                    catchError(this.handleError('update', training))
                );
      }  
      private addParams(params:{}){
        let searchParams = {};
        for(let p in params){
            searchParams[p] = params[p];
        }
        return  {params: searchParams};
    }

}









import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { PlanningUnit } from '../user/user.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { ExtensionEvent } from '../events/extension-event';


@Injectable()
export class MeetingService {

    private baseUrl = '/api/Meetings/';
    private handleError: HandleError;

    constructor( 
        private http:HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('Meeting');
        }



        getCustom(searchParams?:{}):Observable<MeetingWithTime[]>{
            var url = this.baseUrl + "GetCustom/";
            return this.http.get<MeetingWithTime[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
                .pipe(
                    catchError(this.handleError('getCustom', []))
                );
          }

    /*****************************/
    // CRUD operations
    /*****************************/

    add( training:MeetingWithTime ):Observable<Meeting>{
        return this.http.post<Meeting>(this.location.prepareExternalUrl(this.baseUrl + "addmeeting"), training)
            .pipe(
                catchError(this.handleError('add', <Meeting>{}))
            );
      }

    update(id:number, unit:MeetingWithTime):Observable<Meeting>{
        var url = this.baseUrl + "updatemeeting/" + id;
        return this.http.put<Meeting>(this.location.prepareExternalUrl(url), unit)
            .pipe(
                catchError(this.handleError('update', <Meeting>{}))
            );
    }
    delete(id:number):Observable<{}>{
        var url = this.baseUrl + "deletemeeting/" + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
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


export class Meeting extends ExtensionEvent{
}

export class MeetingWithTime extends Meeting{
    starttime:string;
    endtime:string;
    etimezone:boolean;
}
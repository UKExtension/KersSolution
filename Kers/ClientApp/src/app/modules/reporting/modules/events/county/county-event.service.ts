import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { ExtensionEvent } from '../extension-event';
import { User, PlanningUnit } from '../../user/user.service';
import { ProgramCategory } from '../../admin/programs/programs.service';


@Injectable({
  providedIn: 'root'
})
export class CountyEventService {

  private baseUrl = '/api/CountyEvent/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('CountyEventService');
      }

      add( event:CountyEvent ):Observable<CountyEvent>{
        return this.http.post<CountyEvent>(this.location.prepareExternalUrl(this.baseUrl + 'addcountyevent/'), event)
            .pipe(
                catchError(this.handleError('add', <CountyEvent>{}))
            );
      }

      delete(id:number):Observable<{}>{
        var url = this.baseUrl + 'deletecountyevent/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
      }

      update(id:number, event:CountyEvent):Observable<CountyEvent>{
        var url = this.baseUrl + 'updatecountyevent/' + id;
        return this.http.put<CountyEvent>(this.location.prepareExternalUrl(url), event)
                .pipe(
                    catchError(this.handleError('update', event))
                );
      }

/* 
      current():Observable<User>{
        if(this.usr == null){
            var url = this.baseUrl + "current";
            return this.http.get<User>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(
                        res =>
                        {
                            this.usr = <User>resevent
                        }
                    ),
                    catchError(this.handleError('current', <User>{}))
                );
                    
        }else{
            return of(this.usr);
        }
    }



    getCustom(searchParams?:{}) : Observable<User[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<User[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}):Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }

 */

}

export interface CountyEvent extends ExtensionEvent{
    classicCountyEventId:number;
    rDT:Date;
    units:CountyEventPlanningUnit[];
    programCategories:CountyEventProgramCategory[];
}

export interface CountyEventPlanningUnit{
    id:number;
    planningUnit:PlanningUnit;
    planningUnitId:number;
    isHost:boolean;
}
export interface CountyEventProgramCategory{
    id:number;
    programCategory:ProgramCategory;
    programCategoryId:number;
}







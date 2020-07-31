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

      add( event:CountyEventWithTime ):Observable<CountyEventWithTime>{
        return this.http.post<CountyEventWithTime>(this.location.prepareExternalUrl(this.baseUrl + 'addcountyevent/'), event)
            .pipe(
                catchError(this.handleError('add', <CountyEventWithTime>{}))
            );
      }

      delete(id:number):Observable<{}>{
        var url = this.baseUrl + 'deletecountyevent/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
      }

      update(id:number, event:CountyEventWithTime):Observable<CountyEventWithTime>{
        var url = this.baseUrl + 'updatecountyevent/' + id;
        return this.http.put<CountyEventWithTime>(this.location.prepareExternalUrl(url), event)
                .pipe(
                    catchError(this.handleError('update', event))
                );
      }
      range():Observable<CountyEventWithTime[]>{
          var url = this.baseUrl + 'range';
          return this.http.get<CountyEventWithTime[]>(url)
                .pipe(
                    catchError( this.handleError('range', []))
                );
      }

      getCustom(searchParams?:{}):Observable<CountyEventWithTime[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<CountyEventWithTime[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
      }
    
      getLegacyCountyEvents(amount:number = 20): Observable<Object[]>{
        var url = this.baseUrl + "getlegacy/" + amount ;
        return this.http.get<Object[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getLegacyCountyEvents', []))
            );
      }

      migrate( id:number ):Observable<CountyEvent>{
        var url = this.baseUrl + "migrate/" + id;
        return this.http.get<CountyEvent>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('migrate', <CountyEvent>{}))
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

export interface CountyEvent extends ExtensionEvent{
    classicCountyEventId:number;
    rDT:Date;
    units:CountyEventPlanningUnit[];
    programCategories:CountyEventProgramCategory[];
}

export interface CountyEventWithTime extends CountyEvent{
    starttime:string;
    endtime:string;
    etimezone:boolean;
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


export class CountyEventSearchCriteria{
    start: string;
    end: string;
    search: string = "";
    day?: number;
    order: string = 'dsc';
    countyId?:number = 0
  }






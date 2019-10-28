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

  private baseUrl = '/api/ExtensionEvent/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('CountyEventService');
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
                            this.usr = <User>res
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
}
export interface CountyEventProgramCategory{
    id:number;
    programCategory:ProgramCategory;
    programCategoryId:number;
}







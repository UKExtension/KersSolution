import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { PlanningUnit } from '../user/user.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class PlanningunitService {

    private baseUrl = '/api/County/';
    private planningUnits = new Map<number, PlanningUnit>();
    private handleError: HandleError;

    constructor( 
        private http:HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('PlanningunitService');
        }



    counties(districtId:number | null = null):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'countylist' + (districtId == null ? '' : '/' + districtId);
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('counties', []))
            );
    }

    timezones():Observable<Object[]>{
        var url = this.baseUrl + 'timezones';
        return this.http.get<Object[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('timezones', []))
            );
    }

    id(id:number):Observable<PlanningUnit>{
        if(this.planningUnits.has(id)){
            return of( this.planningUnits.get(id));
        }else{
            var url = this.baseUrl + id;
            return this.http.get<PlanningUnit>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(
                        res =>
                        {
                            var unit = res;
                            this.planningUnits.set(id, unit);
                        }
                    ),
                    catchError(this.handleError('id', <PlanningUnit>{}))
                );
        }
    }

    /*****************************/
    // CRUD operations
    /*****************************/

    update(id:number, unit:PlanningUnit):Observable<PlanningUnit>{
        var url = this.baseUrl + id;
        return this.http.put<PlanningUnit>(this.location.prepareExternalUrl(url), unit)
            .pipe(
                catchError(this.handleError('update', <PlanningUnit>{}))
            );
    }


}

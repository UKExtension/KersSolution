import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { PlanningUnit } from "../user/user.service";
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';




@Injectable()
export class CountyService {

    private baseUrl = '/api/county/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ContactService');
        }

    get(county:number = 0):Observable<PlanningUnit>{
        var url = this.baseUrl + county;
        return this.http.get<PlanningUnit>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perPeriod', <PlanningUnit>{}))
            );
    }

    geoCenter(coordinates:[[number, number]]){
        var sumFirst = 0;
        var sumSecond = 0;
        var amount = 0;
        for(var crd of coordinates){
            sumFirst += crd[0];
            sumSecond += crd[1];
            amount++;
        }
        if(amount == 0){
            amount = 1;
        }
        return [sumFirst/amount, sumSecond/amount];
    }
 
    
}

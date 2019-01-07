import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';


@Injectable()
export class FiscalyearService {

    private baseUrl = '/api/FiscalYear/';
    private handleError: HandleError;

    private years:FiscalYear[] = null;
    private currentServiceLogFiscalYear:FiscalYear | null = null;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('FiscalyearService');
        }

    listFiscalYears():Observable<FiscalYear[]>{
            var url = this.baseUrl + "All";
            if(this.years == null){
                return this.http.get<FiscalYear[]>(this.location.prepareExternalUrl(url))
                    .pipe(
                        tap(yrs => this.years = yrs),
                        catchError(this.handleError('listFiscalYears', []))
                    );
            }else{
                return of(this.years);
            } 
            
    }
    byType(type:string = "serviceLog"):Observable<FiscalYear[]>{
        var url = this.baseUrl + "bytype/" + type;
        return this.http.get<FiscalYear[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byType', []))
            );
    }
    forDate(date:Date, type:string = "serviceLog"):Observable<FiscalYear>{
        var url = this.baseUrl + "forDate/" + date.toISOString() + "/" + type;
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                map(res => {
                    var year = res;
                    year.start = new Date(year.start);
                    year.end = new Date(year.end);
                    if(year.availableAt != null){
                        year.availableAt = new Date(year.availableAt);
                    }
                    if(year.extendedTo != null){
                        year.extendedTo = new Date(year.extendedTo);
                    }
                    return year;
                }),
                catchError(this.handleError('forDate', <FiscalYear>{}))
            );
            
    }

    byId(id:number):Observable<FiscalYear>{
        var url = this.baseUrl + id;
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byId', <FiscalYear>{}))
            );
    }
    byName(id:string):Observable<FiscalYear>{
        var url = this.baseUrl + id;
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byName', <FiscalYear>{}))
            );
    }

    current(type:string = "serviceLog"):Observable<FiscalYear>{
        var url = this.baseUrl + "current/" + type;
        if(type == "serviceLog" && this.currentServiceLogFiscalYear != null){
            return of(this.currentServiceLogFiscalYear);
        }
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                tap(yr =>
                    {
                        if(type == "serviceLog") this.currentServiceLogFiscalYear = yr;
                    }
                ),
                catchError(this.handleError('current', <FiscalYear>{}))
            );
          
    }
    next(type:string = "serviceLog"):Observable<FiscalYear>{
        var url = this.baseUrl + "next/" + type;
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('next', <FiscalYear>{}))
            );
    }

    previous(type:string = "serviceLog"):Observable<FiscalYear>{
        var url = this.baseUrl + "previous/" + type;
        return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('previous', <FiscalYear>{}))
            );
    }

    addFiscalYear(fiscalyear):Observable<FiscalYear>{
        return this.http.post<FiscalYear>(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(fiscalyear))
            .pipe(
                catchError(this.handleError('addFiscalYear', <FiscalYear>{}))
            );
    }
    
    updateFiscalYear(id: number, fiscalyear:FiscalYear):Observable<FiscalYear>{
        var url = this.baseUrl + id;
        return this.http.put<FiscalYear>(this.location.prepareExternalUrl(url), fiscalyear)
            .pipe(
                catchError(this.handleError('addFiscalYear', fiscalyear))
            );
    }

    deleteFiscalYear(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteFiscalYear'))
            );
    }
    
}

export class FiscalYear{
    constructor(
        public id: number,
        public start: Date,
        public end: Date,
        public availableAt: Date,
        public extendedTo: Date,
        public type: string,
        public name: string
    ){}
}
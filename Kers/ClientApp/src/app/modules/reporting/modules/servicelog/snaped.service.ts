import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import {Servicelog} from './servicelog.service';

@Injectable()
export class SnapedService {

    private baseUrl = '/api/Snaped/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('SnapedService');
        }

    latest(skip:number = 0, take:number = 5):Observable<Servicelog[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Servicelog[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latest', []))
            );
    }

    reportedhours(id:number = 0):Observable<number>{
        var url = this.baseUrl + 'reportedHours/' + id;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('reportedhours', 0))
            );
    }

    committedhours(id:number = 0):Observable<number>{
        var url = this.baseUrl + 'committed/' + id;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('committedhours', 0))
            );
    }

    statsPerIndividual(userId:number, fy:string = "0"):Observable<{}>{
        var url = this.baseUrl + 'userstats/' + userId + '/' + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('statsPerIndividual'))
            );
    }

    commitmentPerIndividual(userId:number, fy:string = "0"):Observable<{}>{
        var url = this.baseUrl + 'commitments/' + userId + '/' + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('commitmentPerIndividual'))
            );
    }

    reportedhoursCounty(id:number):Observable<number>{
        var url = this.baseUrl + 'reportedhourscounty';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('reportedhoursCounty', 0))
            );
    }

    committedhoursCounty(id:number = 0):Observable<number>{
        var url = this.baseUrl + 'committedhourscounty/' + id;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('committedhoursCounty', 0))
                );
    }

    statsPerCounty(countyId:number):Observable<{}>{
        var url = this.baseUrl + 'reportedcounty/' + countyId;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('statsPerCounty'))
            );
    }

    commitmentPerCounty(countyId:number):Observable<{}>{
        var url = this.baseUrl + 'committedcounty/' + countyId;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('commitmentPerCounty'))
            );
    }

    comitmentActivityTypes(fiscalyearid:number = 0){
        var url = this.baseUrl + 'activitytypes' + '/' + fiscalyearid;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('comitmentActivityTypes'))
            );
    }

    comitmentProjectTypes(fiscalyearid:number = 0){
        var url = this.baseUrl + 'projecttypes' + '/' + fiscalyearid;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('comitmentProjectTypes'))
            );
    }

    comitmentReinforcementItems(fiscalyearid:number = 0){
        var url = this.baseUrl + 'reinforcementitems' + '/' + fiscalyearid;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('comitmentReinforcementItems'))
            );
    }

    copies():Observable<number>{
        var url = this.baseUrl + 'copies';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('copies', 0))
            );
    }

    reach():Observable<number>{
        var url = this.baseUrl + 'reach';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('reach', 0))
            );
    }

    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('num', 0))
        );
    }
}


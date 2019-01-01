import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import { PlanningUnit } from '../user/user.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class IndicatorsService {

    private baseUrl = '/api/ProgramIndicator/';
    private handleError: HandleError;


    constructor( 
        private http:HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('IndicatorsService');
        }


    countiesWithoutIndicators(districtId:number = 0, fy:string = "0"):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'noindicatorscounties/' + districtId + '/' + fy;
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesWithoutIndicators', []))
            );
    }

    listIndicators(program:MajorProgram):Observable<Indicator[]>{
            var url = this.baseUrl + "indicatorsforprogram/" + program.id;
            return this.http.get<Indicator[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('listIndicators', []))
                );
    }

    indicatorValues(program:MajorProgram):Observable<IndicatorValue[]>{
            var url = this.baseUrl + "indicatorvalues/" + program.id;
            return this.http.get<IndicatorValue[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('indicatorValues', []))
                );
    }

    updateValues(program:MajorProgram, vals:IndicatorValue[]):Observable<IndicatorValue[]>{
        var url = this.baseUrl + "valuesupdate/" + program.id;
        return this.http.put<IndicatorValue[]>(this.location.prepareExternalUrl(url), vals)
            .pipe(
                catchError(this.handleError('indicatorValues', vals))
            );

    }

    addIndicator(majorProgramId:number, indicator:Indicator):Observable<Indicator>{
        var url = this.baseUrl + majorProgramId;
        return this.http.post<Indicator>(this.location.prepareExternalUrl(url), indicator)
            .pipe(
                catchError(this.handleError('addIndicator', indicator))
            );
    }

    updateIndicator(indicatorId:number, indicator:Indicator):Observable<Indicator>{
        var url = this.baseUrl + indicatorId;
        return this.http.put<Indicator>(this.location.prepareExternalUrl(url), indicator)
            .pipe(
                catchError(this.handleError('updateIndicator', indicator))
            );
    }

    deleteIndicator(indicatorId:number): Observable<{}>{
        var url = this.baseUrl + indicatorId;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteIndicator'))
            );
    }

}

export interface Indicator{
    id:number;
    question:string;
    order:number;
}

export interface IndicatorValue{
    programIndicatorId:number;
    value:number;
}


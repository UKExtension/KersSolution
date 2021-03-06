import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User, PlanningUnit } from "../user/user.service";
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { ExtensionArea } from '../state/state.service';


@Injectable({
    providedIn: 'root'
  })
export class AreaService {

    private baseUrl = '/api/ExtensionArea/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('AreaService');
        }

    /**********************************/
    // Area CONTENT
    /**********************************/

    get(areaId:number):Observable<ExtensionArea>{
        var url = this.baseUrl + areaId;
        return this.http.get<ExtensionArea>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('get', <ExtensionArea>{}))
            );
    }
    pairing(areaId:number):Observable<string[]>{
        var url = this.baseUrl + 'pairing/' + areaId;
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('pairning', []))
            );
    }

    counties(areaId:number, includePairings:boolean = true):Observable<PlanningUnit[]>{
        var url = this.baseUrl + "countiesbyareaid/" + areaId + "/" + includePairings;
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('counties', []))
            );
    }

    employeeactivity(areaid:number, month:number, year:number, order:string = "asc", type:string = "activity", skip:number=0, take:number=21):Observable<EmployeeNumActivities[]>{
        var url = this.baseUrl + "employeeactivity/" + areaid + '/' + month + '/' + year + '/' + order + '/' + type + '/' + skip + '/' + take + "/area/true";
        return this.http.get<EmployeeNumActivities[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('employeeactivity', []))
            );
    }

    mycounties():Observable<County[]>{
        var url = this.baseUrl + "mycounties";
        return this.http.get<County[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('mycounties', []))
            );
    }
    //Counties without Affirmative Action Plan
    countiesNoAa(district:number):Observable<County[]>{
        var url = this.baseUrl + "countiesnoaa/" + district;
        return this.http.get<County[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesNoAa', []))
            );
    }
    
    // Counties without Plans of Work
    countiesNoPl(district:number):Observable<County[]>{
        var url = this.baseUrl + "countiesnopl/" + district;
        return this.http.get<County[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesNoAa', []))
            );
    }
    
}

export class County{
    constructor(
        public id: number,
        public name: string,
        public areaName: string,
        public order: number,
        public description: string
    ){}
}

export interface EmployeeNumActivities{
    user:User;
    numActivities: number;
}

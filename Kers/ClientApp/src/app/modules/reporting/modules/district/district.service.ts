import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User, PlanningUnit } from "../user/user.service";
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class DistrictService {

    private baseUrl = '/api/district/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('DistrictService');
        }

    /**********************************/
    // DISTRICT CONTENT
    /**********************************/

    get(district:number):Observable<District>{
        var url = this.baseUrl + district;
        return this.http.get<District>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('get', <District>{}))
            );
    }

    counties(district:number):Observable<PlanningUnit[]>{
        var url = this.baseUrl + "counties/" + district;
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('counties', []))
            );
    }

    employeeactivity(districtid:number, month:number, year:number, order:string = "asc", type:string = "activity", skip:number=0, take:number=21, filter:string = "district", includePairings:boolean = true):Observable<EmployeeNumActivities[]>{
        var url = this.baseUrl + "employeeactivity/" + districtid + '/' + month + '/' + year + '/' + order + '/' + type + '/' + skip + '/' + take + '/' + filter + '/' + includePairings;
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

export interface District{
    id:number;
    name: string;
    areaName: string;
    description: string;
    admin: User;
    assistant: User;
}

export interface EmployeeNumActivities{
    user:User;
    numActivities: number;
}

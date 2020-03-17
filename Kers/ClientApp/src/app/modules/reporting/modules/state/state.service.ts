import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { PlanningUnit } from "../user/user.service";
import { District } from "../district/district.service";




@Injectable({
    providedIn: 'root'
  })
export class StateService {

    private baseUrl = '/api/state/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('StateService');
        }


    /**********************************/
    // STATE CONTENT
    /**********************************/


    districts():Observable<District[]>{
        var url = this.baseUrl + "districts";
        return this.http.get<District[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('districts', []))
            );
    }

    counties():Observable<PlanningUnit[]>{
        var url = this.baseUrl + "counties";
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('counties', []))
            );
    }

    congressional():Observable<CongressionalDistrict[]>{
        var url = this.baseUrl + "congressional";
        return this.http.get<CongressionalDistrict[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('districts', []))
            );
    }

    notCounties():Observable<PlanningUnit[]>{
        var url = this.baseUrl + "notcounties";
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('notCounties', []))
            );
    }

    addGeoFeature( planningUnitId:number, feature:Object ){
        var url = this.baseUrl + "addGeoFearure/" + planningUnitId;
        return this.http.post(this.location.prepareExternalUrl(url), feature)
            .pipe(
                catchError(this.handleError('addGeoFeature', feature))
            );
    }

    
    
    kyMap(){
        var url = '/assets/json/kentucky-counties.json';
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('counties'))
            );
    }


    kyPopulationByCounty(){
        var url = this.baseUrl + 'populationByCounty';
        return this.http.get(url)
            .pipe(
                catchError(this.handleError('kyPopulationByCounty'))
            );
    }

}

export interface CongressionalDistrict{
    id:number;
    order?:number;
    name:string;
    units:CongressionalDistrictUnit[];
}
export interface CongressionalDistrictUnit{
    id:number;
    order?:number;
    PlanningUnit:PlanningUnit;
    PlanningUnitId:number;
    IsMultiDistrict:boolean;
}



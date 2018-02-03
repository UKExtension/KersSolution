import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import { PlanningUnit } from "../user/user.service";
import { District } from "../district/district.service";




@Injectable()
export class StateService {

    private baseUrl = '/api/state/';


    constructor( private http:AuthHttp, private location:Location){}


    /**********************************/
    // STATE CONTENT
    /**********************************/


    districts(){
        var url = this.baseUrl + "districts";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <District[]>res.json())
                .catch(this.handleError);
    }

    counties(){
        var url = this.baseUrl + "counties";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <PlanningUnit[]>res.json())
                .catch(this.handleError);
    }

    addGeoFeature( planningUnitId:number, feature:Object ){
        var url = this.baseUrl + "addGeoFearure/" + planningUnitId;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(feature), this.getRequestOptions())
                .map(res => res.json())
                .catch(this.handleError);
    }

    
    
    kyMap(){
        var url = '/assets/json/kentucky-counties.json';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }


    kyPopulationByCounty(){
        var url = this.baseUrl + 'populationByCounty';
        return this.http.get(url)
                .map(res => res.json())
                .catch(this.handleError);
    }


    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
    }

    getRequestOptions(){
        return new RequestOptions(
            {
                headers: new Headers({
                    "Content-Type": "application/json; charset=utf-8"
                })
            }
        )
    }
    
}

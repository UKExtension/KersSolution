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
export class CountyService {

    private baseUrl = '/api/county/';


    constructor( private http:AuthHttp, private location:Location){}

    get(county:number = 0){
        var url = this.baseUrl + county;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>  <PlanningUnit>res.json() )
                .catch(this.handleError);
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

import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import { User } from "../user/user.service";


@Injectable()
export class DistrictService {

    private baseUrl = '/api/district/';


    constructor( private http:AuthHttp, private location:Location){}


    /**********************************/
    // DISTRICT CONTENT
    /**********************************/

    get(district:number){
        var url = this.baseUrl + district;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>  <District>res.json() )
                .catch(this.handleError);
    }

    counties(district:number){
        var url = this.baseUrl + "counties/" + district;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <County[]>res.json())
                .catch(this.handleError);
    }

    mycounties(){
        var url = this.baseUrl + "mycounties";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <County[]>res.json())
                .catch(this.handleError);
    }
    //Counties without Affirmative Action Plan
    countiesNoAa(district:number){
        var url = this.baseUrl + "countiesnoaa/" + district;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <County[]>res.json())
                .catch(this.handleError);
    }
    
    // Counties without Plans of Work
    countiesNoPl(district:number){
        var url = this.baseUrl + "countiesnopl/" + district;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <County[]>res.json())
                .catch(this.handleError);
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
    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
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

import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {Servicelog} from './servicelog.service';

@Injectable()
export class SnapedService {

    private baseUrl = '/api/Snaped/';




    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}





    latest(skip:number = 0, take:number = 5){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Servicelog[]>res.json() )
                .catch(this.handleError);
    }

    reportedhours(id:number = 0){
        var url = this.baseUrl + 'reportedHours/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    committedhours(id:number = 0){
        var url = this.baseUrl + 'committed/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    statsPerIndividual(userId:number){
        var url = this.baseUrl + 'userstats/' + userId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }

    commitmentPerIndividual(userId:number){
        var url = this.baseUrl + 'commitments/' + userId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }




    reportedhoursCounty(id:number){
        var url = this.baseUrl + 'reportedhourscounty';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    committedhoursCounty(id:number = 0){
        var url = this.baseUrl + 'committedhourscounty/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    statsPerCounty(countyId:number){
        var url = this.baseUrl + 'reportedcounty/' + countyId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }

    commitmentPerCounty(countyId:number){
        var url = this.baseUrl + 'committedcounty/' + countyId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }







    comitmentProjectTypes(){
        var url = this.baseUrl + 'projecttypes';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }

    copies(){
        var url = this.baseUrl + 'copies';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
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


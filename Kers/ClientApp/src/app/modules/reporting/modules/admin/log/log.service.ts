import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';
import {KersUser} from '../users/users.service';


@Injectable()
export class LogService {

    private baseUrl = '/api/log/';
    private loaded;

    constructor( private http:AuthHttp, private location:Location){}

    latest(skip:number = 0, take:number = 10):Observable<Log[]>{
            var url = this.baseUrl + skip + "/" + take;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.loaded = <Log[]>res.json())
                .catch(this.handleError);
    }



    getCustom(searchParams?:{}) : Observable<Log[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }

    getCustomCount(searchParams?:{}){
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }

    types() : Observable<string[]>{
            var url = this.baseUrl + "types";
            return this.http.get(this.location.prepareExternalUrl(url))
            .map(response => response.json())
            .catch(this.handleError);

    }

    loadMore(take:number = 10){
        var url = this.baseUrl + this.loaded.length + "/" + take;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    var more = <Log>res.json();
                    this.loaded.concat(more);
                    return this.loaded;
                    })
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

export class Log{
    constructor(
        public id: number,
        public level: string,
        public time: Date,
        public type: string,
        public description: string,
        public objectType: string,
        public object: string,
        public ip: string,
        public agent: string,
        public user: KersUser,
    ){}
}
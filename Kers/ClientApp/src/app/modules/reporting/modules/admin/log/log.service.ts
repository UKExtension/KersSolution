import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import {KersUser} from '../users/users.service';


@Injectable()
export class LogService {

    private baseUrl = '/api/log/';
    private handleError: HandleError;
    private loaded:Log[] = [];

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('LogService');
        }

    latest(skip:number = 0, take:number = 10):Observable<Log[]>{
            var url = this.baseUrl + skip + "/" + take;
            return this.http.get<Log[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('latest', []))
                );
    }



    getCustom(searchParams?:{}) : Observable<Log[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<Log[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}):Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }

    types() : Observable<string[]>{
            var url = this.baseUrl + "types";
            return this.http.get<string[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('types', []))
                );

    }

    loadMore(take:number = 10):Observable<Log[]>{
        var url = this.baseUrl + this.loaded.length + "/" + take;
            return this.http.get<Log[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    map(res => {
                            var more = res;
                            this.loaded.concat(more);
                            return this.loaded;
                        }),
                    catchError(this.handleError('loadMore', []))
                );  
    }

    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('num', 0))
            );
    }

    add(log:Log){
        return this.http.post<Log>(this.location.prepareExternalUrl(this.baseUrl), log)
            .pipe(
                catchError(this.handleError('add', log))
            );
    }

    
    private addParams(params:{}){
        let searchParams = {};
        for(let p in params){
            searchParams[p] = params[p];
        }
        return  {params: searchParams};
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
        public userId?: number
    ){}
}
import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';


@Injectable()
export class FiscalyearService {

    private baseUrl = '/api/FiscalYear/';

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private years = null;
    private currentServiceLogFiscalYear:FiscalYear | null = null;

    constructor( private http:AuthHttp, private location:Location){}

    listFiscalYears(){
            var url = this.baseUrl + "All";
            if(this.years != null) return Observable.of(this.years);
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.years = res.json())
                .catch(this.handleError);
    }
    byType(type:string = "serviceLog"):Observable<FiscalYear[]>{
        var url = this.baseUrl + "bytype/" + type;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => this.years = <FiscalYear[]>res.json())
            .catch(this.handleError);
    }
    forDate(date:Date, type:string = "serviceLog"):Observable<FiscalYear>{
        var url = this.baseUrl + "forDate/" + date.toISOString() + "/" + type;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var year = <FiscalYear>res.json();
                year.start = new Date(year.start);
                year.end = new Date(year.end);
                if(year.availableAt != null){
                    year.availableAt = new Date(year.availableAt);
                }
                if(year.extendedTo != null){
                    year.extendedTo = new Date(year.extendedTo);
                }
                return year;
                
            })
            .catch(this.handleError);
    }

    byId(id:number):Observable<FiscalYear>{
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <FiscalYear>res.json())
            .catch(this.handleError);
    }
    byName(id:string):Observable<FiscalYear>{
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <FiscalYear>res.json())
            .catch(this.handleError);
    }

    current(type:string = "serviceLog"){
        var url = this.baseUrl + "current/" + type;
        if(type == "serviceLog" && this.currentServiceLogFiscalYear != null){
            return Observable.of(this.currentServiceLogFiscalYear);
        }
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var fy = <FiscalYear>res.json();
                if(type == "serviceLog") this.currentServiceLogFiscalYear = fy;
                return fy;
            })
            .catch(this.handleError);
    }
    next(type:string = "serviceLog"){
        var url = this.baseUrl + "next/" + type;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <FiscalYear>res.json())
            .catch(this.handleError);
    }

    previous(type:string = "serviceLog"){
        var url = this.baseUrl + "previous/" + type;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <FiscalYear>res.json())
            .catch(this.handleError);
    }

    addFiscalYear(fiscalyear){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(fiscalyear), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
    }
    
    updateFiscalYear(id: number, fiscalyear:FiscalYear){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(fiscalyear), this.getRequestOptions())
                    .map( res => {
                        return <FiscalYear> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteFiscalYear(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
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

export class FiscalYear{
    constructor(
        public id: number,
        public start: Date,
        public end: Date,
        public availableAt: Date,
        public extendedTo: Date,
        public type: string,
        public name: string
    ){}
}
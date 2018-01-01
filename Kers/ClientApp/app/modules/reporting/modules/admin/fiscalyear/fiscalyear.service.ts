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

    constructor( private http:AuthHttp, private location:Location){}

    listFiscalYears(){
            var url = this.baseUrl + "All";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.years = res.json())
                .catch(this.handleError);
    }

    current(type:string = "serviceLog"){
        var url = this.baseUrl + "current/" + type;
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
import { Injectable} from '@angular/core';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import {Location} from '@angular/common';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';


@Injectable()
export class ReportingHelpService {

    private baseUrl = '/api/HelpContent/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
    ){ }

    

    get(id:number) : Observable<Help>{
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(response => <Help> response.json())
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

export class Help{
    constructor(
        public id: number,
        public title: string,
        public body: string,
        public categoryId: number
    ){}
}
import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';


@Injectable()
export class EmailService {

    private baseUrl = '/api/Email/';

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private years = null;

    constructor( private http:AuthHttp, private location:Location){}




    send(email:Email){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(email), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    /**********************************/
    // HELP CONTENT
    /**********************************/
/*

    all(){
        var url = this.baseUrl + "All";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Help[]>res.json())
                .catch(this.handleError);
    }


    addHelp(help:Help){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(help), this.getRequestOptions())
                    .map( res => {
                        return <Help> res.json();
                    })
                    .catch(this.handleError);
    }
    updateHelp(id:number, help:Help){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(help), this.getRequestOptions())
                    .map( res => {
                        return <Help> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteHelp(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }
*/

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

export interface Email{
    pressets: number;
    server: string;
    port: number;
    username: string;
    password: string;
    from: string;
    to: string;
    subject: string;
    body: string;
}

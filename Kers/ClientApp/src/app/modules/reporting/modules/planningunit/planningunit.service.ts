import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import { PlanningUnit } from '../user/user.service';


@Injectable()
export class PlanningunitService {

    private baseUrl = '/api/County/';



    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}



    counties():Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'countylist';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <PlanningUnit[]>res.json())
                .catch(this.handleError);
    }

    id(id:number):Observable<PlanningUnit>{
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <PlanningUnit>res.json())
                .catch(this.handleError);
    }

    /*****************************/
    // CRUD operations
    /*****************************/

    /*
    add( activity:Servicelog ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => {
                    
                        var ret = <Servicelog>res.json();
                        return ret;
                    } )
                    .catch(this.handleError);
    }
    update(id:number, activity:Servicelog){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => {
                        return <Servicelog> res.json();
                    })
                    .catch(this.handleError);
    }

    delete(id:number){
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

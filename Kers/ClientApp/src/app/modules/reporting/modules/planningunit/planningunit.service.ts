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
                .map(res =>{ 
                    let counties = <PlanningUnit[]>res.json();
                    return counties;
                } )
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

    update(id:number, unit:PlanningUnit){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(unit), this.getRequestOptions())
                    .map( res => {
                        return <PlanningUnit> res.json();
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

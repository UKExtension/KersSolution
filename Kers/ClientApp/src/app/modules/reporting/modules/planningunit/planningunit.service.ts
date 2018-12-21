import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Response, Headers, RequestOptions } from '@angular/http';
import {of, Observable} from 'rxjs';
import {AuthHttp} from '../../../authentication/auth.http';
import { PlanningUnit } from '../user/user.service';


@Injectable()
export class PlanningunitService {

    private baseUrl = '/api/County/';
    private planningUnits = new Map<number, PlanningUnit>();

    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}



    counties(districtId:number | null = null):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'countylist' + (districtId == null ? '' : '/' + districtId);
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    let counties = <PlanningUnit[]>res.json();
                    return counties;
                } )
                .catch(this.handleError);
    }

    id(id:number):Observable<PlanningUnit>{
        if(this.planningUnits.has(id)){
            return of( this.planningUnits.get(id));
        }else{
            var url = this.baseUrl + id;
            return this.http.get(this.location.prepareExternalUrl(url))
                    .map(res => {
                            var unit = <PlanningUnit>res.json();
                            this.planningUnits.set(id, unit);
                            return unit;
                        }
                    )
                    .catch(this.handleError);
        }
            
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

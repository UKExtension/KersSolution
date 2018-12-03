import { Injectable } from '@angular/core';
import { PlanningUnit, User } from '../../user/user.service';
import { AuthHttp } from '../../../../authentication/auth.http';
import {Location} from '@angular/common';
import { RequestOptions, Response, Headers } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class VehicleService {

  private baseUrl = '/api/county/vehicle/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}
  

  add(vehicle:Vehicle){
    return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(vehicle), this.getRequestOptions())
      .map( res => <Vehicle>res.json() )
      .catch(this.handleError);
  }

  update(id:number, vehicle:Vehicle){
    var url = this.baseUrl + id;
    return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(vehicle), this.getRequestOptions())
      .map( res => {
          return <Vehicle> res.json();
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


export interface Vehicle{
  id:number,
  planningUnitId:number,
  planningUnit:PlanningUnit,
  addedById:number,
  addedBy:User,
  make:string,
  model:string,
  year:string,
  licenseTag:string,
  odometer:number,
  color:string,
  enabled:boolean,
  comments:string,
  uploadImageId: number,
  datePurchased?:Date,
  dateDispossed?:Date
}
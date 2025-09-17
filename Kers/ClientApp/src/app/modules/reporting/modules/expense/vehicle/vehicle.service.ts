import { Injectable } from '@angular/core';
import { PlanningUnit, User } from '../../user/user.service';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { Expense } from '../expense.service';
import { MileageBundle } from '../../mileage/mileage';

@Injectable()
export class VehicleService {

  private baseUrl = '/api/county/vehicle/';
  private handleError: HandleError;


  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('VehicleService');
      }
  

  add(vehicle:Vehicle):Observable<Vehicle>{
    return this.http.post<Vehicle>(this.location.prepareExternalUrl(this.baseUrl), vehicle)
      .pipe(
        catchError(this.handleError('add', <Vehicle>{}))
    );
  }

  update(id:number, vehicle:Vehicle):Observable<Vehicle>{
    var url = this.baseUrl + id;
    return this.http.put<Vehicle>(this.location.prepareExternalUrl(url), vehicle)
      .pipe(
        catchError(this.handleError('update', vehicle))
    );
  }

  trips(vehicle:Vehicle, searchParams?:{}):Observable<MileageBundle[]>{
    return this.http.get<MileageBundle[]>(this.location.prepareExternalUrl("/api/county/vehicletrips/"+vehicle.id), this.addParams(searchParams))
      .pipe(
        catchError(this.handleError('trips', []))
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


export interface Vehicle{
  id:number,
  planningUnitId:number,
  planningUnit:PlanningUnit,
  addedById:number,
  addedBy:User,
  name:string,
  make:string,
  model:string,
  year:string,
  licenseTag:string,
  purchasePrice:number,
  odometer:number,
  endingOdometer:number,
  color:string,
  enabled:boolean,
  comments:string,
  uploadImageId: number,
  datePurchased?:Date,
  dateDispossed?:Date
}

export class TripsSearchCriteria{
    start: string;
    end: string;
    
  }
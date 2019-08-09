import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';

@Injectable({
  providedIn: 'root'
})
export class SoildataService {

  private baseUrl = '/api/Soildata/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('SoildataService');
      }
 
      addresses(planningUnitId:number = 0):Observable<FarmerAddress[]>{
        var url = this.baseUrl + "addresses/"+planningUnitId;
        return this.http.get<FarmerAddress[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('addresses', []))
            );
      }
      
      addaddress( address:FarmerAddress ):Observable<FarmerAddress>{
        return this.http.post<FarmerAddress>(this.location.prepareExternalUrl(this.baseUrl + 'addaddress/'), address)
            .pipe(
                catchError(this.handleError('add', <FarmerAddress>{}))
            );
      }

      updateaddress(id:number, address:FarmerAddress):Observable<FarmerAddress>{
        var url = this.baseUrl + 'updateaddress/' + id;
        return this.http.put<FarmerAddress>(this.location.prepareExternalUrl(url), address)
                .pipe(
                    catchError(this.handleError('update', address))
                );
      }   

}

export interface CountyCode{
    id:number;
    code:string;
    name:string;
    countyID:number;
    planningUnitId:number;
}

export interface FarmerAddress{
    id:number;
    countyCode:CountyCode;
    first:string;
    mi:string;
    last:string;
    title:string;
    modifier:string;
    company:string;
    address:string;
    city:string;
    st:string;
    status:string;
    workNumber:string;
    duplicateHouseHold:string;
    homeNumber:string;
    fax:string;
    farmerID:string;
    zip:string;
    emailAddress:string;
    latitude:string;
    longitude:string;
    altitude:string;
    farmerData:string;
}

export interface CountyNote{
    id:number;
    countyCode:CountyCode;
    name:string;
    note:string;
}









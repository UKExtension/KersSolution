import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';


@Injectable({
  providedIn: 'root'
})
export class LocationService {

  private baseUrl = '/api/ExtensionEvent/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('UserService');
      }

/* 
      current():Observable<User>{
        if(this.usr == null){
            var url = this.baseUrl + "current";
            return this.http.get<User>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(
                        res =>
                        {
                            this.usr = <User>res
                        }
                    ),
                    catchError(this.handleError('current', <User>{}))
                );
                    
        }else{
            return of(this.usr);
        }
    }



    getCustom(searchParams?:{}) : Observable<User[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<User[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}):Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }

 */

}

export interface ExtensionEventLocation{
    id:number;
    displayName:string;
    locationEmailAddress:string;
    address: PhysicalAddress;
    locationUri:string;


}

export interface PhysicalAddress{

}

export enum ExtensionEventLocationType {
    Default = 0,
    ConferenceRoom = 1,
	HomeAddress = 2,
	BusinessAddress = 3,
	GeoCoordinates = 4,
	StreetAddress = 5,
	Hotel = 6,
	Restaurant = 7,
	LocalBusiness = 8,
	PostalAddress = 9,
    ExtensionOffice = 10,
    Online = 11
}









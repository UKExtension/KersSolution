import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { ExtensionEvent, ExtensionEventLocation } from '../extension-event';
import { User, PlanningUnit } from '../../user/user.service';


@Injectable({
  providedIn: 'root'
})
export class LocationService {

  private baseUrl = '/api/Location/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('LocationService');
      }

      addLocation( location:ExtensionEventLocation ):Observable<ExtensionEventLocation>{
        return this.http.post<ExtensionEventLocation>(this.location.prepareExternalUrl(this.baseUrl + 'addlocation/'), location)
            .pipe(
                catchError(this.handleError('addNote', <ExtensionEventLocation>{}))
            );
      }
      addLocationConnection( location:ExtensionEventLocationConnection ):Observable<ExtensionEventLocationConnection>{
        return this.http.post<ExtensionEventLocationConnection>(this.location.prepareExternalUrl(this.baseUrl + 'addlocationconnection/'), location)
            .pipe(
                catchError(this.handleError('addNote', <ExtensionEventLocationConnection>{}))
            );
      }
      updateLocation(id:number, location:ExtensionEventLocationConnection):Observable<ExtensionEventLocationConnection>{
        var url = this.baseUrl + 'updatelocation/' + id;
        return this.http.put<ExtensionEventLocationConnection>(this.location.prepareExternalUrl(url), location)
                .pipe(
                    catchError(this.handleError('update', location))
                );
      }
      
      // id: ExtensionEventLocationConnection.id
      // Deletes Connection as well as location itself
      deleteLocationConnection(id:number):Observable<{}>{
        var url = this.baseUrl + "deletelocationconnection/" + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete note'))
            );
      }

      locationsByCounty(id:number = 0, skip:number = 0, take:number = 5, order:string = "often", search:string = ""):Observable<ExtensionEventLocationConnectionSearchResult>{
        var url = this.baseUrl + "countylocations/"+id+"/"+skip+"/"+take+"/"+order+"/"+search;
        return this.http.get<ExtensionEventLocationConnectionSearchResult>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('notes by county', <ExtensionEventLocationConnectionSearchResult> {}))
            );
      }

}

export interface ExtensionEventLocationConnection{
    id:number;
    extensionEvent:ExtensionEvent;
    extensionEventId?:number;
    kersUser:User;
    kersUserId?:number;
    planningUnit:PlanningUnit;
    planningUnitId?:number;
    active:boolean;
    extensionEventLocation:ExtensionEventLocation;
    extensionEventLocationId:number;
}

export interface ExtensionEventLocationConnectionSearchResult{
    results: ExtensionEventLocationConnection[];
    count: number;
}

/* 
export interface PhysicalAddress{
    id:number;
    building:string;
    street:string;
    city:string;
    state:string;
    postalCode:string;
}
 */
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









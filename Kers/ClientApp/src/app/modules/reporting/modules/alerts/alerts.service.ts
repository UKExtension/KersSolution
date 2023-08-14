import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Alert, AlertRoute } from './Alert';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private baseUrl = '/api/Alerts/';
  private handleError: HandleError;

  constructor(
    private http: HttpClient, 
    private location:Location,
    httpErrorHandler: HttpErrorHandler
    ) {
        this.handleError = httpErrorHandler.createHandleError('AlertsService');
  }



  routes():Observable<AlertRoute[]>{
    var url = this.baseUrl + 'routes';
    return this.http.get<AlertRoute[]>(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('Alert Routes', []))
        );
  }
  getAlerts(filter:number = 0):Observable<Alert[]>{
    var url = this.baseUrl + 'getAlerts/' + filter;
    return this.http.get<Alert[]>(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('Get Alerts', []))
        );
  }
  getPage(route:string):Observable<Alert[]>{
    var url = this.baseUrl + 'getPageAlerts/' + encodeURIComponent(route);
    return this.http.get<Alert[]>(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('Get Page Alerts', []))
        );
  }

  addAlert( alert:Alert ):Observable<Alert>{
    return this.http.post<Alert>(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(alert))
            .pipe(
                catchError(this.handleError('add', <Alert>{}))
            );
  }

  updateAlert(id:number, alert:Alert):Observable<Alert>{
    var url = this.baseUrl + id;
    return this.http.put<Alert>(this.location.prepareExternalUrl(url), JSON.stringify(alert))
            .pipe(
                catchError(this.handleError('update', alert))
            );
  }
  delete(id:number):Observable<{}>{
    var url = this.baseUrl + id;
    return this.http.delete(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('delete'))
        );
  } 



}

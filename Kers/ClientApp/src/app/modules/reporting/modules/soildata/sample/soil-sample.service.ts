import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TypeForm } from '../soildata.report';

@Injectable({
  providedIn: 'root'
})
export class SoilSampleService {
  private baseUrl = '/api/SoilSample/';
  private handleError: HandleError;

  constructor( 
    private http: HttpClient, 
    private location:Location,
    httpErrorHandler: HttpErrorHandler
    ) {
        this.handleError = httpErrorHandler.createHandleError('SoilSampleService');
    }

    formTypes():Observable<TypeForm[]>{
      var url = this.baseUrl + "forms/";
      return this.http.get<TypeForm[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('type forms', []))
          );
    }
}

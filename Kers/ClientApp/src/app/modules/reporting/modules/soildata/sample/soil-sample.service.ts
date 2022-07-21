import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TypeForm } from '../soildata.report';
import { BillingType, OptionalTest, SampleAttribute, SampleAttributeType } from './SampleInfoBundle';

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


    attributeTypes(typeFormId:number):Observable<SampleAttributeType[]>{
      var url = this.baseUrl + "attributetypes/" + typeFormId;
      return this.http.get<SampleAttributeType[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('sample attribute types', []))
          );
    }

    lastsamplenum(countyCodeId:number = 0):Observable<number>{
      var url = this.baseUrl + "lastsamplenum/" + countyCodeId;
      return this.http.get<number>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('sample attributes', 0))
          );
    }
    attributes(typeAttributeId:number):Observable<SampleAttribute[]>{
      var url = this.baseUrl + "attributes/" + typeAttributeId;
      return this.http.get<SampleAttribute[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('sample attributes', []))
          );
    }
    billingtypes():Observable<BillingType[]>{
      var url = this.baseUrl + "billingtypes";
      return this.http.get<BillingType[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('billing types', []))
          );
    }

    optionaltests():Observable<OptionalTest[]>{
      var url = this.baseUrl + "optionaltests";
      return this.http.get<OptionalTest[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('optionaltests', []))
          );
    }
}

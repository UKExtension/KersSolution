import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { TaxExemptFinancialYear } from './exmpt';


@Injectable({
  providedIn: 'root'
})
export class ExemptService {

  private baseUrl = '/api/TaxExempt/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('Tax Exempt');
      }


      financialYears():Observable<TaxExemptFinancialYear[]>{
        
            var url = this.baseUrl + "financialyears";
            return this.http.get<TaxExemptFinancialYear[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('financialyears', <TaxExemptFinancialYear[]>{}))
                );
                    
        
    }

/* 

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









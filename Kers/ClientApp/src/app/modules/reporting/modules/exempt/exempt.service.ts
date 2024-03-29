import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { TaxExempt, TaxExemptFinancialYear, TaxExemptFundsHandled, TaxExemptProgramCategory } from './exmpt';


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
    programCategories():Observable<TaxExemptProgramCategory[]>{
        var url = this.baseUrl + "programcategories";
        return this.http.get<TaxExemptProgramCategory[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('programcategories', <[]>{}))
            );
    }
    fundsHandled():Observable<TaxExemptFundsHandled[]>{
        var url = this.baseUrl + "fundshandled";
        return this.http.get<TaxExemptFundsHandled[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('TaxExemptFundsHandled', <[]>{}))
            );
    }
    exemptsList(countyId:number = 0):Observable<TaxExempt[]>{
        var url = this.baseUrl + "exemptslist/"+countyId;
        return this.http.get<TaxExempt[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('TaxExemptList', <[]>{}))
            );
    }

    addExempt( exempt:TaxExempt ):Observable<TaxExempt>{
        return this.http.post<TaxExempt>(this.location.prepareExternalUrl(this.baseUrl), exempt)
            .pipe(
                catchError(this.handleError('addExempt', <TaxExempt>{}))
            );
    }
    update(id:number, exempt:TaxExempt):Observable<TaxExempt>{
        var url = this.baseUrl + id;
        return this.http.put<TaxExempt>(this.location.prepareExternalUrl(url), JSON.stringify(exempt))
                .pipe(
                    catchError(this.handleError('update', exempt))
                );
    } 
    delete(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }

    pdf(id:number):Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/PdfExempt/exempt/' + id ), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('pdf', <Blob>{}))
            );
      }

}









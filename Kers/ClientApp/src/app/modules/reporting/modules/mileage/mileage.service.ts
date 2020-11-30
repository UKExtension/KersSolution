import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Mileage } from './mileage';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { ExpenseFundingSource } from '../expense/expense.service';
import { ProgramCategory } from '../admin/programs/programs.service';

@Injectable({
  providedIn: 'root'
})
export class MileageService {
  private baseUrl = '/api/Mileage/';
  private handleError: HandleError;
  private httpClient: HttpClient;

  constructor( 
      private http:HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler,
      handler: HttpBackend,
    ) {
        this.handleError = httpErrorHandler.createHandleError('Mileage Records');
        this.httpClient = new HttpClient(handler);
    }

    byRevId(id:number):Observable<Mileage>{
        var url = this.baseUrl + 'byrevid/' + id;
        return this.http.get<Mileage>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byRevId', <Mileage>{}))
            );
    }
    source(id:number):Observable<ExpenseFundingSource>{
        var url = this.baseUrl + 'source/' + id;
        return this.http.get<ExpenseFundingSource>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('source', <ExpenseFundingSource>{}))
            );
    }
    sources():Observable<ExpenseFundingSource[]>{
        var url = this.baseUrl + 'sources/';
        return this.http.get<ExpenseFundingSource[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('source', []))
            );
    }
    category(id:number):Observable<ProgramCategory>{
        var url = this.baseUrl + 'category/' + id;
        return this.http.get<ProgramCategory>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('category', <ProgramCategory>{}))
            );
    }
    categories():Observable<ProgramCategory[]>{
        var url = this.baseUrl + 'categories/';
        return this.http.get<ProgramCategory[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('category', []))
            );
    }

    latest(skip:number = 0, take:number = 6):Observable<Mileage[]>{
      var url = this.baseUrl + 'latest/' + skip + '/' + take;
      return this.http.get<Mileage[]>(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('latest', []))
          );
    }
    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('num', 0))
            );
    }

    mileagePerMonth(month:number, year:number = 2017, userid:number = 0, orderBy:string = 'desc') : Observable<Mileage[]>{
        var url = this.baseUrl + 'permonth/' + year + '/' + month + '/' + userid + '/' + orderBy;
        return this.http.get<Mileage[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('expensesPerMonth', []))
            );
    }

    add( expense:Mileage ):Observable<Mileage>{
      return this.http.post<Mileage>(this.location.prepareExternalUrl(this.baseUrl), expense)
          .pipe(
              catchError(this.handleError('add', <Mileage>{}))
          );
    }

    update(id:number, expense:Mileage):Observable<Mileage>{
        var url = this.baseUrl + id;
        return this.http.put<Mileage>(this.location.prepareExternalUrl(url), expense)
            .pipe(
                catchError(this.handleError('update', <Mileage>{}))
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

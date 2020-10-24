import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Mileage } from './mileage';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

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

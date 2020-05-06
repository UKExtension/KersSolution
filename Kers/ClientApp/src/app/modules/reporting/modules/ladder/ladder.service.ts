import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { LadderApplication, LadderLevel, LadderEducationLevel } from './ladder';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LadderService {

  private baseUrl = '/api/Ladder/';
  private handleError: HandleError;

  constructor( 
      private http:HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
    ) {
        this.handleError = httpErrorHandler.createHandleError('Meeting');
    }

    levels():Observable<LadderLevel[]>{
        var url = this.baseUrl + "levels/";
        return this.http.get<LadderLevel[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', []))
            );
    }
    educationLevels():Observable<LadderEducationLevel[]>{
        var url = this.baseUrl + "educationlevels/";
        return this.http.get<LadderEducationLevel[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', []))
            );
    }
    postFile(fileToUpload: File): Observable<boolean> {
        const endpoint = this.baseUrl + 'UploadFiles';
        const formData: FormData = new FormData();
        formData.append('fileKey', fileToUpload, fileToUpload.name);
        return this.http.post<boolean>(endpoint, formData)
            .pipe(
                catchError(this.handleError('levels', false))
            );
    }

  /*****************************/
  // CRUD operations
  /*****************************/
 
  add( ladder:LadderApplication ):Observable<LadderApplication>{
    return this.http.post<LadderApplication>(this.location.prepareExternalUrl(this.baseUrl + "addladder"), ladder)
        .pipe(
            catchError(this.handleError('add', <LadderApplication>{}))
        );
  }
/*
  update(id:number, unit:MeetingWithTime):Observable<Meeting>{
      var url = this.baseUrl + "updatemeeting/" + id;
      return this.http.put<Meeting>(this.location.prepareExternalUrl(url), unit)
          .pipe(
              catchError(this.handleError('update', <Meeting>{}))
          );
  }
  delete(id:number):Observable<{}>{
      var url = this.baseUrl + "deletemeeting/" + id;
      return this.http.delete(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('delete'))
          );
  }

  private addParams(params:{}){
      let searchParams = {};
      for(let p in params){
          searchParams[p] = params[p];
      }
      return  {params: searchParams};
  }

   */
}

import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { LadderApplication, LadderLevel, LadderEducationLevel } from './ladder';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LadderService {

  private baseUrl = '/api/Ladder/';
  private handleError: HandleError;
  private httpClient: HttpClient;

  constructor( 
      private http:HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler,
      handler: HttpBackend,
    ) {
        this.handleError = httpErrorHandler.createHandleError('LadderApplication');
        this.httpClient = new HttpClient(handler);
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

    applicationsByUser(id:number = 0):Observable<LadderApplication[]>{
        var url = this.baseUrl + "applicationsByUser/" + id;
        return this.http.get<LadderApplication[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('applicationsByUser', []))
            );
    }
    applicationByUserByFiscalYear(id:number = 0, fy:string = "0"):Observable<LadderApplication>{
        var url = this.baseUrl + "applicationByUserByFiscalYear/" + id + "/" + fy;
        return this.http.get<LadderApplication>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('applicationsByUser', <LadderApplication>{}))
            );
    }

    postFile(fileToUpload: File, userId:number): Observable<FileUploadResult> {
        const endpoint = '/api/Ladder/UploadFiles/' + userId;
        const formData: FormData = new FormData();
        formData.append('file', fileToUpload, fileToUpload.name);
        return this.httpClient.post<FileUploadResult>(endpoint, formData)
            .pipe(
                catchError(this.handleError('levels', <FileUploadResult>{}))
            ); 
    }

    deleteImage(id:number):Observable<{}>{
        var url = this.baseUrl + "deleteimage/" + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
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

export class FileUploadResult{
    success:boolean;
    message:string;
    fileId:number;
    imageId:number;
    fileName:string;
  }

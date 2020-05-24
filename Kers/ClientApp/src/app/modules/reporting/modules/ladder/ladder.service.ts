import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { LadderApplication, LadderLevel, LadderEducationLevel, LadderStage, LadderApplicationStage } from './ladder';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { LadderApplicantComponent } from './ladder-applicant.component';

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

    getStage(id:number):Observable<LadderStage>{
        var url = this.baseUrl + "getStage/" + id;
        return this.http.get<LadderStage>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', <LadderStage>{}))
            );
    }

    nextStage(id:number):Observable<LadderStage>{
        var url = this.baseUrl + "nextstage/" + id;
        return this.http.get<LadderStage>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', <LadderStage>{}))
            );
    }

    previousStage(id:number):Observable<LadderStage>{
        var url = this.baseUrl + "previousstage/" + id;
        return this.http.get<LadderStage>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', <LadderStage>{}))
            );
    }

    getApplication(id:number):Observable<LadderApplication>{
        var url = this.baseUrl + "Application/" + id;
        return this.http.get<LadderApplication>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('levels', <LadderApplication>{}))
            );
    }

    getApplicationsForReview(stageId:number):Observable<LadderApplication[]>{
        var url = this.baseUrl + "GetApplicationsForReview/" + stageId;
        return this.http.get<LadderApplication[]>(this.location.prepareExternalUrl(url))
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

    review( stage:LadderApplicationStage, approved:boolean ):Observable<LadderApplication>{
        var url = this.baseUrl + "review/"+ approved;
        return this.http.post<LadderApplication>(this.location.prepareExternalUrl(url), stage)
            .pipe(
                catchError(this.handleError('add', <LadderApplication>{}))
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

    update(id:number, application:LadderApplication):Observable<LadderApplication>{
      var url = this.baseUrl + "updateladder/" + id;
      return this.http.put<LadderApplication>(this.location.prepareExternalUrl(url), application)
          .pipe(
              catchError(this.handleError('update', <LadderApplication>{}))
          );
    }
    delete(id:number):Observable<{}>{
      var url = this.baseUrl + "deleteladder/" + id;
      return this.http.delete(this.location.prepareExternalUrl(url))
          .pipe(
              catchError(this.handleError('delete'))
          );
    }
/*
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

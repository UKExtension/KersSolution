import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';


@Injectable()
export class EmailService {

    private baseUrl = '/api/Email/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('EmailService');
        }


    send(email:Email):Observable<Email>{
        return this.http.post<Email>(this.location.prepareExternalUrl(this.baseUrl), email)
            .pipe(
                catchError(this.handleError('send', email))
            );
    }

}

export interface Email{
    pressets: number;
    server: string;
    port: number;
    username: string;
    password: string;
    from: string;
    to: string;
    subject: string;
    body: string;
}

import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MessageTemplate } from './message-template';


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



    add( template:EmailTemplate ):Observable<EmailTemplate>{
        return this.http.post<EmailTemplate>(this.location.prepareExternalUrl(this.baseUrl + 'addtemplate/'), template)
            .pipe(
                catchError(this.handleError('add', <EmailTemplate>{}))
            );
    }
    
    delete(id:number):Observable<{}>{
        var url = this.baseUrl + 'deletetemplate/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }

    update(id:number, template:EmailTemplate):Observable<EmailTemplate>{
        var url = this.baseUrl + 'updatetemplate/' + id;
        return this.http.put<EmailTemplate>(this.location.prepareExternalUrl(url), template)
                .pipe(
                    catchError(this.handleError('update', template))
                );
    } 

    gettemplates():Observable<MessageTemplate[]>{
        var url = this.baseUrl + "gettemplates/";
        return this.http.get<MessageTemplate[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('gettemplates', []))
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

export interface EmailTemplate{
    id:number;
    code:string;
    bodyHtml:string;
    bodyText:string;
}

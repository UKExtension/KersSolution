import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';


@Injectable()
export class ReportingHelpService {

    private baseUrl = '/api/HelpContent/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ReportingHelpService');
        }
    

    get(id:number) : Observable<Help>{
        var url = this.baseUrl + id;
        return this.http.get<Help>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('get', <Help>{}))
            );
    }

    

    
    
    
}

export class Help{
    constructor(
        public id: number,
        public title: string,
        public body: string,
        public categoryId: number
    ){}
}
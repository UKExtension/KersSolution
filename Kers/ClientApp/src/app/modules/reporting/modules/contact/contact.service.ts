import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import {ActivityOptionNumber, Race, Ethnicity} from '../activity/activity.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class ContactService {

    private baseUrl = '/api/contact/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ContactService');
        }


    perPeriod(start:Date, end:Date, userId:number = 0):Observable<Contact[]>{
        var url = this.baseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
        return this.http.get<Contact[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perPeriod', []))
            );
    }

    summaryPerMonth(userId:number = 0):Observable<{}[]>{
        var url = this.baseUrl + 'summaryPerMonth/' + userId;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('summaryPerMonth', []))
            );
    }

    summaryPerProgram(userId:number = 0):Observable<{}[]>{
        var url = this.baseUrl + 'summaryPerProgram/' + userId;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('summaryPerProgram', []))
            );
    }

    
    optionnumbers():Observable<ActivityOptionNumber[]>{
        var url = this.baseUrl + 'optionnumbers';
        return this.http.get<ActivityOptionNumber[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('optionnumbers', []))
            );
    }
    races():Observable<Race[]>{
        var url = this.baseUrl + 'races';
        return this.http.get<Race[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('races', []))
            );
    }
    ethnicities():Observable<Ethnicity[]>{
        var url = this.baseUrl + 'ethnicities';
        return this.http.get<Ethnicity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('ethnicities', []))
            );
    }

    add( contact:Contact ):Observable<Contact>{
        return this.http.post<Contact>(this.location.prepareExternalUrl(this.baseUrl), contact)
            .pipe(
                catchError(this.handleError('add', <Contact>{}))
            );
    }


    latest(skip:number = 0, take:number = 5):Observable<Contact[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Contact[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latest', []))
            );
    }
    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latest', 0))
            );
    }

    update(id:number, contact:Contact):Observable<Contact>{
        var url = this.baseUrl + id;
        return this.http.put<Contact>(this.location.prepareExternalUrl(url), contact)
            .pipe(
                catchError(this.handleError('update', <Contact>{}))
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

export interface Contact{
    id:number;
    contactDate:Date;
    contactId:number;
    days:number;
    multistate:number;
    majorProgramId:number;
    majorProgram:MajorProgram;
    contactRaceEthnicityValues:ContactRaceEthnicityValue[];
    female:number;
    male:number;
    contactOptionNumbers:ContactOptionNumberValue[];
}

export interface ContactOptionNumberValue{
    id:number;
    activityOptionNumberId:number;
    activityOptionNumber:ActivityOptionNumber;
    value:number;
}
export interface ContactRaceEthnicityValue{
    raceId:number;
    ethnicityId:number;
    amount:number;
}

export interface ContactMonth{
    month:number;
    year:number;
    date:Date;
    activities:Contact[];
}

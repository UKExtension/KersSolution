import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';
import {ActivityOptionNumber, Race, Ethnicity} from '../activity/activity.service';


@Injectable()
export class ContactService {

    private baseUrl = '/api/contact/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}


    perPeriod(start:Date, end:Date, userId:number = 0):Observable<Contact[]>{
        var url = this.baseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Contact[]>res.json())
                .catch(this.handleError);
    }

    summaryPerMonth(userId:number = 0){
        var url = this.baseUrl + 'summaryPerMonth/' + userId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }

    summaryPerProgram(userId:number = 0){
        var url = this.baseUrl + 'summaryPerProgram/' + userId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json() )
                .catch(this.handleError);
    }

    
    optionnumbers():Observable<ActivityOptionNumber[]>{
        var url = this.baseUrl + 'optionnumbers';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <ActivityOptionNumber[]>res.json())
                .catch(this.handleError);
    }
    races():Observable<Race[]>{
        var url = this.baseUrl + 'races';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Race[]>res.json())
                .catch(this.handleError);
    }
    ethnicities():Observable<Ethnicity[]>{
        var url = this.baseUrl + 'ethnicities';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Ethnicity[]>res.json())
                .catch(this.handleError);
    }

    add( contact:Contact ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(contact), this.getRequestOptions())
                    .map( res => <Contact>res.json() )
                    .catch(this.handleError);
    }


    latest(skip:number = 0, take:number = 5){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Contact[]>res.json() )
                .catch(this.handleError);
    }
    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    update(id:number, contact:Contact){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(contact), this.getRequestOptions())
                    .map( res => {
                        return <Contact> res.json();
                    })
                    .catch(this.handleError);
    }

    delete(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    getRequestOptions(){
        return new RequestOptions(
            {
                headers: new Headers({
                    "Content-Type": "application/json; charset=utf-8"
                })
            }
        )
    }

    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
    }
    
}

export interface Contact{
    id:number;
    contactDate:Date;
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

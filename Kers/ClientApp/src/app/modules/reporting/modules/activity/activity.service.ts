import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { Http, Response, Headers, RequestOptions, URLSearchParams, ResponseContentType } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';


@Injectable()
export class ActivityService {

    private baseUrl = '/api/activity/';


    private racesVar:Race[] = null;
    private activityOptionNumbers:ActivityOptionNumber[] = null;
    private activityOptions:ActivityOption[] = null;

    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}




    perDay(userId:number, start:Date, end:Date){
        var url = this.baseUrl + 'perDay/'+ userId + '/' + start.toISOString() + '/' + end.toISOString() ;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <[{}]>res.json())
                .catch(this.handleError);
    }
    perPeriod(start:Date, end:Date, userId:number = 0):Observable<Activity[]>{
        var url = this.baseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Activity[]>res.json())
                .catch(this.handleError);
    }

    latestByUser(userId:number, amount:number = 3):Observable<Activity[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Activity[]>res.json())
                .catch(this.handleError);
    }

    yearsWithActivities(id:number = 0){
        var url = this.baseUrl + 'years/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }
    monthsWithActivities(year, userid:number = 0){
        var url = this.baseUrl + 'months/' + year + '/' + userid;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }

    activitiesPerMonth(month:number, year:number = 2017, userid:number = 0, orderBy:string = "desc") : Observable<Activity[]>{
        var url = this.baseUrl + 'permonth/' + year + '/' + month + '/' + userid + '/' + orderBy;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Activity[]>res.json() )
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


    options():Observable<ActivityOption[]>{
        if(this.activityOptions == null){
            var url = this.baseUrl + 'options';
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <ActivityOption[]>res.json())
                .catch(this.handleError);
        }else{
            return Observable.of(this.activityOptions);
        }
        
    }
    optionnumbers():Observable<ActivityOptionNumber[]>{
        if(this.activityOptionNumbers == null){
            var url = this.baseUrl + 'optionnumbers';
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    this.activityOptionNumbers = <ActivityOptionNumber[]>res.json();
                    return this.activityOptionNumbers;
                })
                .catch(this.handleError);
        }else{
            return Observable.of(this.activityOptionNumbers);
        }
        
    }
    races():Observable<Race[]>{
        if(this.racesVar == null){
            var url = this.baseUrl + 'races';
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    this.racesVar = <Race[]>res.json();
                    return this.racesVar;
                })
                .catch(this.handleError);
        }else{
            return Observable.of(this.racesVar);
        }
        
    }
    ethnicities():Observable<Ethnicity[]>{
        var url = this.baseUrl + 'ethnicities';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Ethnicity[]>res.json())
                .catch(this.handleError);
    }

    add( activity:Activity ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => <Activity>res.json() )
                    .catch(this.handleError);
    }


    latest(skip:number = 0, take:number = 5){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Activity[]>res.json() )
                .catch(this.handleError);
    }
    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    pdf(year:number, month:number, id:number = 0){
        return this.http.get(this.location.prepareExternalUrl('/api/PdfActivity/activities/' + year + '/' + month + '/' + id ), { responseType: ResponseContentType.Blob })
                .map((res:Response) => {
                    var pd = res.blob();
                    return pd;
                })
                .catch(this.handleError);
    }

    csv(year:number, month:number, id:number = 0){
        return this.http.get(this.location.prepareExternalUrl('/api/Activity/' + year + '/' + month + '/' + id + '/data.csv'), { responseType: ResponseContentType.Blob })
        .map((res:Response) => {
            var pd = res.blob();
            return pd;
        })
        .catch(this.handleError);
    }

    update(id:number, activity:Activity){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => {
                        return <Activity> res.json();
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

export interface Activity{
    id:number;
    activityId:number;
    activityDate:Date;
    hours:number;
    majorProgramId:number;
    majorProgram: MajorProgram;
    title:string;
    description:string;
    activityOptionSelections:ActivityOptionSelection[];
    raceEthnicityValues:RaceEthnicityValue[];
    female:number;
    male:number;
    activityOptionNumbers:ActivityOptionNumberValue[];
    isSnap:boolean;
    classicSnapId?: number;
    classicIndirectSnapId?:number;
}
export interface ActivityOption{
    id:number;
    name:string;
    order:number;
}
export interface ActivityOptionSelection{
    activityOptionId:number;
    activityOption:ActivityOption;
    selected:boolean;
}
export interface ActivityOptionNumber{
    id:number;
    name:string;
    order:number;
}
export interface ActivityOptionNumberValue{
    id:number;
    activityOptionNumberId:number;
    activityOptionNumber:ActivityOptionNumber;
    value:number;
}
export interface Race{
    id:number;
    name:string;
}
export interface Ethnicity{
    id:number;
    name:string;
}
export interface RaceEthnicityValue{
    raceId:number;
    ethnicityId:number;
    amount:number;
}

export interface ActivityMonth{
    month:number;
    year:number;
    date:Date;
    activities:Activity[];
}

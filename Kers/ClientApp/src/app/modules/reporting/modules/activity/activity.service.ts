import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap, retry } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { User, PlanningUnit } from '../user/user.service';
import { Servicelog } from '../servicelog/servicelog.service';


@Injectable()
export class ActivityService {

    private baseUrl = '/api/activity/';
    private handleError: HandleError;


    private racesVar:Race[] = null;
    private activityOptionNumbers:ActivityOptionNumber[] = null;
    private activityOptions:ActivityOption[] = null;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ActivityService');
        }




    perDay(userId:number, start:Date, end:Date):Observable<{}[]>{
        var url = this.baseUrl + 'perDay/'+ userId + '/' + start.toISOString() + '/' + end.toISOString() ;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perDay', []))
            );
    }
    perPeriod(start:Date, end:Date, userId:number = 0):Observable<Activity[]>{
        var url = this.baseUrl + 'perPeriod/' + start.toISOString() + '/' + end.toISOString()+ '/' + userId  ;
        return this.http.get<Activity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perPeriod', []))
            );
    }

    latestByUser(userId:number, amount:number = 3):Observable<Activity[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount;
        return this.http.get<Activity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latestByUser', []))
            );
    }

    yearsWithActivities(id:number = 0):Observable<string[]>{
        var url = this.baseUrl + 'years/' + id;
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('yearsWithActivities', []))
            );
    }
    monthsWithActivities(year, userid:number = 0):Observable<string[]>{
        var url = this.baseUrl + 'months/' + year + '/' + userid;
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('monthsWithActivities', []))
            );
    }

    activitiesPerMonth(month:number, year:number = 2017, userid:number = 0, orderBy:string = "desc", isSnap:boolean = false) : Observable<Activity[]>{
        var url = this.baseUrl + 'permonth/' + year + '/' + month + '/' + userid + '/' + orderBy + '/' + isSnap;
        return this.http.get<Activity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('activitiesPerMonth', []))
            );
    }
    summaryPerMonth(userId:number = 0, fy:string = "0"):Observable<{}[]>{
        var url = this.baseUrl + 'allContactsSummaryPerMonth/' + userId + '/'+fy;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('summaryPerMonth', []))
            );
    }

    summaryPerProgram(userId:number = 0, fy:string = "0"):Observable<{}[]>{
        var url = this.baseUrl + 'allContactsSummaryPerProgram/' + userId + '/' + fy;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('summaryPerProgram', []))
            );
    }


    options():Observable<ActivityOption[]>{
        if(this.activityOptions == null){
            var url = this.baseUrl + 'options';
            return this.http.get<ActivityOption[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(opts => this.activityOptions = opts),
                    catchError(this.handleError('summaryPerProgram', []))
                );
        }else{
            return of(this.activityOptions);
        }
        
    }
    optionnumbers():Observable<ActivityOptionNumber[]>{
        if(this.activityOptionNumbers == null){
            var url = this.baseUrl + 'optionnumbers';
            return this.http.get<ActivityOptionNumber[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(opts => this.activityOptionNumbers = opts),
                    catchError(this.handleError('optionnumbers', []))
                );
        }else{
            return of(this.activityOptionNumbers);
        }
        
    }
    races():Observable<Race[]>{
        if(this.racesVar == null){
            var url = this.baseUrl + 'races';
            return this.http.get<Race[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(opts => this.racesVar = opts),
                    catchError(this.handleError('races', []))
                );
        }else{
            return of(this.racesVar);
        }
        
    }
    ethnicities():Observable<Ethnicity[]>{
        var url = this.baseUrl + 'ethnicities';
        return this.http.get<Ethnicity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('ethnicities', []))
            );
    }

    add( activity:Activity ):Observable<Activity>{
        return this.http.post<Activity>(this.location.prepareExternalUrl(this.baseUrl), activity)
            .pipe(
                catchError(this.handleError('add', <Activity>{}))
            );
    }


    latest(skip:number = 0, take:number = 5):Observable<Activity[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Activity[]>(this.location.prepareExternalUrl(url))
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

    numberActivitiesPerYear(fiscalYaerId:number, userId:number):Observable<number>{

    
        var url = this.baseUrl + 'numberActivitiesPerYear/' + fiscalYaerId + "/" + userId;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('num', 0))
            );

    }

    pdf(year:number, month:number, id:number = 0): Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/PdfActivity/activities/' + year + '/' + month + '/' + id ), {responseType: 'blob'})

        /* 
        .map((res:Response) => {
                    var pd = res.blob();
                    return pd;
                })
                .catch(this.handleError); */
    }

    csv(year:number, month:number, id:number = 0): Observable<string[]>{
        return this.http.get<string[]>(this.location.prepareExternalUrl('/api/Activity/' + year + '/' + month + '/' + id + '/data.csv'))
        
        /* 
        
        .map((res:Response) => {
            var pd = res.blob();
            return pd;
        })
        .catch(this.handleError);

         */
    }

    update(id:number, activity:Activity):Observable<Activity>{
        var url = this.baseUrl + id;
        return this.http.put<Activity>(this.location.prepareExternalUrl(url), JSON.stringify(activity))
            .pipe(
                catchError(this.handleError('update', activity))
            );
    }

    delete(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }
    getCustom(criteria:ActivitySearchCriteria, userId:number | null = null):Observable<ActivitySeearchResultsWithCount>{
        var url = this.baseUrl + 'getCustom/';
        if( userId != null ) url += userId;
        return this.http.post<ActivitySeearchResultsWithCount>(this.location.prepareExternalUrl(url), criteria)
            .pipe(
                catchError(this.handleError('getCustom',<ActivitySeearchResultsWithCount>{}))
            );
    }
    GetCustomDataHeader():Observable<string[]>{
        var url = this.baseUrl + 'GetCustomDataHeader/';
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('GetCustomDataHeader', []))
            );
    }
    getCustomData(criteria:ActivitySearchCriteria, userId:number | null = null):Observable<string[]>{
        var url = this.baseUrl + 'getCustomData/';
        if( userId != null ) url += userId;
        return this.http.post<string[]>(this.location.prepareExternalUrl(url), criteria)
            .pipe(
                retry(3),
                catchError(this.handleError('getCustomData',[]))
            );
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
    activityImages:ActivityImage[];
}
export interface ActivityImage{
    id:number;
    activityRevisionId:number;
    uploadImageId:number;
    created:Date;
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

export class ActivitySearchCriteria{
    start: string;
    end: string;
    search: string = "";
    order: string = 'dsc';
    congressionalDistrictId?:number;
    regionId?:number;
    areaId?:number;
    unitId?:number;
    positionId?:number;
    specialtyId?:number;
    options:number[];
    skip:number = 0;
    take?:number;
}

export class ActivitySearchResult{
    user: User;
    revision: Servicelog;
    unit: PlanningUnit
}
export class ActivitySeearchResultsWithCount{
    results:ActivitySearchResult[];
    resultsCount:number;
}

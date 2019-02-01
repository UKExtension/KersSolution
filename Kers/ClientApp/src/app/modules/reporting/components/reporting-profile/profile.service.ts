import { Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import {Location} from '@angular/common';
import { HandleError, HttpErrorHandler } from '../../core/services/http-error-handler.service';
import { PlanningUnit } from '../../modules/plansofwork/plansofwork.service';
import { ExtensionPosition } from '../../modules/user/user.service';


@Injectable()
export class ProfileService {

    private baseUrl = '/api/Profile/';

    public profile:Profile = null;

    private pUnits:PlanningUnit[] = null;
    private pstns:ExtensionPosition[] = null;
    private lctns = null;
    private rls = null;
    private handleError: HandleError;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ProfileService');
        }

    getLatest(num?:number){
        var url = this.baseUrl + "GetLatest/";
        if( num != null ) url += num;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getLatest'))
            );
    }

    getRandom(num?:number){
        var url = this.baseUrl + "GetRandom/";
        if (num != null) url += num;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getLatest'))
            );
    }

    getCustom(searchParams?:{}) : Observable<Profile[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<Profile[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}):Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }

    get(id:number){
        if(id==null) throw new Error("id is required");
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('get'))
            );
    }

    currentUser():Observable<Profile>{
        var url = this.baseUrl + "CurrentUser/";
        if(this.profile == null){
            return this.http.get<Profile>(this.location.prepareExternalUrl(url))
            .pipe(
                tap(res => this.profile = res),
                catchError(this.handleError('currentUser', <Profile>{}))
            );
        }else{
            return of(this.profile);
        }
        
    }

    update(id:Number, profile:Profile, admin?:boolean):Observable<Profile>{
        var url = this.baseUrl + id;
        if(admin == true){
            url += '/true';
        }
        return this.http.put<Profile>(this.location.prepareExternalUrl(url), profile)
            .pipe(
                catchError(this.handleError('update', <Profile>{}))
            );
    }

    planningUnits():Observable<PlanningUnit[]>{
        if(this.pUnits == null){
            var url = this.baseUrl + "PlanningUnit";
            return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(res => this.pUnits = res),
                    catchError(this.handleError('planningUnits', []))
                );
        }else{
            return of(this.pUnits);
        }
    }

    positions():Observable<ExtensionPosition[]>{
        if(this.pstns == null){
            var url = this.baseUrl + "Position";
            return this.http.get<ExtensionPosition[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(res => this.pstns = res),
                    catchError(this.handleError('positions', []))
                );
        }else{
            return of(this.pstns);
        }
    }

    locations(){
        if(this.lctns == null){
            var url = this.baseUrl + "Location";
            return this.http.get(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(res => this.lctns = res),
                    catchError(this.handleError('locations', []))
                );
        }else{
            return of(this.lctns);
        }
    }

    private addParams(params:{}){
        let searchParams = {};
        for(let p in params){
            searchParams[p] = params[p];
        }
        return  {params: searchParams};
    }

    
}

export class Profile{
    constructor(
        public id:number,
        public rDT:Date,
        public enabled: boolean,
        public sapLeaveRequestPilotGroup: boolean,
        public extensionIntern: boolean,
        public instID: string,
        public planningUnitID: string,
        public planningUnitName: string,
        public positionID: string,
        public progANR: boolean,
        public progHORT: boolean,
        public progFCS: boolean,
        public prog4HYD: boolean,
        public progFACLD: boolean,
        public progNEP: boolean,
        public progOther: boolean,
        public locationID: string,
        public linkBlueID: string,
        public personID: string,
        public personName: string,
        public isDD: boolean,
        public isCesInServiceTrainer: boolean,
        public isCesInServiceAdmin: boolean,
        public emailDeliveryAddress: string,
        public emailUEA: string
    ){}
}
import { Injectable} from '@angular/core';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import {Location} from '@angular/common';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';


@Injectable()
export class ProfileService {

    private baseUrl = '/api/Profile/';

    public profile:Profile = null;

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private rls = null;

    constructor( private http:AuthHttp, private location:Location){

    }

    getLatest(num?:number){
        var url = this.baseUrl + "GetLatest/";
        if( num != null ) url += num;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(response => response.json())
            .catch(this.handleError);
    }

    getRandom(num?:number){
        var url = this.baseUrl + "GetRandom/";
        if (num != null) url += num;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(response => response.json())
            .catch(this.handleError);
    }

    getCustom(searchParams?:{}) : Observable<Profile[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }

    getCustomCount(searchParams?:{}){
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }

    get(id:number){
        if(id==null) throw new Error("id is required");
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <Profile>res.json())
            .catch(this.handleError);
    }

    currentUser(){
        var url = this.baseUrl + "CurrentUser/";
        if(this.profile == null){
            return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => this.profile = <Profile>res.json())
            .catch(this.handleError);
        }else{
            return Observable.of(this.profile);
        }
        
    }

    update(id, profile, admin?:boolean){
        var url = this.baseUrl + id;
        if(admin == true){
            url += '/true';
        }
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(profile), this.getRequestOptions())
            .map(response => {
                return <Profile>response.json();
            })
            .catch(this.handleError)
    }

    planningUnits(){
        if(this.pUnits == null){
            var url = this.baseUrl + "PlanningUnit";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.pUnits = res.json())
                .catch(this.handleError);
        }else{
            return Observable.of(this.pUnits);
        }
    }

    positions(){
        if(this.pstns == null){
            var url = this.baseUrl + "Position";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.pstns = res.json())
                .catch(this.handleError);
        }else{
            return Observable.of(this.pstns);
        }
    }

    locations(){
        if(this.lctns == null){
            var url = this.baseUrl + "Location";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.lctns = res.json())
                .catch(this.handleError);
        }else{
            return Observable.of(this.lctns);
        }
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
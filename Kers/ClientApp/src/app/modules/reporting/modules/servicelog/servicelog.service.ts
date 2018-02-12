import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';


@Injectable()
export class ServicelogService {

    private baseUrl = '/api/Servicelog/';


    private racesVar:Race[] = null;
    private activityOptionNumbers:ActivityOptionNumber[] = null;
    private activityOptions:ActivityOption[] = null;

    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}



    latestByUser(userId:number, amount:number = 3):Observable<Servicelog[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Servicelog[]>res.json())
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

    /*****************************/
    // Snap Ed Direct
    /*****************************/

    getSnapDirect(id:number):Observable<SnapDirect>{
        var url = this.baseUrl + 'getsnapdirect/'+id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapDirect>res.json())
                .catch(this.handleError);
    }

    sessiontypes():Observable<SnapDirectSessionType[]>{
        var url = this.baseUrl + 'sessiontypes';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapDirectSessionType[]>res.json())
                .catch(this.handleError);
    }

    snapdirectages():Observable<SnapDirectAges[]>{
        var url = this.baseUrl + 'snapdirectages';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapDirectAges[]>res.json())
                .catch(this.handleError);
    }

    snapdirectaudience():Observable<SnapDirectAudience[]>{
        var url = this.baseUrl + 'snapdirectaudience';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapDirectAudience[]>res.json())
                .catch(this.handleError);
    }

    snapdirectdeliverysite():Observable<SnapDirectDeliverySite[]>{
        var url = this.baseUrl + 'snapdirectdeliverysite';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapDirectDeliverySite[]>res.json())
                .catch(this.handleError);
    }

    /*****************************/
    // Snap Ed InDirect
    /*****************************/

    getSnapInDirect(id:number):Observable<SnapIndirect>{
        var url = this.baseUrl + 'getsnapindirect/'+id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapIndirect>res.json())
                .catch(this.handleError);
    }

    snapindirectmethod():Observable<SnapIndirectMethod[]>{
        var url = this.baseUrl + 'snapindirectmethod';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapIndirectMethod[]>res.json())
                .catch(this.handleError);
    }

    snapindirectreached():Observable<SnapIndirectReached[]>{
        var url = this.baseUrl + 'snapindirectreached';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapIndirectReached[]>res.json())
                .catch(this.handleError);
    }



    /*****************************/
    // Snap Ed Policy
    /*****************************/

    getSnapPolicy(id:number):Observable<SnapPolicy>{
        var url = this.baseUrl + 'getsnappolicy/'+id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapPolicy>res.json())
                .catch(this.handleError);
    }

    snappolicyaimed():Observable<SnapPolicyAimed[]>{
        var url = this.baseUrl + 'snappolicyaimed';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapPolicyAimed[]>res.json())
                .catch(this.handleError);
    }

    snappolicypartner():Observable<SnapPolicyPartner[]>{
        var url = this.baseUrl + 'snappolicypartner';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapPolicyPartner[]>res.json())
                .catch(this.handleError);
    }



    /*****************************/
    // CRUD operations
    /*****************************/
    add( activity:Servicelog ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => {
                    
                        var ret = <Servicelog>res.json();
                        return ret;
                    } )
                    .catch(this.handleError);
    }
    update(id:number, activity:Servicelog){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(activity), this.getRequestOptions())
                    .map( res => {
                        return <Servicelog> res.json();
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


    latest(skip:number = 0, take:number = 5){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Servicelog[]>res.json() )
                .catch(this.handleError);
    }
    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
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

/************************************/
// Activity Entities
/************************************/

export interface Servicelog{
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
    snapDirect:SnapDirect;
    snapIndirect:SnapIndirect;
    snapPolicy:SnapPolicy;
    snapCopies:number;
    snapAdmin:boolean;
    isPolicy: boolean;
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

export interface ServicelogMonth{
    month:number;
    year:number;
    date:Date;
    activities:Servicelog[];
}

/************************************/
// Snap Ed Direct
/************************************/

export interface SnapDirect{
    id: number;
    siteName: string;
    snapDirectDeliverySiteId:number;
    snapDirectDeliverySite: SnapDirectDeliverySite;
    snapDirectSessionTypeId:number;
    snapDirectSessionType:SnapDirectSessionType;
    snapDirectAgesAudienceValues: SnapDirectAgesAudienceValue[];
}


export interface SnapDirectAges{
    id:number;
    name:string;
}

export interface SnapDirectAudience{
    id:number;
    name:string;
}

export interface SnapDirectAgesAudienceValue{
    //snapDirectAges:SnapDirectAges;
    snapDirectAgesId:number;
    //snapDirectAudience:SnapDirectAudience;
    snapDirectAudienceId:number;
    value:number;
}

export interface SnapDirectDeliverySite{
    id:number;
    name:string;
}

export interface SnapDirectSessionType{
    id:number;
    name:string;
}


/************************************/
// Snap Ed InDirect
/************************************/

export interface SnapIndirect{
    id: number;
    snapIndirectMethodSelections:SnapIndirectMethodSelection[];
    snapIndirectReachedValues:SnapIndirectReachedValue[];
}

export interface SnapIndirectMethod{
    id:number;
    name:string;
}

export interface SnapIndirectMethodSelection{
    id:number;
    snapIndirectMethodId:number;
    snapIndirectMethod:SnapIndirectMethod;
}

export interface SnapIndirectReached{
    id:number;
    name:string;
}

export interface SnapIndirectReachedValue{
    id:number;
    snapIndirectReachedId:number;
    //snapIndirectReached:SnapIndirectReached;
    value:number;
}

/************************************/
// Snap Ed Policy
/************************************/

export interface SnapPolicy{
    id:number;
    purpose:string;
    result:string;
    snapPolicyAimedSelections: SnapPolicyAimedSelection[];
    snapPolicyPartnerValue: SnapPolicyPartnerValue[];
}

export interface SnapPolicyAimed{
    id:number;
    name:string;
}

export interface SnapPolicyAimedSelection{
    snapPolicyAimedId:number;
    snapPolicyAimed:SnapPolicyAimed;
}

export interface SnapPolicyPartner{
    id:number;
    name:string;
}

export interface SnapPolicyPartnerValue{
    snapPolicyPartnerId:number;
    //snapPolicyPartner:SnapPolicyPartner;
    value:number;
}
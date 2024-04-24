import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { ActivityImage } from '../activity/activity.service';
import { ExtensionEventLocation } from '../events/extension-event';


@Injectable()
export class ServicelogService {

    private baseUrl = '/api/Servicelog/';
    private handleError: HandleError;

    private racesVar:Race[] = null;
    private activityOptionNumbers:ActivityOptionNumber[] = null;
    private activityOptions:ActivityOption[] = null;

    public cond = false;
    public condition = false;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ServicelogService');
        }



    latestByUser(userId:number, amount:number = 3):Observable<Servicelog[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount;
        return this.http.get<Servicelog[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latestByUser', []))
            );
    }

    options():Observable<ActivityOption[]>{
        if(this.activityOptions == null){
            var url = this.baseUrl + 'options';
            return this.http.get<ActivityOption[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(opts => this.activityOptions = opts),
                    catchError(this.handleError('options', []))
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
                    tap(races => this.racesVar = races),
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

    byId(id:number):Observable<Servicelog>{
        var url = this.baseUrl + 'byid/'+ id;
        return this.http.get<Servicelog>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byId', <Servicelog>{}))
            );
    }

    /*****************************/
    // Snap Ed Direct
    /*****************************/

    getSnapDirect(id:number):Observable<SnapDirect>{
        var url = this.baseUrl + 'getsnapdirect/'+id;
        return this.http.get<SnapDirect>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getSnapDirect', <SnapDirect>{}))
            );
    }

    sessiontypes():Observable<SnapDirectSessionType[]>{
        var url = this.baseUrl + 'sessiontypes';
        return this.http.get<SnapDirectSessionType[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('sessiontypes', []))
            );
    }
    sessionlengths():Observable<SnapDirectSessionLength[]>{
        var url = this.baseUrl + 'sessionlengths';
        return this.http.get<SnapDirectSessionLength[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('sessionlengths', []))
            );
    }


    

    snapdirectages():Observable<SnapDirectAges[]>{
        var url = this.baseUrl + 'snapdirectages';
        return this.http.get<SnapDirectAges[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapdirectages', []))
            );
    }

    snapdirectaudience():Observable<SnapDirectAudience[]>{
        var url = this.baseUrl + 'snapdirectaudience';
        return this.http.get<SnapDirectAudience[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapdirectaudience', []))
            );
    }

    snapdirectdeliverysite():Observable<SnapDirectDeliverySite[]>{
        var url = this.baseUrl + 'snapdirectdeliverysite';
        return this.http.get<SnapDirectDeliverySite[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapdirectdeliverysite', []))
            );
    }

    /*****************************/
    // Snap Ed InDirect
    /*****************************/

    getSnapInDirect(id:number):Observable<SnapIndirect>{
        var url = this.baseUrl + 'getsnapindirect/'+id;
        return this.http.get<SnapIndirect>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getSnapInDirect', <SnapIndirect>{}))
            );
    }

    snapindirectmethod():Observable<SnapIndirectMethod[]>{
        var url = this.baseUrl + 'snapindirectmethod';
        return this.http.get<SnapIndirectMethod[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapindirectmethod', []))
            );
    }
    snapIndirectAudienceTargeted():Observable<SnapIndirectAudienceTargeted[]>{
        var url = this.baseUrl + 'SnapIndirectAudienceTargeted';
        return this.http.get<SnapIndirectAudienceTargeted[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('SnapIndirectAudienceTargeted', []))
            );
    }

    snapindirectreached():Observable<SnapIndirectReached[]>{
        var url = this.baseUrl + 'snapindirectreached';
        return this.http.get<SnapIndirectReached[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapindirectreached', []))
            );
    }



    /*****************************/
    // Snap Ed Policy
    /*****************************/

    getSnapPolicy(id:number):Observable<SnapPolicy>{
        var url = this.baseUrl + 'getsnappolicy/'+id;
        return this.http.get<SnapPolicy>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getSnapInDirect', <SnapPolicy>{}))
            );
    }

    snappolicyaimed():Observable<SnapPolicyAimed[]>{
        var url = this.baseUrl + 'snappolicyaimed';
        return this.http.get<SnapPolicyAimed[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snappolicyaimed', []))
            );
    }

    snappolicypartner():Observable<SnapPolicyPartner[]>{
        var url = this.baseUrl + 'snappolicypartner';
        return this.http.get<SnapPolicyPartner[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snappolicypartner', []))
            );
    }



    /*****************************/
    // CRUD operations
    /*****************************/
    add( activity:Servicelog ):Observable<Servicelog>{
        return this.http.post<Servicelog>(this.location.prepareExternalUrl(this.baseUrl), activity)
            .pipe(
                catchError(this.handleError('add', activity))
            );
    }
    update(id:number, activity:Servicelog):Observable<Servicelog>{
        var url = this.baseUrl + id;
        return this.http.put<Servicelog>(this.location.prepareExternalUrl(url), activity)
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

    deleteByActivityId(id:number):Observable<{}>{
        var url = this.baseUrl + 'byactivityid/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteByActivityId'))
            );
    }


    latest(skip:number = 0, take:number = 5):Observable<Servicelog[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Servicelog[]>(this.location.prepareExternalUrl(url))
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
    snapDirectId?:number;
    snapIndirect:SnapIndirect;
    snapIndirectId?:number;
    snapPolicy:SnapPolicy;
    snapPolicyId?:number;
    snapCopies:number;
    snapCopiesBW:number;
    snapAdmin:boolean;
    isPolicy: boolean;
    activityImages:ActivityImage[];
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
    еxtensionEventLocationId: number;
    extensionEventLocation: ExtensionEventLocation;
    snapDirectSessionTypeId:number;
    snapDirectSessionType:SnapDirectSessionType;
    snapDirectSessionLengthId: number;
    snapDirectSessionLength: SnapDirectSessionLength;
    snapDirectAgesAudienceValues: SnapDirectAgesAudienceValue[];
}

export interface SnapDirectSessionLength{
    id:number;
    name:string;
    order:number;
    active: boolean;
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
    snapIndirectAudienceTargetedId:number;
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

export interface SnapIndirectAudienceTargeted{
    id:number;
    name:string;
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
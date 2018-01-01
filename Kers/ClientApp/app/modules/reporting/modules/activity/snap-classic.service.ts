import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';
import {Activity} from './activity.service';

@Injectable()
export class SnapClassicService {

    private baseUrl = '/api/snapclassic/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}


    site():Observable<zzSnapEdDeliverySite[]>{
        var url = this.baseUrl + 'site';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <zzSnapEdDeliverySite[]>res.json())
                .catch(this.handleError);
    }
    session():Observable<zzSnapEdSessionTypes[]>{
        var url = this.baseUrl + 'session';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <zzSnapEdSessionTypes[]>res.json())
                .catch(this.handleError);
    }

    process(vals, activity:Activity):{classicSnapId: Observable<SnapClassic>, classicIndirectSnapId: Observable<SnapClassic>}{
        
        if(vals.isSnap){
            return {classicSnapId:this.direct(vals, activity), classicIndirectSnapId:this.indirect(vals, activity) };
        }else{
            return {classicSnapId:Observable.of(null),classicIndirectSnapId:Observable.of(null)};
        }
    }



    indirect(vals, activity:Activity):Observable<SnapClassic>{
        vals.snapClassic.indirect.snapModeID = 2;
        if(vals.activityOptionNumbers.filter(n => n.activityOptionNumberId == 3)[0].value > 0){
            vals.snapClassic.indirect.snapCopies = vals.snapClassic.snapCopies;
            if(activity != null && activity.classicIndirectSnapId != 0 && activity.classicIndirectSnapId != null ){
                return this.update(activity.classicIndirectSnapId, <SnapClassic> vals.snapClassic.indirect);
            }else{
                return this.add(<SnapClassic> vals.snapClassic.indirect);
            }
        }else{
            return Observable.of(null)
        }
    }
    direct(vals, activity:Activity):Observable<SnapClassic>{
        vals.snapClassic.direct.snapModeID = 1;
        if((vals.male  + vals.female) > 0){
            if( !(vals.activityOptionNumbers.filter(n => n.activityOptionNumberId == 3)[0].value > 0)){
                vals.snapClassic.direct.snapCopies = vals.snapClassic.snapCopies;
            }
            vals.snapClassic.direct.snapDirectGenderMale                    = vals.male;
            vals.snapClassic.direct.snapDirectGenderFemale                  = vals.female;
            vals.snapClassic.direct.snapDirectRaceWhiteNonHispanic          = vals.raceEthnicityValues.filter(n => n.raceId == 1 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceWhiteHispanic             = vals.raceEthnicityValues.filter(n => n.raceId == 1 && n.ethnicityId == 2)[0].amount;
            vals.snapClassic.direct.snapDirectRaceBlackNonHispanic          = vals.raceEthnicityValues.filter(n => n.raceId == 2 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceBlackHispanic             = vals.raceEthnicityValues.filter(n => n.raceId == 2 && n.ethnicityId == 2)[0].amount;
            vals.snapClassic.direct.snapDirectRaceAsianNonHispanic          = vals.raceEthnicityValues.filter(n => n.raceId == 3 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceAsianHispanic             = vals.raceEthnicityValues.filter(n => n.raceId == 3 && n.ethnicityId == 2)[0].amount;
            vals.snapClassic.direct.snapDirectRaceAmericanIndianNonHispanic = vals.raceEthnicityValues.filter(n => n.raceId == 4 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceAmericanIndianHispanic    = vals.raceEthnicityValues.filter(n => n.raceId == 4 && n.ethnicityId == 2)[0].amount;
            vals.snapClassic.direct.snapDirectRaceHawaiianNonHispanic       = vals.raceEthnicityValues.filter(n => n.raceId == 5 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceHawaiianHispanic          = vals.raceEthnicityValues.filter(n => n.raceId == 5 && n.ethnicityId == 2)[0].amount;
            vals.snapClassic.direct.snapDirectRaceOtherNonHispanic          = vals.raceEthnicityValues.filter(n => n.raceId == 6 && n.ethnicityId == 1)[0].amount + vals.raceEthnicityValues.filter(n => n.raceId == 7 && n.ethnicityId == 1)[0].amount;
            vals.snapClassic.direct.snapDirectRaceOtherHispanic             = vals.raceEthnicityValues.filter(n => n.raceId == 6 && n.ethnicityId == 2)[0].amount + vals.raceEthnicityValues.filter(n => n.raceId == 7 && n.ethnicityId == 2)[0].amount;

            if(activity != null && activity.classicSnapId != 0 && activity.classicSnapId != null){
                return this.update(activity.classicSnapId, <SnapClassic> vals.snapClassic.direct);
            }else{
                return this.add(<SnapClassic> vals.snapClassic.direct);
            }
        }else{
            return Observable.of(null)
        }
    }

    get(id:number):Observable<SnapClassic>{
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapClassic>res.json())
                .catch(this.handleError);
    }


    add( snap:SnapClassic ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(snap), this.getRequestOptions())
                    .map( res => <SnapClassic>res.json() )
                    .catch(this.handleError);
    }


    update(id:number, snap:SnapClassic){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(snap), this.getRequestOptions())
                    .map( res => {
                        return <SnapClassic> res.json();
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

export interface SnapClassic{
    rID:number;
    snapModeID?:number;
    hours:number;
    snapDate:string;
    snapHours:string;
    snapCopies: string;
    snapDirectDeliverySiteID?:number;
    snapDirectSpecificSiteName?:string;
    snapDirectSessionTypeID?:number;
    snapDirectAudience_00_04_FarmersMarket:string;
    snapDirectAudience_05_17_FarmersMarket:string;
    snapDirectAudience_18_59_FarmersMarket:string;
    snapDirectAudience_60_pl_FarmersMarket:string;
    snapDirectAudience_00_04_PreSchool:string;
    snapDirectAudience_05_17_PreSchool:string;
    snapDirectAudience_18_59_PreSchool:string;
    snapDirectAudience_60_pl_PreSchool:string;
    snapDirectAudience_00_04_Family:string;
    snapDirectAudience_05_17_Family:string;
    snapDirectAudience_18_59_Family:string;
    snapDirectAudience_60_pl_Family:string;
    snapDirectAudience_00_04_SchoolAge:string;
    snapDirectAudience_05_17_SchoolAge:string;
    snapDirectAudience_18_59_SchoolAge:string;
    snapDirectAudience_60_pl_SchoolAge:string;
    snapDirectAudience_00_04_LimitedEnglish:string;
    snapDirectAudience_05_17_LimitedEnglish: string;
    snapDirectAudience_18_59_LimitedEnglish: string;
    snapDirectAudience_60_pl_LimitedEnglish:string;
    snapDirectAudience_00_04_Seniors: string;
    napDirectAudience_05_17_Seniors: string;
    snapDirectAudience_18_59_Seniors: string;
    snapDirectAudience_60_pl_Seniors: string;
    snapDirectGenderMale: string;
    snapDirectGenderFemale: string;
    snapDirectRaceWhiteNonHispanic: string;
    snapDirectRaceWhiteHispanic: string;
    snapDirectRaceBlackNonHispanic: string;
    snapDirectRaceBlackHispanic: string;
    snapDirectRaceAsianNonHispanic: string;
    snapDirectRaceAsianHispanic: string;
    snapDirectRaceAmericanIndianNonHispanic: string;
    snapDirectRaceAmericanIndianHispanic: string;
    snapDirectRaceHawaiianNonHispanic: string;
    snapDirectRaceHawaiianHispanic: string;
    snapDirectRaceOtherNonHispanic: string;
    snapDirectRaceOtherHispanic: string;
    snapIndirectEstNumbReachedPsaRadio: string;
    snapIndirectEstNumbReachedPsaTv: string;
    snapIndirectEstNumbReachedArticles: string;
    snapIndirectEstNumbReachedGroceryStore: string;
    snapIndirectEstNumbReachedFairsParticipated: string;
    snapIndirectEstNumbReachedFairsSponsored: string;
    snapIndirectEstNumbReachedNewsletter: string;
    snapIndirectEstNumbReachedOther: string;
    snapIndirectMethodFactSheets?:boolean;
    snapIndirectMethodPosters?: boolean;
    snapIndirectMethodCalendars?: boolean;
    snapIndirectMethodPromoMaterial?: boolean;
    snapIndirectMethodWebsite?:boolean;
    snapIndirectMethodEmail?:boolean;
    snapIndirectMethodVideo?:boolean;
    snapIndirectMethodOther?:boolean;

}
export interface zzSnapEdSessionTypes{
    rID:number;
    fY?:number;
    orderID?:number;
    snapDirectSessionTypeName: string;
}
export interface zzSnapEdDeliverySite{
    rID:number;
    fY?:number;
    orderID?:number;
    snapDirectDeliverySiteName: string;
}

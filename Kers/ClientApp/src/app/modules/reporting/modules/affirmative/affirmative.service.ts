import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';


@Injectable()
export class AffirmativeService {

    private baseUrl = '/api/AffirmativeAction/';


    plan:AffirmativePlan;

    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}


    get(unitId:number = 0, fy:string = "0"){
        var url = this.baseUrl + unitId + '/' + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.plan = <AffirmativePlan>res.json())
                .catch(this.handleError);
    }
    add(affirmative){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(affirmative), this.getRequestOptions())
                    .map( res => {
                        this.plan = <AffirmativePlan> res.json();
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    getMakeupDiversityGroups(){
        var url = this.baseUrl + 'MakeupDiversityGroups';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <MakeupDiversityGroup[]>res.json())
                .catch(this.handleError);
    }

    getAdvisoryGroups(){
        var url = this.baseUrl + 'AdvisoryGroups';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <AdvisoryGroup[]>res.json())
                .catch(this.handleError);
    }

    getSummaryDiversity(){
        var url = this.baseUrl + 'SummaryDiversity';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SummaryDiversity[]>res.json())
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


export class AffirmativePlan{
    constructor(
        public id:number,
        public agents:string,
        public description:string,
        public goals:string,
        public strategies:string,
        public efforts:string,
        public success:string,
        public makeupValues: MakeupValue[],
        public summaryValues: SummaryValue[],
        public AffirmativeActionPlanId: number
    ){}
}

export class AdvisoryGroup{
    constructor(
        public id:number,
        public name:string
    ){}
}

export class MakeupDiversity{
    constructor(
        public id:number,
        public name:string,
        public type:string
    ){}
}
export class MakeupDiversityGroup{
    constructor(
        public id:number,
        public name:string,
        public render: boolean,
        public types: MakeupDiversity[]
    ){}
}

export class SummaryDiversity{
    constructor(
        public id:number,
        public name:string
    ){}
}

export class MakeupValue{
    constructor(
        public value:string,
        public diversityTypeId:number,
        public diversityType:MakeupDiversity,
        public groupTypeId:number,
        public GroupType:AdvisoryGroup
    ){}
}

export class MakeupValueForm{
    constructor(
        public value:MakeupValueFormValue,
        public diversityTypeId:number,
        public diversityType:MakeupDiversity,
        public groupTypeId:number,
        public GroupType:AdvisoryGroup
    ){}
}

export interface MakeupValueFormValue{
    value:string|number;
    disabled:boolean;
}



export class SummaryValue{
    constructor(
        public value: SummaryFormValue,
        public diversityTypeId:number,
        public diversityType:SummaryDiversity,
        public groupTypeId:number,
        public groupType:AdvisoryGroup
    ){}
}
export interface SummaryFormValue{
    value:string|number;
    disabled:boolean;
}


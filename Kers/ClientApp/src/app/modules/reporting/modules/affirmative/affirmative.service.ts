import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { PlanningUnit } from '../plansofwork/plansofwork.service';


@Injectable()
export class AffirmativeService {

    private baseUrl = '/api/AffirmativeAction/';
    private handleError: HandleError;


    plan:AffirmativePlan;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('AffirmativeService');
        }



    countiesWithoutPlan(districtId:number = 0, fy:string = "0", type:string = "district"):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'countieswithoutplan/' + districtId + '/' + fy + '/' + type;
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesWithoutPlan', []))
            );
    }

    countiesWithoutReport(districtId:number = 0, fy:string = "0", type:string = "district"):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'countieswithoutreport/' + districtId + '/' + fy + '/' + type;
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesWithoutReport', []))
            );
    }

    get(unitId:number = 0, fy:string = "0"):Observable<AffirmativePlan>{
        var url = this.baseUrl + unitId + '/' + fy;
        return this.http.get<AffirmativePlan>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countiesWithoutReport', <AffirmativePlan>{}))
            );
    }

    add(affirmative:AffirmativePlan):Observable<AffirmativePlan>{
        return this.http.post<AffirmativePlan>(this.location.prepareExternalUrl(this.baseUrl), affirmative)
            .pipe(
                tap(res => this.plan = res),
                catchError(this.handleError('add', affirmative))
            );            
    }

    getMakeupDiversityGroups():Observable<MakeupDiversityGroup[]>{
        var url = this.baseUrl + 'MakeupDiversityGroups';
        return this.http.get<MakeupDiversityGroup[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getMakeupDiversityGroups', []))
            );
    }

    getAdvisoryGroups():Observable<AdvisoryGroup[]>{
        var url = this.baseUrl + 'AdvisoryGroups';
        return this.http.get<AdvisoryGroup[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getMakeupDiversityGroups', []))
            );
    }

    getSummaryDiversity():Observable<SummaryDiversity[]>{
        var url = this.baseUrl + 'SummaryDiversity';
        return this.http.get<SummaryDiversity[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getSummaryDiversity', []))
            );
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


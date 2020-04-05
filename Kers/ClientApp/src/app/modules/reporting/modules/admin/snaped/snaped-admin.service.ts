import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { User } from '../../user/user.service';
import { Servicelog } from '../../servicelog/servicelog.service';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';






@Injectable()
export class SnapedAdminService {

    private baseUrl = '/api/SnapedAdmin/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('SnapedAdminService');
        }

    commited(fiscalYear:string = "0"):Observable<number>{
        var url = this.baseUrl + "committed/"+fiscalYear;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('commited', 0))
            );
    }
    reported(fiscalYear:string = "0"):Observable<number>{
        var url = this.baseUrl + "reported/"+fiscalYear;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('reported', 0))
            );
    }
    assistants(countyId:number = 0):Observable<User[]>{
        var url = this.baseUrl + "assistants/"+countyId;
        return this.http.get<User[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('assistants', []))
            );
    }

    assistantBudget(fiscalYear:string = "0"):Observable<number>{
        var url = this.baseUrl + "assistantbudget" + '/' + fiscalYear;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('assistantBudget', 0))
            );
    }


    addAssistantReimbursment( id: number, reimbursment:{} ):Observable<{}>{
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.post<{}>(this.location.prepareExternalUrl(url), reimbursment)
            .pipe(
                catchError(this.handleError('addAssistantReimbursment'))
            );
    }

    editAssistantReimbursment( id: number, reimbursment:{} ):Observable<{}>{
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.put<{}>(this.location.prepareExternalUrl(url), reimbursment)
            .pipe(
                catchError(this.handleError('editAssistantReimbursment'))
            );
    }
    deleteAssistantReimbursment(id:number):Observable<{}>{
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteAssistantReimbursment'))
            );
    }

    assistantReimbursments(id: number, fiscalYear: string = "0"):Observable<SnapBudgetReimbursementsNepAssistant[]>{
        var url = this.baseUrl + "assistantreimbursements/" + id + '/' + fiscalYear;
        return this.http.get<SnapBudgetReimbursementsNepAssistant[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('assistantReimbursments', []))
            );
    }

    csv(located:string): Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/SnapedData/' + located + '/data.csv'), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('assistantReimbursments', <Blob>{}))
            );
    }

    csvPost(located:string, data:{}): Observable<Blob>{
        return this.http.post(this.location.prepareExternalUrl(located), JSON.stringify(data), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('assistantReimbursments', <Blob>{}))
            );
    }


    countyBudget(countyId:number = 0, fy:string="0"):Observable<number>{
        var url = this.baseUrl + "countybudget/"+countyId + '/' + fy;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countyBudget', 0))
            );
    }

    addCountyReimbursment( countyId: number, reimbursment:{} ):Observable<{}>{
        var url = this.baseUrl + 'countyreimbursment/' + countyId;
        return this.http.post<{}>(this.location.prepareExternalUrl(url), reimbursment)
            .pipe(
                catchError(this.handleError('addCountyReimbursment'))
            );
    }

    editCountyReimbursment( id: number, reimbursment:{} ):Observable<{}>{
        var url = this.baseUrl + 'countyreimbursment/' + id;
        return this.http.put<{}>(this.location.prepareExternalUrl(url), reimbursment)
            .pipe(
                catchError(this.handleError('editCountyReimbursment'))
            );
    }
    deleteCountyReimbursment(id:number):Observable<{}>{
        var url = this.baseUrl + 'countyreimbursment/' + id;
        return this.http.delete<{}>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteCountyReimbursment'))
            );
    }

    countyReimbursments(countyId: number, fiscalYear: string = "0"):Observable<SnapBudgetReimbursementsCounty[]>{
        var url = this.baseUrl + "countyreimbursments/"+countyId + '/' + fiscalYear;
        return this.http.get<SnapBudgetReimbursementsCounty[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('countyReimbursments', []))
            );
    }

    updateCountyBudget(countyId:number, budget:SnapBudgetReimbursementsCounty):Observable<SnapBudgetReimbursementsCounty>{
        var url = this.baseUrl + 'countybudget/' + countyId;
        return this.http.put<SnapBudgetReimbursementsCounty>(this.location.prepareExternalUrl(url), budget)
            .pipe(
                catchError(this.handleError('updateCountyBudget', budget))
            );
    }

    getCustom(criteria:SnapedSearchCriteria):Observable<SnapSeearchResultsWithCount>{
        var url = this.baseUrl + 'getCustom/';
        return this.http.post<SnapSeearchResultsWithCount>(this.location.prepareExternalUrl(url), criteria)
            .pipe(
                catchError(this.handleError('getCustom',<SnapSeearchResultsWithCount>{}))
            );
    }
    GetCustomDataHeader():Observable<string[]>{
        var url = this.baseUrl + 'GetCustomDataHeader/';
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('GetCustomDataHeader', []))
            );
    }
    getCustomData(criteria:SnapedSearchCriteria):Observable<string[]>{
        var url = this.baseUrl + 'getCustomData/';
        return this.http.post<string[]>(this.location.prepareExternalUrl(url), criteria)
            .pipe(
                catchError(this.handleError('getCustomData',[]))
            );
    }
    
}

export interface SnapBudgetReimbursementsCounty{
    id:number;
    planningUnitId:number;
    byId:number;
    updatedById:number;
    amount:number;
    notes:string;
}

export interface SnapBudgetReimbursementsNepAssistant{
    id:number;
    byId:number;
    updatedById:number;
    toId:number;
    amount:number;
    notes:string;
}

export class SnapedSearchCriteria{
    start: string;
    end: string;
    search: string = "";
    type = "direct";
    order: string = 'dsc';
    congressionalDistrictId?:number;
    regionId?:number;
    areaId?:number;
    unitId?:number;
    skip:number = 0;
    take?:number;
}

export class SnapSearchResult{
    user: User;
    revision: Servicelog;
    unit: PlanningUnit
}
export class SnapSeearchResultsWithCount{
    results:SnapSearchResult[];
    resultsCount:number;
}

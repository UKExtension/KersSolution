import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams, ResponseContentType } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';





@Injectable()
export class SnapedAdminService {

    private baseUrl = '/api/SnapedAdmin/';


    constructor( private http:AuthHttp, private location:Location){}


    commited(fiscalYear:string = "0"){
        var url = this.baseUrl + "committed/"+fiscalYear;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }
    reported(fiscalYear:string = "0"){
        var url = this.baseUrl + "reported/"+fiscalYear;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }
    assistants(countyId:number = 0){
        var url = this.baseUrl + "assistants/"+countyId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }

    assistantBudget(){
        var url = this.baseUrl + "assistantbudget";
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <number>res.json())
            .catch(this.handleError);
    }






    addAssistantReimbursment( id: number, reimbursment:{} ){
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(reimbursment), this.getRequestOptions())
                    .map( res => <{}>res.json() )
                    .catch(this.handleError);
    }

    editAssistantReimbursment( id: number, reimbursment:{} ){
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(reimbursment), this.getRequestOptions())
            .map( res => {
                return <{}> res.json();
            })
            .catch(this.handleError);
    }
    deleteAssistantReimbursment(id:number){
        var url = this.baseUrl + 'assistantreimbursements/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    assistantReimbursments(id: number, fiscalYear: string = "0"){
        var url = this.baseUrl + "assistantreimbursements/" + id + '/' + fiscalYear;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SnapBudgetReimbursementsNepAssistant[]>res.json())
                .catch(this.handleError);
    }

    csv(located:string){
        return this.http.get(this.location.prepareExternalUrl('/api/SnapedData/' + located + '/data.csv'), { responseType: ResponseContentType.Blob })
        .map((res:Response) => {
            var pd = res.blob();
            return pd;
        })
        .catch(this.handleError);
    }

    csvPost(located:string, data:{}){
        return this.http.post(this.location.prepareExternalUrl(located), JSON.stringify(data), { responseType: ResponseContentType.Blob })
        .map((res:Response) => {
            var pd = res.blob();
            return pd;
        })
        .catch(this.handleError);
    }










    countyBudget(countyId:number = 0, fy:string="0"){
        var url = this.baseUrl + "countybudget/"+countyId + '/' + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json())
                .catch(this.handleError);
    }

    addCountyReimbursment( countyId: number, reimbursment:{} ){
        var url = this.baseUrl + 'countyreimbursment/' + countyId;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(reimbursment), this.getRequestOptions())
                    .map( res => <{}>res.json() )
                    .catch(this.handleError);
    }

    editCountyReimbursment( id: number, reimbursment:{} ){
        var url = this.baseUrl + 'countyreimbursment/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(reimbursment), this.getRequestOptions())
            .map( res => {
                return <{}> res.json();
            })
            .catch(this.handleError);
    }
    deleteCountyReimbursment(id:number){
        var url = this.baseUrl + 'countyreimbursment/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    countyReimbursments(countyId: number, fiscalYear: string = "0"){
        var url = this.baseUrl + "countyreimbursments/"+countyId + '/' + fiscalYear;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }

    updateCountyBudget(countyId:number, budget:{}){
        var url = this.baseUrl + 'countybudget/' + countyId;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(budget), this.getRequestOptions())
                    .map( res => {
                        return <SnapBudgetReimbursementsCounty> res.json();
                    })
                    .catch(this.handleError);
    }


    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
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

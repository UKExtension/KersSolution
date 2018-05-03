import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';
import { PlanningUnit } from '../user/user.service';


@Injectable()
export class IndicatorsService {

    private baseUrl = '/api/ProgramIndicator/';



    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}



        

    countiesWithoutIndicators(districtId:number = 0, fy:string = "0"):Observable<PlanningUnit[]>{
        var url = this.baseUrl + 'noindicatorscounties/' + districtId + '/' + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <PlanningUnit[]>res.json())
                .catch(this.handleError);
    }

    listIndicators(program:MajorProgram):Observable<Indicator[]>{
            var url = this.baseUrl + "indicatorsforprogram/" + program.id;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Indicator[]>res.json())
                .catch(this.handleError);
    }

    indicatorValues(program:MajorProgram):Observable<Indicator[]>{
            var url = this.baseUrl + "indicatorvalues/" + program.id;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <IndicatorValue[]>res.json())
                .catch(this.handleError);
    }

    updateValues(program:MajorProgram, vals:IndicatorValue[]){
        var url = this.baseUrl + "valuesupdate/" + program.id;
        
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(vals), this.getRequestOptions())
                .map(res => <IndicatorValue[]>res.json())
                .catch(this.handleError);

    }

    addIndicator(majorProgramId:number, indicator:Indicator){
        var url = this.baseUrl + majorProgramId;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(indicator), this.getRequestOptions())
                    .map( res => {
                        return <Indicator>res.json();
                    })
                    .catch(this.handleError);
    }

    updateIndicator(indicatorId:number, indicator:Indicator){
        var url = this.baseUrl + indicatorId;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(indicator), this.getRequestOptions())
                    .map( res => {
                        return <Indicator>res.json();
                    })
                    .catch(this.handleError);
    }

    deleteIndicator(indicatorId:number){
        var url = this.baseUrl + indicatorId;
        
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

export interface Indicator{
    id:number;
    question:string;
    order:number;
}

export interface IndicatorValue{
    programIndicatorId:number;
    value:number;
}


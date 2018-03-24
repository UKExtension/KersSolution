import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';


@Injectable()
export class PlansofworkService {

    private baseUrl = '/api/PlansOfWork/';


    maps:Map[];
    plans:PlanOfWork[];

    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}



    listPlans(fy:string = "0"){
            var url = this.baseUrl + "All/"+fy;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.plans = <PlanOfWork[]>res.json())
                .catch(this.handleError);
    }
    listPlansDetails(fy:string = "0"){
            var url = this.baseUrl + "AllDetails/" + 0 + "/" + fy;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.plans = <PlanOfWork[]>res.json())
                .catch(this.handleError);
    }
    plansForCounty(id:number, fy:string = "0"){
        var url = this.baseUrl + "AllDetails/" + id + "/" + fy;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.plans = <PlanOfWork[]>res.json())
                .catch(this.handleError);
    }
    planForRevision(id:number){
        var url = this.baseUrl + "planforrevision/"+id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Plan>res.json())
                .catch(this.handleError);
    }

    addPlan(plan:PlanOfWork){
        var url = this.baseUrl;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(plan), this.getRequestOptions())
                    .map( res => {
                        this.plans.push(<PlanOfWork> res.json());
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    
    updatePlan(id: number, plan:PlanOfWork){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(plan), this.getRequestOptions())
                    .map( res => {
                        return <PlanOfWork> res.json();
                    })
                    .catch(this.handleError);
    }

    deletePlan(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    listMaps(fy:string = "0"){
            var url = this.baseUrl + "mapsall/" + fy;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.maps = res.json())
                .catch(this.handleError);
    }

    addMap(map:Map){
        var url = this.baseUrl + 'map';
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(map), this.getRequestOptions())
                    .map( res => {
                        this.maps.push(<Map> res.json());
                        return res.json();
                    })
                    .catch(this.handleError);
    }

    isMapDeleteAllowed(id:number){
        var url = this.baseUrl + "maphasplan/" + id;
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    return <boolean>res.json();
                })
                .catch(this.handleError);
    }
    
    updateMap(id: number, map:Map){
        var url = this.baseUrl + 'map/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(map), this.getRequestOptions())
                    .map( res => {
                        return <Map> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteMap(id:number){
        var url = this.baseUrl + 'map/' + id;
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

export class Map{
    constructor(
        public id:number,
        public title:string,
        public fiscalYearId:number
    ){}
}

export class PlanOfWork{
    constructor(
        public id:number,
        public title:string,
        public agentsInvolved:string,
        public mp1:MajorProgram,
        public mp2:MajorProgram,
        public mp3:MajorProgram,
        public mp4:MajorProgram,
        public situation:string,
        public longTermOutcomes:string,
        public intermediateOutcomes:string,
        public initialOutcomes:string,
        public learning:string,
        public evaluation:string,
        public map:Map,
        public created:Date
    ){}
}
export class Plan{
    constructor(
        public id:number,
        public revisions:PlanOfWork[],
        public fiscalYear:FiscalYear,
        public planningUnit: PlanningUnit
    ){}
}


export class PlanningUnit{
    constructor(
        public id:number,
        public name: string
    ){}
}

import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class PlansofworkService {

    private baseUrl = '/api/PlansOfWork/';
    private handleError: HandleError;


    maps:Map[] = [];
    plans:PlanOfWork[] = [];

    constructor( 
        private http:HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('PlansofworkService');
        }



    listPlans(fy:string = "0"):Observable<PlanOfWork[]>{
            var url = this.baseUrl + "All/"+fy;
            return this.http.get<PlanOfWork[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('listPlans', []))
                );
    }

    dataSources():Observable<PlanOfWorkDataSource[]>{
        var url = this.baseUrl + "datasources";
        return this.http.get<PlanOfWorkDataSource[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('listPlans', []))
            );
}
    listPlansDetails(fy:string = "0", planningUnitId:number = 0):Observable<PlanOfWork[]>{
            var url = this.baseUrl + "AllDetails/" + planningUnitId  + "/" + fy;
            return this.http.get<PlanOfWork[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(res => this.plans = res),
                    catchError(this.handleError('listPlans', []))
                );
    }
    plansForCounty(id:number, fy:string = "0"):Observable<PlanOfWork[]>{
        var url = this.baseUrl + "AllDetails/" + id + "/" + fy;
            return this.http.get<PlanOfWork[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(res => this.plans = res),
                    catchError(this.handleError('plansForCounty', []))
                );
    }

    countiesWithoutPlans(id:number = 0, fy:string = "0", type:string = "district"):Observable<PlanningUnit[]>{
        var url = this.baseUrl + "noplanscounties/" + id + "/" + fy + "/" + type;
            return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('countiesWithoutPlans', []))
                );
    }

    planForRevision(id:number):Observable<Plan>{
        var url = this.baseUrl + "planforrevision/"+id;
        return this.http.get<Plan>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('listPlans', <Plan>{}))
            );
    }

    addPlan(plan:PlanOfWork, fy:string = "0"):Observable<PlanOfWork>{
        var url = this.baseUrl + fy;
        return this.http.post<PlanOfWork>(this.location.prepareExternalUrl(url), plan)
            .pipe(
                tap( res => this.plans.push( res) ),
                catchError(this.handleError('addPlan', <PlanOfWork>{}))
            );            
    }

    
    updatePlan(id: number, plan:PlanOfWork):Observable<PlanOfWork>{
        var url = this.baseUrl + id;
        return this.http.put<PlanOfWork>(this.location.prepareExternalUrl(url), plan)
            .pipe(
                catchError(this.handleError('updatePlan', <PlanOfWork>{}))
            );
    }

    deletePlan(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deletePlan'))
            );
    }

    listMaps(fy:string = "0"):Observable<Map[]>{
            var url = this.baseUrl + "mapsall/" + fy;
            return this.http.get<Map[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap( res => this.maps = res ),
                    catchError(this.handleError('listMaps', []))
                );
    }

    addMap(map:Map):Observable<Map>{
        var url = this.baseUrl + 'map';
        return this.http.post<Map>(this.location.prepareExternalUrl(url), map)
        .pipe(
            tap( res => this.maps.push( res )),
            catchError(this.handleError('addMap', <Map>{}))
        );
    }

    isMapDeleteAllowed(id:number):Observable<boolean>{
        var url = this.baseUrl + "maphasplan/" + id;
            return this.http.get<boolean>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('isMapDeleteAllowed', false))
                );
    }
    
    updateMap(id: number, map:Map):Observable<Map>{
        var url = this.baseUrl + 'map/' + id;
        return this.http.put<Map>(this.location.prepareExternalUrl(url), map)
            .pipe(
                catchError(this.handleError('updateMap', <Map>{}))
            );
    }

    deleteMap(id:number):Observable<{}>{
        var url = this.baseUrl + 'map/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteMap'))
            );
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
        public countySituation:string,
        public longTermOutcomes:string,
        public intermediateOutcomes:string,
        public initialOutcomes:string,
        public learning:string,
        public evaluation:string,
        public map:Map,
        public created:Date,
        public planOfWorkDataSourceSelections:PlanOfWorkDataSourceSelection[]
    ){}
}

export interface PlanOfWorkDataSource{
    id:number,
    name:string,
    order:number,
    active:boolean
}

export interface PlanOfWorkDataSourceSelection{
    planOfWorkDataSourceId:number,
    planOfWorkRevisionId:number
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

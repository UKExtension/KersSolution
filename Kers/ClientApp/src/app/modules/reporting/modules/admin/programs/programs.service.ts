import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import {FiscalYear} from '../fiscalyear/fiscalyear.service';


@Injectable()
export class ProgramsService {

    private baseUrl = '/api/initiative/';
    private handleError: HandleError;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ProgramsService');
        }



    listInitiatives(fy:string = "0"):Observable<StrategicInitiative[]>{
        var url = this.baseUrl + "All/" + fy;
        return this.http.get<StrategicInitiative[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('listInitiatives', []))
            );
    }

    categories():Observable<ProgramCategory[]>{
        var url = this.baseUrl + "category";
        return this.http.get<ProgramCategory[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('listInitiatives', []))
            );
    }

    addInitiative(initiative:StrategicInitiative, fiscalYear:FiscalYear):Observable<StrategicInitiative>{
        var url = this.baseUrl + fiscalYear.id;
        return this.http.post<StrategicInitiative>(this.location.prepareExternalUrl(url), initiative)
            .pipe(
                catchError(this.handleError('addInitiative', initiative))
            );
    }
    
    updateInitiative(id: number, initiative:StrategicInitiative):Observable<StrategicInitiative>{
        var url = this.baseUrl + id;
        return this.http.put<StrategicInitiative>(this.location.prepareExternalUrl(url), initiative)
            .pipe(
                catchError(this.handleError('updateInitiative', initiative))
            );
    }

    deleteInitiative(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteInitiative'))
            );
    }


    addProgram(initiative:StrategicInitiative, program:MajorProgram):Observable<MajorProgram>{
        var url = this.baseUrl + 'program/' + initiative.id;
        return this.http.post<MajorProgram>(this.location.prepareExternalUrl(url), program)
            .pipe(
                catchError(this.handleError('addProgram', program))
            );
    }
    
    updateProgram(id: number, program:MajorProgram):Observable<MajorProgram>{
        var url = this.baseUrl + 'program/' + id;
        return this.http.put<MajorProgram>(this.location.prepareExternalUrl(url), program)
            .pipe(
                catchError(this.handleError('updateProgram', program))
            );
    }

    deleteProgram(id:number):Observable<{}>{
        var url = this.baseUrl + 'program/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteProgram'))
            );
    }
    
}

export class StrategicInitiative{
    constructor(
        public id: number,
        public name: string,
        public pacCode: number,
        public order: number,
        public fiscalYear: FiscalYear,
        public programCategoryId: number,
        public programCategory: ProgramCategory,
        public majorPrograms: MajorProgram[]
    ){}
}

export class MajorProgram{
    constructor(
        public id: number,
        public shortName: string,
        public name: string,
        public pacCode: number,
        public order: number,
        public strategicInitiative: StrategicInitiative
    ){}
}

export class ProgramCategory{
    constructor(
        public id: number,
        public shortName: string,
        public name: string,
        public order: number
    ){}
}
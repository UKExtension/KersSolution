import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';
import {FiscalYear} from '../fiscalyear/fiscalyear.service';


@Injectable()
export class ProgramsService {

    private baseUrl = '/api/initiative/';

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private years = null;
    private programCategories: ProgramCategory[];

    constructor( private http:AuthHttp, private location:Location){}

    listInitiatives(){
            var url = this.baseUrl + "All";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>  res.json())
                .catch(this.handleError);
    }

    categories(){
        var url = this.baseUrl + "category";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.programCategories = <ProgramCategory[]>res.json())
                .catch(this.handleError);
        
    }

    addInitiative(initiative:StrategicInitiative){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(initiative), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
    }
    
    updateInitiative(id: number, initiative:StrategicInitiative){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(initiative), this.getRequestOptions())
                    .map( res => {
                        return <StrategicInitiative> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteInitiative(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }



    addProgram(initiative:StrategicInitiative, program:MajorProgram){
        var url = this.baseUrl + 'program/' + initiative.id;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(program), this.getRequestOptions())
                    .map( res => {
                        return res.json();
                    })
                    .catch(this.handleError);
    }
    
    updateProgram(id: number, program:MajorProgram){
        var url = this.baseUrl + 'program/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(program), this.getRequestOptions())
                    .map( res => {
                        return <MajorProgram> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteProgram(id:number){
        var url = this.baseUrl + 'program/' + id;
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

export class StrategicInitiative{
    constructor(
        public id: number,
        public name: string,
        public pacCode: number,
        public order: number,
        public fiscalYear: FiscalYear,
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
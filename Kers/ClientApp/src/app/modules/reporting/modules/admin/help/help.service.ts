import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';


@Injectable()
export class HelpService {

    private baseUrl = '/api/HelpContent/';

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private years = null;

    constructor( private http:AuthHttp, private location:Location){}


    /**********************************/
    // HELP CONTENT
    /**********************************/


    all(){
        var url = this.baseUrl + "All";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Help[]>res.json())
                .catch(this.handleError);
    }


    addHelp(help:Help){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(help), this.getRequestOptions())
                    .map( res => {
                        return <Help> res.json();
                    })
                    .catch(this.handleError);
    }
    updateHelp(id:number, help:Help){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(help), this.getRequestOptions())
                    .map( res => {
                        return <Help> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteHelp(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    /**********************************/
    // HELP CATEGORY
    /**********************************/

    allCategories(): Observable<HelpCategory[]> {
        var url = this.baseUrl + "allCategories";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <HelpCategory[]>res.json())
                .catch(this.handleError);
    }

    categoryChildren(id: Number) : Observable<HelpCategory[]>{
        var url = this.baseUrl + "childrenCategories/"+id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <HelpCategory[]>res.json())
                .catch(this.handleError);
    }

    addHelpCategory(helpCategory:HelpCategory, parentId:number){
        var url = this.baseUrl + 'newcategory/' + parentId;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(helpCategory), this.getRequestOptions())
                    .map( res => {
                        return <HelpCategory> res.json();
                    })
                    .catch(this.handleError);
    }

    updateHelpCategory(id:number, helpCategory: HelpCategory){
        var url = this.baseUrl + 'updatecategory/' + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(helpCategory), this.getRequestOptions())
                    .map( res => {
                        return <HelpCategory> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteHelpCategory(id:number){
        var url = this.baseUrl + 'deletecategory/' + id;
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

export class Help{
    constructor(
        public id: number,
        public title: string,
        public body: string,
        public categoryId: number,
        public employeePositionId?: number,
        public zEmpRoleTypeId?: number,
        public isContyStaff?: number
    ){}
}
export class HelpCategory{
    constructor(
        public id: number,
        public title: string,
        public description: string,
        public parent?: HelpCategory,
        public employeePositionId?: number,
        public zEmpRoleTypeId?: number,
        public isContyStaff?: number
    ){}
}
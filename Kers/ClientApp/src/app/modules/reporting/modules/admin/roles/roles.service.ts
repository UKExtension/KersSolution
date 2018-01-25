import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';


@Injectable()
export class RolesService {

    private rolesUrl = '/api/EmpRoles/';

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private rls = null;

    constructor( private http:AuthHttp, private location:Location){}

    listRoles(){
            var url = this.rolesUrl + "All";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => this.rls = res.json())
                .catch(this.handleError);
    }

    addRole(role){
        return this.http.post(this.location.prepareExternalUrl(this.rolesUrl), JSON.stringify(role), this.getRequestOptions())
                    .map( res => {
                        this.rls.push(<Role> res.json());
                        return res.json();
                    })
                    .catch(this.handleError);
    }
    
    updateRole(id: number, role:Role){
        var url = this.rolesUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(role), this.getRequestOptions())
                    .map( res => {
                        return <Role> res.json();
                    })
                    .catch(this.handleError);
    }

    deleteRole(id:number){
        var url = this.rolesUrl + id;
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

export class Role{
    constructor(
        public id: number,
        public enabled: boolean,
        public selfEnrolling: boolean,
        public shortTitle: string,
        public title: string,
        public description: string,
        public created: Date,
        public updated: Date
    ){}
}
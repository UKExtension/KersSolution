import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';


@Injectable()
export class RolesService {

    private rolesUrl = '/api/EmpRoles/';
    private handleError: HandleError;


    private rls:Role[] = null;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('RolesService');
        }

    listRoles():Observable<Role[]>{
            var url = this.rolesUrl + "All";
            return this.http.get<Role[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap( res => this.rls = res),
                    catchError(this.handleError('listRoles', []))
                );
    }

    addRole(role:Role):Observable<Role>{
        return this.http.post<Role>(this.location.prepareExternalUrl(this.rolesUrl), role)
            .pipe(
                tap( res => this.rls.push(res)),
                catchError(this.handleError('addRole', role))
            );
    }
    
    updateRole(id: number, role:Role):Observable<Role>{
        var url = this.rolesUrl + id;
        return this.http.put<Role>(this.location.prepareExternalUrl(url), role)
            .pipe(
                catchError(this.handleError('updateRole', role))
            );
    }

    deleteRole(id:number):Observable<{}>{
        var url = this.rolesUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('updateRole'))
            );
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
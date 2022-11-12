import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';


@Injectable({providedIn:'root'})
export class HelpService {

    private baseUrl = '/api/HelpContent/';
    private handleError: HandleError;


    constructor(  
        private location:Location,
        private http: HttpClient,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('HelpService');
        }


    /**********************************/
    // HELP CONTENT
    /**********************************/


    all():Observable<Help[]>{
        var url = this.baseUrl + "All";
        return this.http.get<Help[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('all', []))
            );
    }
    bayCategory(categoryId:number):Observable<Help[]>{
        var url = this.baseUrl + "bycategory/" + categoryId;
        return this.http.get<Help[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('by category', []))
            );
    }


    addHelp(help:Help): Observable<Help>{
        return this.http.post<Help>(this.location.prepareExternalUrl(this.baseUrl), help)
            .pipe(
                catchError(this.handleError('addHelp', help))
            );
    }
    updateHelp(id:number, help:Help): Observable<Help>{
        var url = this.baseUrl + id;
        return this.http.put<Help>(this.location.prepareExternalUrl(url), help)
            .pipe(
                catchError(this.handleError('updateHelp', help))
            );
    }

    deleteHelp(id:number): Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteHelp'))
            );
    }

    /**********************************/
    // HELP CATEGORY
    /**********************************/

    allCategories(): Observable<HelpCategory[]> {
        var url = this.baseUrl + "allCategories";
        return this.http.get<HelpCategory[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('allCategories', []))
            );
    }

    categoryChildren(id: Number) : Observable<HelpCategory[]>{
        var url = this.baseUrl + "childrenCategories/"+id;
        return this.http.get<HelpCategory[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('categoryChildren', []))
            );
    }

    addHelpCategory(helpCategory:HelpCategory, parentId:number): Observable<HelpCategory>{
        var url = this.baseUrl + 'newcategory/' + parentId;
        return this.http.post<HelpCategory>(this.location.prepareExternalUrl(url), JSON.stringify(helpCategory))
            .pipe(
                catchError(this.handleError('categoryChildren', helpCategory))
            );
    }

    updateHelpCategory(id:number, helpCategory: HelpCategory): Observable<HelpCategory>{
        var url = this.baseUrl + 'updatecategory/' + id;
        return this.http.put<HelpCategory>(this.location.prepareExternalUrl(url), JSON.stringify(helpCategory))
            .pipe(
                catchError(this.handleError('updateHelpCategory', helpCategory))
            );
    }

    deleteHelpCategory(id:number):Observable<{}>{
        var url = this.baseUrl + 'deletecategory/' + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteHelpCategory', []))
            );
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
        public parentId:number,
        public helpContents:Help[],
        public parent?: HelpCategory,
        public employeePositionId?: number,
        public zEmpRoleTypeId?: number,
        public isContyStaff?: number
    ){}
}
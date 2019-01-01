import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';




const FETCH_LATENCY = 400;

@Injectable()
export class NavigationService {

    private handleError: HandleError;
    constructor( 
            private http:HttpClient, 
            private location:Location,
            httpErrorHandler: HttpErrorHandler
            ) {
                this.handleError = httpErrorHandler.createHandleError('NavigationService');
            }

    nav(): Observable<NavSection[]>{

        var url =  "/api/nav/user";
        return this.http.get<NavSection[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('nav', []))
            );
    }

    
}
export class NavItem {
    constructor(
        public id: number = 0,
        public name: string,
        public route: string,
        public isRelative:boolean,
        public order:number,
        public employeePositionId?: number,
        public zEmpRoleTypeId?: number,
        public isContyStaff?: number
      ) { }
  }
  
  export class NavGroup {
      constructor (
          public id: number = 0,
          public name:string,
          public icon: string,
          public items: NavItem[],
          public isOpen: string = 'inactive',
          public order:number,
          public employeePositionId?: number,
          public zEmpRoleTypeId?: number,
          public isContyStaff?: number
          
      ){}
  }
  
  export class NavSection {
      constructor (
          public id: number = 0,
          public name: string,
          public groups: NavGroup[],
          public employeePositionId?: number,
          public zEmpRoleTypeId?: number,
          public isContyStaff?: number
      ){}
  }
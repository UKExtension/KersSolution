import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';


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
/*
    hasUrl( url: string){
        for(var key in this.items){
            if(this.items[key].route === url){
                return true;
            }
        }
        return false;
    }
    */
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
/*
const NAVIGATION: NavSection[] = [
    new NavSection(
        1,
        'KERS',
        [
            new NavGroup(
                1,
                'My Activity',
                'fa-edit',
                [
                    new NavItem(
                        1,
                        'Service Log',
                        '/reporting/activities/create'
                    ),
                    new NavItem(
                        2,
                        'SNAP-Ed Records',
                        '/reporting/list'
                    ),
                    new NavItem(
                        3,
                        'Program Indicators',
                        '/reporting/activities/indicators'
                    ),
                    new NavItem(
                        4,
                        'In-Service Training',
                        '/reporting/activities/inservice'
                    ),
                    new NavItem(
                        5,
                        'Success Stories',
                        '/reporting/activities/successstories'
                    ),
                    new NavItem(
                        6,
                        'Tax Exempt/Voluntiers',
                        '/reporting/activities/voluntiers'
                    )
                ]
            ),
            new NavGroup(
                2,
                'My Extension Office',
                'fa-home',
                [
                    new NavItem(
                        7,
                        'County Events',
                        '/home'
                    )
                ]
            ),
            new NavGroup(
                3,
                'My Profile',
                'fa-user',
                [
                    new NavItem(
                        8,
                        'Reporting Profile',
                        '/reporting/profile'
                    ),
                    new NavItem(
                        9,
                        'Personal Profile',
                        '/reporting/personal-profile'
                    )
                ]
            ),
            new NavGroup(
                3,
                'Administration',
                'fa-briefcase',
                [
                    new NavItem(
                        10,
                        'Navigation',
                        '/reporting/admin/navigation'
                    ),
                    new NavItem(
                        11,
                        'User Management',
                        '/reporting/admin/users'
                    ),
                    new NavItem(
                        12,
                        'Roles',
                        '/reporting/admin/roles/list'
                    )
                ]
            )
        ]
    )
];

*/
const FETCH_LATENCY = 400;

@Injectable()
export class NavigationService {

    constructor( private http:AuthHttp, private location:Location){}
/*
    getNavigation(){
        return new Promise<NavSection[]>(resolve => {
                setTimeout(() => { resolve(NAVIGATION); }, FETCH_LATENCY);
            });


        //return NAVIGATION;
    }
*/
    nav(){

        var url =  "/api/nav/user";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <NavSection[]> res.json())
                .catch(this.handleError);
    }

    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
    }
}
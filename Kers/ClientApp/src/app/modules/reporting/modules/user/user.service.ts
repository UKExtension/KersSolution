import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { Vehicle } from '../expense/vehicle/vehicle.service';
import {TrainingEnrollment} from '../training/training';


@Injectable()
export class UserService {

    private baseUrl = '/api/User/';

    private usr:User;
    private handleError: HandleError;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('UserService');
        }


    current():Observable<User>{
        if(this.usr == null){
            var url = this.baseUrl + "current";
            return this.http.get<User>(this.location.prepareExternalUrl(url))
                .pipe(
                    tap(
                        res =>
                        {
                            this.usr = <User>res
                        }
                    ),
                    catchError(this.handleError('current', <User>{}))
                );
                    
        }else{
            return of(this.usr);
        }
    }

    currentUserHasAnyOfTheRoles(roles:string[]):Observable<boolean>{
        return this.current()
            .pipe(
                map(
                    res => {
                        return res.roles.some( 
                                role => roles.includes( role.zEmpRoleType.shortTitle ) 
                            );
                    }
            )
        );
        
        
    }



    getCustom(searchParams?:{}) : Observable<User[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<User[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}):Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }

    byId(id:number):Observable<User>{
        var url = this.baseUrl + "id/" + id;
        return this.http.get<User>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byId', <User>{}))
            );
        
    }

    InServiceEnrolment(id:number, fy:string){
        var url = this.baseUrl + "InServiceEnrolment/" + id + "/" + fy;
        return this.http.get(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('InServiceEnrolment', <User>{}))
            );
    }

    trainingsEnrolment(id:number, year:number):Observable<TrainingEnrollment[]>{
        var url = this.baseUrl + "TrainingsEnrolment/" + id + "/" + year;
        return this.http.get<TrainingEnrollment[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('InServiceEnrolment', []))
            );
    }

    usersWithRole(role:string):Observable<User[]>{
        var url = this.baseUrl + "userswithrole/" + role;
        return this.http.get<User[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('usersWithRole', []))
            );
    }


    startDate(linkBlueId:string):Observable<Date>{
        var url = this.baseUrl + "startdate/" + linkBlueId;
        return this.http.get<Date>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('startDate', <Date>{}))
            );
    }

    unset(){
        this.usr = null;
    }

    user(rprtProfileId:number):Observable<User>{
        var url = this.baseUrl + rprtProfileId;
        return this.http.get<User>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('user', <User>{}))
            );
    }

    add( user:User ):Observable<User>{
        var url = this.baseUrl;
        return this.http.post<User>(this.location.prepareExternalUrl(url), user)
            .pipe(
                catchError(this.handleError('add', <User>{}))
            );
    }

    update(id:number, user:User ):Observable<User>{
        var url = this.baseUrl + id;
        return this.http.put<User>(this.location.prepareExternalUrl(url), user)
            .pipe(
                tap( _ => this.usr = null),
                catchError(this.handleError('update', <User>{}))
            );  
        
        
    }

    tagsAutocomplete(text: string){
        const url = this.baseUrl + 'tags?q=' + text;
        return this.http.get(url)
            .pipe(
                catchError(this.handleError('tagsAutocomplete'))
            );
    }

    filenameForImageId(id:number){
        return this.http.get(this.location.prepareExternalUrl('image/id/'+id))
            .pipe(
                catchError(this.handleError('filenameForImageId'))
            );
    }

    extensionPositions():Observable<ExtensionPosition[]>{
        var url = this.baseUrl + "positions";
        return this.http.get<ExtensionPosition[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('extensionPositions', []))
            );
    }


    specialties():Observable<Specialty[]>{
        var url = this.baseUrl + "specialties";
        return this.http.get<Specialty[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('specialties', []))
            );
    }

    locations():Observable<GeneralLocation[]>{
        var url = this.baseUrl + "locations";
        return this.http.get<GeneralLocation[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }
    units():Observable<PlanningUnit[]>{
        var url = this.baseUrl + "units";
        return this.http.get<PlanningUnit[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('units', []))
            );
    }
    instititutions():Observable<Institution[]>{
        var url = this.baseUrl + "institutions";
        return this.http.get<Institution[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('instititutions', []))
            );
    }

    socialConnections():Observable<SocialConnectionType[]>{
        var url = this.baseUrl + "connections";
        return this.http.get<SocialConnectionType[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('socialConnections', []))
            );
    }

    linkBlueExists(linkBlueId:string):Observable<{}|null>{
        var url = this.baseUrl + "isItExists/" + linkBlueId;
        return this.http.get<{}|null>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('linkBlueExists'))
            );
    }

    personIdExists(personId:string):Observable<{}|null>{
        var url = this.baseUrl + "isPersonIdExists/" + personId;
        return this.http.get<{}|null>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('personIdExists'))
            );
    }

    unitEmployees( unitId:number = 0):Observable<User[]>{
        var url = this.baseUrl + "unitemployees/" + unitId;
        return this.http.get<User[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('unitEmployees', [] ))
            );
    }
    private addParams(params:{}){
        let searchParams = {};
        for(let p in params){
            searchParams[p] = params[p];
        }
        return  {params: searchParams};
    }
    
}


export class User{
    constructor(
        public id:number,
        public classicReportingProfileId: number,
        public specialties:UserSpecialty[],
        public rprtngProfile: ReportingProfile,
        public personalProfile: PersonalProfile,
        public extensionPositionId: number,
        public extensionPosition: ExtensionPosition,
        public created:Date,
        public updated:Date,
        public lastLogin:Date,
        public roles:RoleConnection[]
    ){}
}

export interface RoleConnection{
    zEmpRoleTypeId:number
    zEmpRoleType:zEmpRoleType
}

export interface zEmpRoleType{
    id:number,
    enabled:boolean,
    shortTitle:string,
    title:string,
    description:string
}

export class ReportingProfile{
    constructor(
        public id:number,
        public linkBlueId:string,
        public personId:string,
        public name:string,
        public email:string,
        public emailAlias:string,
        public enabled:boolean,
        public generalLocationId:number,
        public generalLocation:GeneralLocation,
        public planningUnitId:number,
        public planningUnit:PlanningUnit,
        public institutionId: number,
        public institution: Institution
    ){}
}

export class PersonalProfile{
    constructor(
        public id:number,
        public firstName: string,
        public lastName: string,
        public professionalTitle: string,
        public officePhone: string,
        public mobilePhone: string,
        public officeAddress:string,
        public timeZoneId: string,
        public interests:InterestProfile[],
        public socialConnections: SocialConnection[],
        public bio:string,
        public uploadImage: Image
    ){}
}

export class Image{
    public id:number;
    public uploadFile:UploadFile;
    public name:string;
}

export class UploadFile{
    public id:number;
    public name:string;
}

export class ExtensionPosition{
    constructor(
        public id:number,
        public code:string,
        public title:string,
        public description:string
    ){}
}

export class SocialConnection{
    constructor(
        public personalProfile:PersonalProfile,
        public socialConnectionTypeId:number,
        public socialConnectionType:SocialConnectionType,
        public identifier: string
    ){}
}

export class SocialConnectionType{
    constructor(
        public id:number,
        public name:string,
        public icon:string,
        public url:string,
        public description:string
    ){}
}

export class InterestProfile{
    constructor(
        public id:number,
        public interest:Interest,
        public personalProfile: PersonalProfile
    ){}
}
export class Interest{
    constructor(
        public id:number,
        public name: string
    ){}
}



export class GeneralLocation{
    constructor(
        public id:number,
        public code:string,
        public name:string,
        public order:number
    ){}
}

export class Institution{
    constructor(
        public id:number,
        public code:string,
        public order:number,
        public name:string
    ){}
}

export class PlanningUnit{
    constructor(
        public id:number,
        public name:string,
        public fullName:string,
        public phone:string,
        public code:string,
        public address:string,
        public zip:string,
        public city:string,
        public webSite:string,
        public email:string,
        public timeZoneId:string,
        public reportsExtension:boolean,
        public vehicles:Vehicle[],
        public fipsCode?:number,
        public districtId?:number,
        public geoFeature?:string,
        public population?:number
    ){}
}

export class UserSpecialty{
    constructor(
        public kersUserId: number,
        public specialtyId: number,
        public kersUser?:User,
        public specialty?:Specialty
    ){}
}

export class Specialty{
    constructor(
        public id:number,
        public name:string,
        public code:string,
        public description:string
    ){}
}


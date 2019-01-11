import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import {Profile} from '../../../components/reporting-profile/profile.service';
import {Role} from '../roles/roles.service';

@Injectable()
export class UsersService {

    private baseUrl = '/api/users/';
    private handleError: HandleError;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('UsersService');
        }

    byProfile(profile:Profile):Observable<KersUser>{
        var url = this.baseUrl + 'byprofile/' + profile.id;
        return this.http.get<KersUser>(this.location.prepareExternalUrl(url))
            .pipe(
                map( res => 
                    {
                        var user = res;
                        user.reportingProfile = profile;
                        return user;
                    }
                ),
                catchError(this.handleError('byProfile', <KersUser>{}))
            );
    }

    byProfileId(id:string):Observable<KersUser>{
        var url = this.baseUrl + 'byprofile/' + id;
        return this.http.get<KersUser>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byProfileId', <KersUser>{}))
            );
    }

    byId(id:string):Observable<KersUser>{
        var url = this.baseUrl + id;
        return this.http.get<KersUser>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byProfileId', <KersUser>{}))
            );
    }

    roleIds(user:KersUser):Observable<number[]>{
        var url = this.baseUrl + 'roleids/' + user.id;
        return this.http.get<number[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('roleIds', []))
            );
    }

    positions():Observable<Position[]>{
        var url = this.baseUrl + 'positions';
        return this.http.get<Position[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('positions', []))
            );
    }

    updateRoles(userId:number, roleIds:number[]):Observable<Role[]>{
        var url = this.baseUrl + 'updateroles/' + userId;
        var rls = [];
        for(var i=0; i < roleIds.length; i++){
            var r = {};
            r["zEmpRoleType"] = {"id": roleIds[i]};
            rls.push(r);
        }
        return this.http.put<Role[]>(this.location.prepareExternalUrl(url), rls)
            .pipe(
                catchError(this.handleError('roleIds', []))
            );
    }

    updatePersonalProfile(id:number, profile:PersonalProfile):Observable<PersonalProfile>{
        var url = this.baseUrl +'personal/'+ id;
        return this.http.put<PersonalProfile>(this.location.prepareExternalUrl(url), profile)
            .pipe(
                catchError(this.handleError('roleIds', profile))
            );
    }

}


export class KersUser{
    constructor(
        public id:number,
        public reportingProfile:Profile,
        public personalProfile:PersonalProfile,
        public roles:Role[]
    ){}
}

export class PersonalProfile{
    constructor(
        public id:number,
        public firstName: string,
        public lastName: string,
        public officePhone: string,
        public mobilePhone: string,
        public interests:InterestProfile[],
        public socialConnections: SocialConnection[],
        public bio:string,
        public uploadImage: Image
    ){}
}

export class Image{
    public uploadFile: UploadFile
}

export class UploadFile{
    public name
}

export class Position{
    constructor(
        public id:number,
        public code:string,
        public title:string,
        public description:string,
        public order?:number,
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
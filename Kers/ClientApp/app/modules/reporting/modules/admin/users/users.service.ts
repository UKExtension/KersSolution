import { Injectable} from '@angular/core';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import {Location} from '@angular/common';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../../authentication/auth.http';
import {Profile} from '../../../components/reporting-profile/profile.service';
import {Role} from '../roles/roles.service';

@Injectable()
export class UsersService {

    private baseUrl = '/api/users/';

    constructor( 
        private http:AuthHttp, 
        private location:Location
        )
    {

    }

    byProfile(profile:Profile){
        var url = this.baseUrl + 'byprofile/' + profile.id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var user = <KersUser>res.json();
                user.reportingProfile = profile;
                return user;
            })
            .catch(this.handleError);
    }

    byProfileId(id:string){
        var url = this.baseUrl + 'byprofile/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var user = <KersUser>res.json();
                return user;
            })
            .catch(this.handleError);
    }

    byId(id:string){
        var url = this.baseUrl + id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var user = <KersUser>res.json();
                return user;
            })
            .catch(this.handleError);
    }

    roleIds(user:KersUser){
        var url = this.baseUrl + 'roleids/' + user.id;
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var roleids = <number[]>res.json();
    
                return roleids;
            })
            .catch(this.handleError);
    }

    positions(){
        var url = this.baseUrl + 'positions';
        return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => {
                var positions = <Position[]>res.json();
                return positions;
            })
            .catch(this.handleError);
    }

    updateRoles(userId:number, roleIds:number[]){
        var url = this.baseUrl + 'updateroles/' + userId;
        var rls = [];
        for(var i=0; i < roleIds.length; i++){
            var r = {};
            r["zEmpRoleType"] = {"id": roleIds[i]};
            rls.push(r);
        }
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(rls), this.getRequestOptions())
            .map(res => {
                return true;
            })
            .catch(this.handleError);
    }

    updatePersonalProfile(id:number, profile:PersonalProfile){
        var url = this.baseUrl +'personal/'+ id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(profile), this.getRequestOptions())
            .map(response => {
                var profile = <PersonalProfile>response.json();
                return profile;
            })
            .catch(this.handleError)
    }

    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
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
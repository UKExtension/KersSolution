import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {Role} from '../admin/roles/roles.service';


@Injectable()
export class UserService {

    private baseUrl = '/api/User/';

    private usr:User;

    private pUnits = null;
    private pstns = null;
    private lctns = null;
    private years = null;

    constructor( private http:AuthHttp, private location:Location){}



    current():Observable<User>{
        if(this.usr == null){
            var url = this.baseUrl + "current";
            return this.http.get(this.location.prepareExternalUrl(url))
                    .map(res =>{ 
                        this.usr = <User>res.json();
                        return this.usr;
                    })
                    .catch(this.handleError);
        }else{
            return Observable.of(this.usr);
        }
    }



    getCustom(searchParams?:{}) : Observable<User[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response =>  <User[]>response.json() )
            .catch(this.handleError);
    }

    getCustomCount(searchParams?:{}){
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }

    byId(id:number):Observable<User>{

        var url = this.baseUrl + "id/" + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    return  <User>res.json();
                })
                .catch(this.handleError);
        
    }

    InServiceEnrolment(id:number){
        var url = this.baseUrl + "InServiceEnrolment/" + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    return  res.json();
                })
                .catch(this.handleError);
    }


    startDate(linkBlueId:string):Observable<Date>{
        var url = this.baseUrl + "startdate/" + linkBlueId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    return  <Date>res.json();
                })
                .catch(this.handleError);
    }

    unset(){
        this.usr = null;
    }

    user(rprtProfileId:number):Observable<User>{
        var url = this.baseUrl + rprtProfileId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <User>res.json())
                .catch(this.handleError);
    }

    add( user:User ):Observable<User>{
        var url = this.baseUrl;
        return this.http.post(this.location.prepareExternalUrl(url), JSON.stringify(user), this.getRequestOptions())
                .map(res => <User>res.json())
                .catch(this.handleError);
    }

    update(id:number, user:User ):Observable<User>{
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(user), this.getRequestOptions())
                .map(res =>{ 
                    this.usr = null;
                    return <User>res.json();
                })
                .catch(this.handleError);
    }

    tagsAutocomplete(text: string){
        const url = this.baseUrl + 'tags?q=' + text;
        return this.http
            .get(url)
            .map(data => data.json());
    }

    filenameForImageId(id:number){
        return this.http.get(this.location.prepareExternalUrl('image/id/'+id))
                    .map(res => res)
                    .catch(this.handleError);
    }

    extensionPositions():Observable<ExtensionPosition[]>{
        var url = this.baseUrl + "positions";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <ExtensionPosition[]>res.json())
                .catch(this.handleError);
    }


    specialties():Observable<Specialty[]>{
        var url = this.baseUrl + "specialties";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Specialty[]>res.json())
                .catch(this.handleError);
    }

    locations():Observable<GeneralLocation[]>{
        var url = this.baseUrl + "locations";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <GeneralLocation[]>res.json())
                .catch(this.handleError);
    }
    units():Observable<PlanningUnit[]>{
        var url = this.baseUrl + "units";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <PlanningUnit[]>res.json())
                .catch(this.handleError);
    }
    instititutions():Observable<Institution[]>{
        var url = this.baseUrl + "institutions";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Institution[]>res.json())
                .catch(this.handleError);
    }

    socialConnections():Observable<SocialConnectionType[]>{
        var url = this.baseUrl + "connections";
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <SocialConnectionType[]>res.json())
                .catch(this.handleError);
    }

    linkBlueExists(linkBlueId:string):Observable<{}|null>{
        var url = this.baseUrl + "isItExists/" + linkBlueId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => < {} | null >res.json())
                .catch(this.handleError);
    }

    personIdExists(personId:string):Observable<{}|null>{
        var url = this.baseUrl + "isPersonIdExists/" + personId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => < {} | null >res.json())
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
        public reportsExtension:boolean,
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


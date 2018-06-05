import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram } from '../admin/programs/programs.service';
import {PlanOfWork} from '../plansofwork/plansofwork.service';
import { User } from "../user/user.service";


@Injectable()
export class StoryService {

    private baseUrl = '/api/story/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}

    byId(id:number):Observable<Story>{

        var url = this.baseUrl + "id/" + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    return  <Story>res.json();
                })
                .catch(this.handleError);
        
    }

    getCustom(searchParams?:{}) : Observable<Story[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response =>  <Story[]>response.json() )
            .catch(this.handleError);
    }

    getCustomCount(searchParams?:{}){
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.getBy(this.location.prepareExternalUrl(url), searchParams)
            .map(response => response.json())
            .catch(this.handleError);
    }
    
    author(storyId:number):Observable<User>{
        var url = this.baseUrl + "author/" + storyId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res =>{ 
                    return  <User>res.json();
                })
                .catch(this.handleError);
    }
    
    add( story:Story ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(story), this.getRequestOptions())
                    .map( res => <Story>res.json() )
                    .catch(this.handleError);
    }
    
    outcome():Observable<StoryOutcome[]>{
        var url = this.baseUrl + 'outcome';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <StoryOutcome[]>res.json())
                .catch(this.handleError);
    }

    latestByUser(userId:number, amount:number = 23, fiscalYearName="0"):Observable<Story[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount + '/' + fiscalYearName;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Story[]>res.json())
                .catch(this.handleError);
    }

    perDay(userId:number, start:Date, end:Date){
        var url = this.baseUrl + 'perDay/'+ userId + '/' + start.toISOString() + '/' + end.toISOString() ;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <[{}]>res.json())
                .catch(this.handleError);
    }

    latest(skip:number = 0, take:number = 5){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Story[]>res.json() )
                .catch(this.handleError);
    }
    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    delete(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    update(id:number, story:Story){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(story), this.getRequestOptions())
                    .map( res => {
                        return <Story> res.json();
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

export interface Story{
    id:number;
    title:string;
    story:string;
    isSnap:boolean;
    majorProgramId:number;
    majorProgram:MajorProgram;
    planOfWorkId:number;
    planOfWork:PlanOfWork;
    storyOutcomeId:number;
    storyOutcome:StoryOutcome;
    storyImages:StoryImage[];
    created: Date;
}

export interface StoryImage{
    uploadImageId:number;
    uploadImage:Image;
}
export interface Image{
    uploadFile: File;
}
export interface File{
    name:string;
}

export interface StoryOutcome{
    id:number;
    name:string;
}

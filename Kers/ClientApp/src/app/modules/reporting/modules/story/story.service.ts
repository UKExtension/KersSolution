import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {MajorProgram } from '../admin/programs/programs.service';
import {PlanOfWork} from '../plansofwork/plansofwork.service';
import { User } from "../user/user.service";
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class StoryService {

    private baseUrl = '/api/story/';
    private handleError: HandleError;

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('SuccessStoryService');
        }

    byId(id:number):Observable<Story>{

        var url = this.baseUrl + "id/" + id;
        return this.http.get<Story>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byId', <Story>{}))
            );
    }

    getCustom(searchParams?:{}) : Observable<Story[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<Story[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
    }

    getCustomCount(searchParams?:{}): Observable<number>{
        var url = this.baseUrl + "GetCustomCount/";
        return this.http.get<number>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustomCount', 0))
            );
    }
    
    author(storyId:number):Observable<User>{
        var url = this.baseUrl + "author/" + storyId;
        return this.http.get<User>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('author', <User>{}))
            );
    }
    
    add( story:Story ):Observable<Story>{
        return this.http.post<Story>(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(story))
                .pipe(
                    catchError(this.handleError('add', <Story>{}))
                );
    }
    
    outcome():Observable<StoryOutcome[]>{
        var url = this.baseUrl + 'outcome';
        return this.http.get<StoryOutcome[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('outcome', []))
                );
    }

    latestByUser(userId:number, amount:number = 23, fiscalYearName="0"):Observable<Story[]>{
        var url = this.baseUrl + 'latestbyuser/'+ userId + '/' + amount + '/' + fiscalYearName;
        return this.http.get<Story[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('latestByUser', []))
                );
    }

    perDay(userId:number, start:Date, end:Date):Observable<{}[]>{
        var url = this.baseUrl + 'perDay/'+ userId + '/' + start.toISOString() + '/' + end.toISOString() ;
        return this.http.get<{}[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('perDay', []))
            );
    }

    latest(skip:number = 0, take:number = 5):Observable<Story[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Story[]>(this.location.prepareExternalUrl(url))
                .pipe(
                    catchError(this.handleError('latest', []))
                );
    }
    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('num', 0))
            );
    }

    delete(id:number): Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }

    update(id:number, story:Story):Observable<Story>{
        var url = this.baseUrl + id;
        return this.http.put<Story>(this.location.prepareExternalUrl(url), JSON.stringify(story))
                .pipe(
                    catchError(this.handleError('update', story))
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

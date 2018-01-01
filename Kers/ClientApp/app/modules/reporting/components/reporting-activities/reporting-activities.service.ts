import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import {AuthHttp} from '../../../authentication/auth.http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {Http, Response, Headers, RequestOptions } from '@angular/http';


@Injectable ()
export class ReportingActivitiesService {
    private baseUrl = '/api/Activity/';
    
    private pcs = null;
    private dayChoices = null;
    public pacGroups: PacGroup[];

    constructor( private http:AuthHttp, private location:Location){

    }


    pacs(){
        if(this.pacGroups == null){
            var url = this.baseUrl + "Pacs";
            return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                   this.pacGroups = this.processPacs(res.json());
                   return this.pacGroups;
                })
                .catch(this.handleError);
        }else{
            return Observable.of(this.pacGroups);
        }
    }

    days(){
        if( this.dayChoices == null ){
            var url = this.baseUrl + "Days";
            return this.http.get(this.location.prepareExternalUrl(url))
                        .map( res => res.json())
                        .catch( this.handleError);
        }else{
            return Observable.of(this.dayChoices);
        }
    }

    private processPacs(rawPacs){
        var items: PacItem[] = new Array<PacItem>();
        var group: PacGroup;
        var groups: PacGroup[] = new Array<PacGroup>();
        rawPacs.forEach(element => {
            if(element.rptCodeType == 1){
                if(items.length != 0){
                    group.pacItems = items;
                    groups.push(group);
                    items = new Array<PacItem>();
                }
                group = <PacGroup>element;
            }else{
                items.push(this.fixItemTitle(<PacItem>element));
            }
        });
        group.pacItems = items;
        groups.push(group);
        return this.pacGroups = groups;
    }


    private fixItemTitle(item:PacItem):PacItem{

        var titleParts = item.pacTitle.split(' - ');
        item.pacTitle = titleParts[1]+" - "+titleParts[0]+"";

        return item;
    }

    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
    }
    
}


export class PacGroup{
    constructor(
        public pacID: number,
        public fy: number,
        public cesProgCategory: number,
        public cesProgCategoryNameShort: string,
        public pacCodeID: number,
        public pacTitle: string,
        public pacItems: PacItem[]
    ){}
}

export class PacItem{
    constructor(
        public pacID: number,
        public fy: number,
        public cesProgCategory: number,
        public cesProgCategoryNameShort: string,
        public pacCodeID: number,
        public pacTitle: string
    ){}
}

export class zActivity
    {
        constructor(
            public id: number,
            public rDT: Date,
            public FY: number,
            public instID: string,
            public planningUnitID: string,
            public personID: string,
            public personName: string,
            public activityDate: string,
            public pacID1: number,
            public activityTitle: string,
            public activityDescription: string,
            public notPresent: boolean,
            public night: boolean,
            public weekend: boolean,
            public notExtensionSponsored: boolean,
            public multiState: boolean,
            public MS4: boolean,
            public activityDays: string,
            public activityHours: string,
            public raceWhite: string,
            public raceBlack: string,
            public raceAsian: string,
            public raceNativeAmerican: string,
            public raceOther: string,
            public raceNotDetermined: string,
            public ethHispanic: string,
            public genderFemale: string,
            public grpVolunteers: string,
            public grpYouth: string,
            public grpIndirect: string
        ){}
    
}

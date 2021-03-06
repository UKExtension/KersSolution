import { Component, Input } from '@angular/core';
import {StoryService, Story} from '../story.service';
import { Observable } from "rxjs";
import { User, UserService } from "../../user/user.service";
import {Location} from '@angular/common';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'success-story-list',
    template: ` 
    <div *ngIf="author">
        <div *ngIf="(otherStories | async)?.length == 0 ">No success stories submitted.</div>
         <success-story-display-list [stories]="otherStories"></success-story-display-list>
    </div>
    `
})
export class StoryReportsListComponent { 


    @Input() author:User;
    @Input() fiscalYear:FiscalYear;
    otherStories: Observable<Story[]>
    errorMessage: string;

    constructor(  
        private service: StoryService,
        private userService: UserService,
        private location: Location
    )   
    {}

    ngOnInit(){ 
        if(this.author == null){
            this.userService.current().subscribe(
                res =>{
                    this.author = res;
                    this.otherStories=this.service.latestByUser(this.author.id, 30, this.fiscalYear.name);
                } 
            )
        }else{
            this.otherStories=this.service.latestByUser(this.author.id, 30, this.fiscalYear.name);
        }
        

        
    }

    htmlToPlaintext(text) {
        var result = text ? String(text).replace(/<[^>]+>/gm, '') : ''
        return result.substring(0, 200);
    }

    day(dateString){
        return new Date(dateString).getDay();
    }


    month(dateString){


        return new Date(dateString).toLocaleString("en-us", { month: "long" });
    }

    externalUrl(url){
        return this.location.prepareExternalUrl(url);
    }

    
}
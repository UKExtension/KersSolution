import { Component, Input } from '@angular/core';
import {StoryService, Story} from '../story.service';
import { Observable } from "rxjs";
import { User } from "../../user/user.service";
import {Location} from '@angular/common';

@Component({
    selector: 'success-story-full',
    template: `
    <div *ngIf="story">
        <success-story-display [story]="story"></success-story-display>
        <div class="row">
            <div class="col-xs-12">
                <br>
                    <div class="ln_solid"></div><br>
                <div *ngIf="author | async">
                    <h2>More stories by  {{(author | async)?.personalProfile.firstName}} {{(author | async)?.personalProfile.lastName}}</h2>
                    
                    <!-- start more stories -->
                    <ul class="messages">
                        <li *ngFor="let story of otherStories | async">
                        <img src="{{externalUrl('/image/crop/100/100/' + story.storyImages[0].uploadImage.uploadFile.name)}}" class="avtr" alt="Avatar" *ngIf="story.storyImages.length > 0">
                        <div class="message_date">
                            <h3 class="date text-info">{{day(story.created)}}</h3>
                            <p class="month">{{month(story.created)}}</p>
                        </div>
                        <div class="message_wrapper">
                            <h4 class="heading">{{story.title}}</h4>
                            <blockquote class="message" [innerHtml]="htmlToPlaintext(story.story)"></blockquote>
                            <br>
                            <p class="url">
                            <span class="fs1 text-info" aria-hidden="true" data-icon=""></span>
                            <a [routerLink]="['/reporting/story', story.id]" (click)="clicked()"><i class="fa fa-align-left"></i> Full Story</a>
                            </p>
                        </div>
                        </li>
                        

                    </ul>
                    <!-- end more stories -->
                    
                
                </div>
            </div>
        </div>
    </div>
    
    `,
    styles: [`
        ul.messages li img.avtr, img.avtr {
                height: 92px;
                width: 92px;
                float: left;
                display: inline-block;
                -webkit-border-radius: 2px;
                -moz-border-radius: 2px;
                border-radius: 2px;
                padding: 2px;
                background: #f7f7f7;
                border: 1px solid #e6e6e6;
            }
            ul.messages li .message_wrapper {
                margin-left: 105px;
                margin-right: 45px;
            }
    `]
})
export class StoryReportsFullComponent { 

    @Input() story: Story;
    author:Observable<User>;
    otherStories: Observable<Story[]>
    errorMessage: string;

    constructor(  
        private service: StoryService,
        private location: Location
    )   
    {}

    ngOnInit(){ 
        this.author = this.service.author(this.story.id);
        this.author.subscribe(
                res => {
                    this.otherStories=this.service.latestByUser(res.id);
                    
                }
            ,
            err => this.errorMessage = <any> err
        );
    }

    clicked(){
        window.scrollTo(0,0);
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
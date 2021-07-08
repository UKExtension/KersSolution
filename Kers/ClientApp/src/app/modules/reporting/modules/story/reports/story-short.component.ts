import { Component, Input } from '@angular/core';
import {StoryService, Story} from '../story.service';
import {Location} from '@angular/common';

@Component({
    selector: '[success-story-short]',
    template: ` 
                   
                        <div *ngIf="!fullDisplay">
                            <img src="{{externalUrl('/image/crop/100/100/' + story.storyImages[0].uploadImage.uploadFile.name)}}" class="avtr" alt="Avatar" *ngIf="story.storyImages.length > 0">
                            <div class="message_date">
                                <h3 class="date text-info">{{day(story.created)}}</h3>
                                <p class="month">{{month(story.created)}}</p>
                            </div>
                            <div class="message_wrapper">
                                <h4 *ngIf="link" [routerLink]="['/reporting/story', story.id]" (click)="clicked()" class="heading">{{story.title}}<small *ngIf="showAuthor"> by <story-author [storyId]="story.id"></story-author></small></h4>
                                <h4 *ngIf="!link" (click)="fullDisplay=true" class="heading">{{story.title}}<small *ngIf="showAuthor"> by <story-author [storyId]="story.id"></story-author></small></h4>
                                <blockquote class="message" [innerHtml]="htmlToPlaintext(story.story)"></blockquote>
                                <br>
                                <p class="url">
                                    <span class="fs1 text-info" aria-hidden="true" data-icon=""></span>
                                    <a *ngIf="link" [routerLink]="['/reporting/story', story.id]" (click)="clicked()"><i class="fa fa-align-left"></i> Full Story</a>
                                    <a *ngIf="!link" (click)="fullDisplay=true" style="cursor:pointer;"><i class="fa fa-align-left"></i> Full Story</a>
                                </p>
                            </div>
                        </div>
                        <div *ngIf="fullDisplay">
                        
                            <div class="text-right">
                                <button (click)="fullDisplay = false" class="btn btn-info btn-xs">close</button>
                            </div>
                            <br>
                            <br>
                            <br>
                            <success-story-display [story]="story"></success-story-display>
                            <br>
                            <br>
                            <br>
                        </div>
                    
    `,
    styles: [`
        img.avtr, img.avtr {
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
            .message_wrapper {
                margin-left: 105px !important;
                margin-right: 45px !important;
            }
    `]
})
export class StoryShortComponent { 


    @Input('success-story-short') story: Story;
    @Input() link:boolean = true;
    
    fullDisplay = false;
    errorMessage: string;

    constructor(  
        private service: StoryService,
        private location: Location
    )   
    {}

    ngOnInit(){ 
        
    }

    htmlToPlaintext(text) {
        var result = text ? String(text).replace(/<[^>]+>/gm, '') : ''
        return result.substring(0, 200);
    }

    day(dateString){
        return new Date(dateString).getDate();
    }


    month(dateString){
        return new Date(dateString).toLocaleString("en-us", { month: "long" });
    }

    externalUrl(url){
        return this.location.prepareExternalUrl(url);
    }

    clicked(){
        window.scrollTo(0,0);
    }

    
}
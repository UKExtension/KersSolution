import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {StoryService, Story} from '../story.service';
import { Observable } from "rxjs";
import {Location} from '@angular/common';

@Component({
    selector: 'success-story-display-list',
    template: `

<!-- start more stories -->
<ul class="messages">
  @for (story of stories | async; track story) {
    <li [success-story-short]="story" [link]="link"></li>
  }
</ul>
<!-- end more stories -->
`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class StoryReportsDisplayListComponent { 


    @Input() stories: Observable<Story[]>;
    @Input() showAuthor:boolean = false;
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
        return new Date(dateString).getDay();
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
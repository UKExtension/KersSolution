import { Component, Input } from '@angular/core';
import {Story} from '../story.service';

@Component({
    selector: 'success-story-display-list-sync',
    template: ` 
                <ul class="messages">
                    <li *ngFor="let story of stories" [success-story-short]="story" [link]="link"></li>
                </ul>    
    `
})
export class StoryReportsDisplayListSyncComponent { 


    @Input() stories: Story[];
    @Input() showAuthor:boolean = false;
    @Input() link:boolean = true;
    
    fullDisplay = false;
    errorMessage: string;

    constructor( 
    )   
    {}

    ngOnInit(){ 
        
    }

    

    
}
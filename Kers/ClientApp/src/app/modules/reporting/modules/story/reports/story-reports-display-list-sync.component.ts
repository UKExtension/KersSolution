import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {Story} from '../story.service';

@Component({
    selector: 'success-story-display-list-sync',
    template: `
                <ul class="messages">
                  @for (story of stories; track story) {
                    <li [success-story-short]="story" [link]="link"></li>
                  }
                </ul>
                `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { StoryService } from '../story.service';
import { User } from '../../user/user.service';


@Component({
    selector: 'story-author',
    template: `@if (author) {<span>{{author.personalProfile.firstName}} {{author.personalProfile.lastName}}</span>}`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class StoryAuthorComponent { 

   @Input()storyId:number;
   author:User;

    constructor(  
        private service: StoryService 
    )   
    {}

    ngOnInit(){ 
        this.service.author(this.storyId).subscribe(
            res => this.author = res
        )
    }

    
}
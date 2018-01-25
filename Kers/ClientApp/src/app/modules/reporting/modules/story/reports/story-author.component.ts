import { Component, Input } from '@angular/core';
import { StoryService } from '../story.service';
import { User } from '../../user/user.service';


@Component({
    selector: 'story-author',
    template: `<span ngIf="author">{{author.personalProfile.firstName}} {{author.personalProfile.lastName}}</span>`
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
import { Component, Input } from '@angular/core';
import {StoryService, Story} from '../story.service';
import { User } from "../../user/user.service";

@Component({
    selector: 'success-story-display',
    template: `
        <div class="row">
            <div class="col-xs-12">
                <h1>{{story.title}}</h1> 
                <p *ngIf="author">
                <strong>by</strong> <a routerLink="/reporting/user/{{author.id}}" > {{author.personalProfile.firstName}} {{author.personalProfile.lastName}}</a>
                <br><strong>Planning Unit:</strong> {{author.rprtngProfile.planningUnit.name}}
                </p>
                <p><strong>Major Program: </strong> {{story.majorProgram.name}}</p><br><br>
                <div [innerHtml]="story.story | safeHtml" class="fr-view"></div>
            </div>
        </div>

    
    `
})
export class StoryDisplayComponent { 

    @Input() story: Story;
    author:User;
    errorMessage: string;

    constructor(  
        private service: StoryService
    )   
    {}

    ngOnInit(){ 
         this.service.author(this.story.id).subscribe(
             res => {
                this.author = res;
             }
         )
    }

    


    
}
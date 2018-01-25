import { Component, Input } from '@angular/core';
import {StoryService, Story} from '../story.service';
import { Observable } from "rxjs/Observable";
import { User } from "../../user/user.service";

@Component({
    selector: 'success-story-display',
    template: `
        <div class="row">
            <div class="col-xs-12">
                <h1>{{story.title}}</h1> 
                <p *ngIf="author | async">by <a routerLink="/reporting/user/{{(author | async)?.id}}" >{{(author | async)?.personalProfile.firstName}} {{(author | async)?.personalProfile.lastName}}</a></p>
                <p><strong>Major Program: </strong> {{story.majorProgram.name}}</p><br><br>
                <div [innerHtml]="story.story | safeHtml" class="fr-view"></div>
            </div>
        </div>

    
    `
})
export class StoryDisplayComponent { 

    @Input() story: Story;
    author:Observable<User>;
    errorMessage: string;

    constructor(  
        private service: StoryService
    )   
    {}

    ngOnInit(){ 
        this.author = this.service.author(this.story.id).share();
    }

    


    
}
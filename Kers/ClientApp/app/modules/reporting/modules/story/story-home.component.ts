import { Component, OnInit } from '@angular/core';
import {StoryService, Story} from './story.service';


@Component({
  templateUrl: 'story-home.component.html'
})
export class StoryHomeComponent implements OnInit { 
    
    latest:Story[] = [];
    numbStories = 0;
    errorMessage:string;
    newStory = false;

    constructor( 
        private service:StoryService
    )   
    {}

    ngOnInit(){
        this.service.latest().subscribe(
            res => this.latest = <Story[]> res,
            err => this.errorMessage = <any> err
        );
       this.service.num().subscribe(
           res=> this.numbStories = <number>res,
           err => this.errorMessage = <any> err
       );
    }
    deleted(story:Story){
        let index: number = this.latest.indexOf(story);
        if (index !== -1) {
            this.latest.splice(index, 1);
            this.numbStories--;
        }
    }
     newStorySubmitted(story:Story){
        this.latest.unshift(story);
        this.numbStories++;
        this.newStory = false;
    }

    loadMore(){
        var lt = this.latest;
        this.service.latest(this.latest.length, 2).subscribe(
            res=>{
                    var batch = <Story[]>res; 
                    batch.forEach(function(element){
                        lt.push(element);
                    });
                },
            err => this.errorMessage = <any>err
        );
    }


}
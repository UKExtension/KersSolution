import { Component, Input, Output, EventEmitter } from '@angular/core';
import {StoryService, Story} from './story.service';

@Component({
    selector: 'story-detail',
    templateUrl: 'story-detail.component.html'
})
export class StoryDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    
    @Input() story:Story;

    @Output() onDeleted = new EventEmitter<Story>();
    
    errorMessage: string;

    constructor( 
        private service:StoryService
    )   
    {}

    ngOnInit(){
       
       
       
    }
    edit(){
        this.rowDefault = false;
        this.rowEdit = true;
        this.rowDelete = false;
    }
    delete(){
        this.rowDefault = false;
        this.rowEdit = false;
        this.rowDelete = true;
    }
    default(){
        this.rowDefault = true;
        this.rowEdit = false;
        this.rowDelete = false;
    }

    confirmDelete(){
        
        this.service.delete(this.story.id).subscribe(
            res=>{
                this.onDeleted.emit(this.story);
            },
            err => this.errorMessage = <any> err
        );
        
    }

    storySubmitted(story:Story){
        this.story = story;
        this.default();
    }
    

    
}
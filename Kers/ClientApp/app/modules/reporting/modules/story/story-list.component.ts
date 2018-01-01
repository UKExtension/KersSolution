import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {Story} from './story.service';


@Component({
    selector: 'story-list',
    templateUrl: 'story-list.component.html'
})
export class StoryListComponent implements OnInit{ 
    
    @Input() latest:Story[] = [];
    @Output() onDeleted = new EventEmitter<Story>();
    
    errorMessage: string;

    constructor( 
    )   
    {}

    ngOnInit(){
       
       
       
    }

    deleted(story:Story){
        this.onDeleted.emit(story);
    }
    

}
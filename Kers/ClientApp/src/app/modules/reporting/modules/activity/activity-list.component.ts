import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {ActivityService, ActivityMonth, Activity} from './activity.service';


@Component({
    selector: 'activity-list',
    templateUrl: 'activity-list.component.html'
})
export class ActivityListComponent implements OnInit{ 
    
    @Input() byMonth:ActivityMonth[] = [];
    @Output() onDeleted = new EventEmitter<Activity>();
    @Output() onEdited = new EventEmitter<Activity>();
    
    errorMessage: string;

    constructor( 
        private service:ActivityService
    )   
    {}

    ngOnInit(){
       
       
       
    }

    deleted(activity:Activity){
        this.onDeleted.emit(activity);
    }
    edited(activity:Activity){
        this.onEdited.emit(activity);
    }
    

}
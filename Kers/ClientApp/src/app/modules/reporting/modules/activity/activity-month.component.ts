import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ActivityService, ActivityMonth, Activity} from './activity.service';

@Component({
    selector: 'activity-month',
    templateUrl: 'activity-month.component.html'
})
export class ActivityMonthComponent { 

    
    @Input() month:ActivityMonth;
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
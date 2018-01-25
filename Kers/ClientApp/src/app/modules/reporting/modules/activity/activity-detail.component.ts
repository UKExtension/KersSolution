import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ActivityService, ActivityMonth, Activity} from './activity.service';

@Component({
    selector: 'activity-detail',
    templateUrl: 'activity-detail.component.html'
})
export class ActivityDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    
    @Input() activity:Activity;

    @Output() onDeleted = new EventEmitter<Activity>();
    @Output() onEdited = new EventEmitter<Activity>();
    
    errorMessage: string;

    constructor( 
        private service:ActivityService
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

    activitySubmitted(activity:Activity){
        this.activity = activity;
        this.onEdited.emit(activity);
        this.default();
    }

    confirmDelete(){
        
        this.service.delete(this.activity.id).subscribe(
            res=>{
                this.onDeleted.emit(this.activity);
            },
            err => this.errorMessage = <any> err
        );
        
    }
    

    
}
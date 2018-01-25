import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ServicelogService, Servicelog } from "./servicelog.service";

@Component({
    selector: 'servicelog-detail',
    templateUrl: 'servicelog-detail.component.html'
})
export class ServicelogDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    
    @Input() activity:Servicelog;

    @Output() onDeleted = new EventEmitter<Servicelog>();
    @Output() onEdited = new EventEmitter<Servicelog>();
    
    errorMessage: string;

    constructor( 
        private service:ServicelogService
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

    activitySubmitted(activity:Servicelog){
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
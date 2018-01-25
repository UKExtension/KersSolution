import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ServicelogService, ServicelogMonth, Servicelog } from './servicelog.service';


@Component({
    selector: 'servicelog-list',
    templateUrl: 'servicelog-list.component.html'
})
export class ServocelogListComponent implements OnInit{ 
    
    
    @Input() byMonth:ServicelogMonth[] = [];
    @Output() onDeleted = new EventEmitter<Servicelog>();
    @Output() onEdited = new EventEmitter<Servicelog>();
    
    errorMessage: string;

    constructor( 
        private service:ServicelogService
    )   
    {}

    ngOnInit(){
       
       
       
    }

    deleted(activity:Servicelog){
        this.onDeleted.emit(activity);
    }
    edited(activity:Servicelog){
        this.onEdited.emit(activity);
    }
    

}
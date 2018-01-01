import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Servicelog, ServicelogService, ServicelogMonth } from "./servicelog.service";

@Component({
    selector: 'servicelog-month',
    templateUrl: 'servicelog-month.component.html'
})
export class ServicelogMonthComponent { 

    
    @Input() month:ServicelogMonth;
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
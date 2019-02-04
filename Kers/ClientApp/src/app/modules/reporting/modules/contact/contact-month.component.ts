import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ContactService, ContactMonth, Contact} from './contact.service';

@Component({
    selector: 'contact-month',
    templateUrl: 'contact-month.component.html'
})
export class ContactMonthComponent { 

    
    @Input() month:ContactMonth;
    @Output() onDeleted = new EventEmitter<Contact>();
    @Output() onUpdated = new EventEmitter<Contact>();
    
    errorMessage: string;

    constructor( 
        private service:ContactService
    )   
    {}

    ngOnInit(){
       
       
       
    }

    updated(contact:Contact){
        this.onUpdated.emit(contact);
    }

    deleted(contact:Contact){
        this.onDeleted.emit(contact);
    }

    
}
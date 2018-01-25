import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ContactService, ContactMonth, Contact} from './contact.service';

@Component({
    selector: 'contact-month',
    templateUrl: 'contact-month.component.html'
})
export class ContactMonthComponent { 

    
    @Input() month:ContactMonth;
    @Output() onDeleted = new EventEmitter<Contact>();
    
    errorMessage: string;

    constructor( 
        private service:ContactService
    )   
    {}

    ngOnInit(){
       
       
       
    }

    deleted(contact:Contact){
        this.onDeleted.emit(contact);
    }

    
}
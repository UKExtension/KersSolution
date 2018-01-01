import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {ContactService, ContactMonth, Contact} from './contact.service';


@Component({
    selector: 'contact-list',
    templateUrl: 'contact-list.component.html'
})
export class ContactListComponent implements OnInit{ 
    
    @Input() byMonth:ContactMonth[] = [];
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
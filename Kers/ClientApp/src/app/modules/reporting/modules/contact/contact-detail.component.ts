import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ContactService, ContactMonth, Contact} from './contact.service';

@Component({
    selector: 'contact-detail',
    templateUrl: 'contact-detail.component.html'
})
export class ContactDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    
    @Input() contact:Contact;

    @Output() onDeleted = new EventEmitter<Contact>();
    
    errorMessage: string;

    constructor( 
        private service:ContactService
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

    contactSubmitted(contact:Contact){
        this.contact = contact;
        this.default();
    }

    confirmDelete(){
        
        this.service.delete(this.contact.id).subscribe(
            res=>{
                this.onDeleted.emit(this.contact);
            },
            err => this.errorMessage = <any> err
        );
        
    }
    

    
}
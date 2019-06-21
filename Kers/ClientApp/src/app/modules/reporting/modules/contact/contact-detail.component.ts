import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ContactService, ContactMonth, Contact} from './contact.service';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'contact-detail',
    templateUrl: 'contact-detail.component.html'
})
export class ContactDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    currentFiscalYear:FiscalYear;
    displayEditButtons = false;
    
    @Input() contact:Contact;

    @Output() onDeleted = new EventEmitter<Contact>();
    @Output() onUpdated = new EventEmitter<Contact>();
    
    errorMessage: string;

    constructor( 
        private service:ContactService,
        private fiscalYearService:FiscalyearService
    )   
    {}

    ngOnInit(){
        this.fiscalYearService.current("serviceLog", true).subscribe(
            res =>{
                this.currentFiscalYear = res;
      
                if( 
                    new Date(this.currentFiscalYear.start) <= new Date(this.contact.contactDate) 
                    && 
                    new Date(this.currentFiscalYear.end) >= new Date(this.contact.contactDate)
                ){
                    this.displayEditButtons = true;
                }
            } 
       );
       
       
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
        this.onUpdated.emit(this.contact);
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
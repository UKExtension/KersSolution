import { Component, OnInit } from '@angular/core';
import {    ContactService, Contact, ContactRaceEthnicityValue,
            ContactMonth
        } from './contact.service';

@Component({
  templateUrl: 'contact-home.component.html'
})
export class ContactHomeComponent implements OnInit { 
    
    newContact = false;
    latest = [];
    numbContacts = 0;
    byMonth:ContactMonth[] = [];

    errorMessage:string;

    constructor( 
        private service:ContactService
    )   
    {}

    ngOnInit(){
        this.service.latest().subscribe(
            res=>{
                    this.latest = <Contact[]>res; 
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
        this.service.num().subscribe(
            res => {
                this.numbContacts = <number>res;
            },
            err => this.errorMessage = <any>err
        );
       
    }


    populateByMonth(){
        var bm = this.byMonth;
        this.latest.forEach(function(element){
            
                var exDt = new Date(element.contactDate);
                var exMonth = bm.filter(f=>f.month==exDt.getMonth() && f.year == exDt.getFullYear());
                if(exMonth.length == 0){
                    var ob = <ContactMonth>{
                        month : exDt.getMonth(),
                        year : exDt.getFullYear(),
                        date: exDt,
                        activities : [element]
                    };
                    bm.push(ob);
                }else{
                    exMonth[0].activities.push(element);
                }
            }); 
    }

    newContactSubmitted(contact:Contact){
        this.latest.unshift(contact);
        this.byMonth = [];
        this.populateByMonth();
        this.numbContacts++;
        this.newContact = false;
    }

    loadMore(){
        var lt = this.latest;
        this.service.latest(this.latest.length, 2).subscribe(
            res=>{
                    var batch = <Contact[]>res; 
                    batch.forEach(function(element){
                        lt.push(element);
                    });
                    this.byMonth = [];
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
    }

    deleted(activity:Contact){
        let index: number = this.latest.indexOf(activity);
        if (index !== -1) {
            this.latest.splice(index, 1);
            this.numbContacts--;
        }
        this.byMonth = [];
        this.populateByMonth();
    }


}
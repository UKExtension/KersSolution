import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import { ContactService } from '../contact/contact.service';
import { Observable } from 'rxjs/internal/Observable';


@Component({
  template: `


  <div>
    <div class="row x_title">
        <div class="col-md-6">
        <h3>Employees</h3>
        </div>
                  
   </div>
   

    <user-directory-list [onlyKSU]="true" [initialAmount]="100" [showEmployeeSummaryButton]="true"></user-directory-list>

  </div>
  <div class="ln_solid"></div>
  <h2>MONTHLY STATISTICAL CONTACTS BY EMPLOYEE</h2>
<div class="view-row" *ngIf="!pr">
    <div class="text-right">
        <button class="btn btn-info btn-xs" (click)="openStats()" >view</button>
    </div>
</div>
<div class="close-row" *ngIf="pr">
    <div class="text-right">
        <button class="btn btn-info btn-xs" (click)="closeStats()" >close</button>
    </div>
    <loading *ngIf="loading"></loading>
    <div [innerHTML]="kSUdata"></div>
</div>
    
  `
})
export class KsuHomeComponent { 


    pr:boolean = false;
    loading:boolean = true;
    kSUdata:string;
    constructor( 
        private reportingService: ReportingService,
        private contactService: ContactService
    )   
    {}

    ngOnInit(){
        this.defaultTitle();
        
    }

    ngOnDestroy(){
        this.reportingService.setTitle( '' );
        this.reportingService.setSubtitle('');
    }

    defaultTitle(){
        this.reportingService.setTitle("Kentucky State University");
    }
    openStats(){
        this.pr = true;
        if(this.kSUdata == null){
             this.contactService.LastMonthsKSUData().subscribe(
                res=>{
                    this.kSUdata = res["table"];
                    this.loading = false;
                }
             );
        }

    }
    closeStats(){
        this.pr = false;
    }
}
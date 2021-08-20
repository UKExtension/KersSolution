import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs';
import {FiscalyearFormComponent} from './fiscalyear-form.component';
import {FiscalyearService, FiscalYear} from './fiscalyear.service';

@Component({
    template: `
<div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newYear" (click)="newFiscalYearOpen()">+ new fiscal year</a>
    </div>
    <fiscalyear-form *ngIf="newYear" (onFormCancel)="newFiscalYearCancelled()" (onFormSubmit)="newFiscalYearSubmitted()"></fiscalyear-form>
</div>
<div *ngIf="fiscalyears">
    <table class="table table-striped">
        <thead>
        <tr>
            <th>Name</th>
            <th>Type</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
            <tr *ngFor="let fiscalyear of fiscalyears" [fiscalyearListDetail]="fiscalyear" (onFiscalyearUpdated)="onFiscalYearUpdate()" (onFisclyearDeleted)="onFiscalYearUpdate()"></tr>
        </tbody>               
    </table>            
</div>
       
    `

})
export class FiscalyearListComponent implements OnInit{


    errorMessage: string;
    newYear = false;

    fiscalyears: FiscalYear[];

    constructor(
        private router: Router,
        private service: FiscalyearService
    ){}

    ngOnInit(){
        this.getList();
    }

    getList(){
        this.service.listFiscalYears(true).subscribe(
            fiscal => {
                this.fiscalyears = null;
                this.fiscalyears = fiscal;
            },
            error =>  this.errorMessage = <any>error
        );
    }

    onFiscalYearUpdate(){
        this.getList();
    }

    newFiscalYearOpen(){
        this.newYear = true;
    }

    newFiscalYearCancelled(){
        this.newYear=false;
    }
    newFiscalYearSubmitted(){
        this.newYear=false;
        this.getList();
    }
}

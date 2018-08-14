import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';

import { Router } from "@angular/router";

import { saveAs } from 'file-saver';
import { User } from "../../user/user.service";

@Component({
    selector: 'expense-reports-month',
    template: `
    
    <div class="expense-row" [class.row-even]="isIndexEven()">
        <div class="row">
            
            <div class="col-xs-7" *ngIf="rowDefault">
                
                <article class="media event">
                    <div class="media-body">
                        <a class="title" style="font-size:1.1em;">{{date | date:'MMMM'}} <small style="font-weight:normal;">({{year.year}})</small></a>
                    </div>
                </article>

            </div>
            <div class="col-xs-11" *ngIf="rowSummary">
                <h3>{{date | date:'MMMM'}}</h3>Monthly Expense Summary<br><br>
                <expense-reports-summary [month]="month" [year]="year" [user]="user"></expense-reports-summary>

            </div>
            <div class="col-xs-11" *ngIf="rowDetails" style="padding-bottom: 40px;">
                    
                    
                    <h3>{{date | date:'MMMM'}}</h3>Detailed Expense Records<br><br>


                    <expense-reports-details [month]="month" [year]="year" [user]="user"></expense-reports-details>
            </div>
                

            

            <div class="col-xs-5 text-right" *ngIf="rowDefault">
                                
                

                <div class="btn-group" [class.open]="pdfMenuOpen" style="margin-top: -5px; margin-right: 5px;">
                    <button type="button" class="btn btn-info btn-xs"><i class="fa fa-download"></i> Pdf</button>
                    <button type="button" class="btn btn-info btn-xs dropdown-toggle" (click)="pdfMenuOpen = !pdfMenuOpen">
                        <span class="caret"></span>
                        <span class="sr-only">Toggle Dropdown</span>
                    </button>
                    <ul class="dropdown-menu" role="menu">
                        <li><a (click)="printTrip()" *ngIf="!pdfTripLoading" >Mileage Log - Day Trips</a>
                        <loading [type]="'bars'" *ngIf="pdfTripLoading"></loading>
                        </li>
                        <li><a (click)="printTrip(true)" *ngIf="!pdfTripLoadingOvernight">Mileage Log - Overnight Trips</a>
                        <loading [type]="'bars'" *ngIf="pdfTripLoadingOvernight"></loading>
                        </li>
                        <li class="divider"></li>
                        <li><a (click)="print()" *ngIf="rowDefault && !pdfLoading">Detailed Monthly Report</a>
                        <loading [type]="'bars'" *ngIf="pdfLoading"></loading>
                        </li>
                    </ul>
                </div>
                <a class="btn btn-info btn-xs" (click)="summary()" ><i class="fa fa-cog"></i> Summary</a>
                <a class="btn btn-info btn-xs" (click)="details()" *ngIf="rowDefault"><i class="fa fa-cogs"></i> Details</a>

            </div>
            
            <div class="col-xs-1 text-right" *ngIf="!rowDefault">
                <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
            </div>  
        </div>
    </div>
        `,
        styles: [`
            .row-even{
                background-color: #f9f9f9;
            }
            .expense-row{
                padding: 10px 7px;
                border-top: 1px solid #ddd;
            }
        `]
})
export class ExpenseReportsMonthComponent { 

    errorMessage: string;
    @Input() month;
    @Input() index;
    @Input() year;
    @Input() user:User;
    date:Date;

    pdfTripLoadingOvernight = false;
    pdfTripLoading = false;
    pdfMenuOpen = false;

    rowDefault =true;
    rowSummary = false;
    rowDetails = false;
    
    loading = false;
    pdfLoading = false;


    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
        this.date = new Date();
        this.date.setDate(15);
        this.date.setMonth(this.month.month - 1);
    }


    summary(){
        this.rowDefault = false;
        this.rowSummary = true;
        this.rowDetails = false;
    }
    details(){
        this.rowDefault = false;
        this.rowSummary = false;
        this.rowDetails = true;
    }
    default(){
        this.rowDefault = true;
        this.rowSummary = false;
        this.rowDetails = false;
    }

    print(){
        this.pdfLoading = true;
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        
        this.service.pdf(this.year.year, this.month.month, userid).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ExpensesReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfLoading = false;
            },
            err => console.error(err)
        )
    }

    printTrip(isOvernight:boolean = false){
        if( isOvernight )
        {
            this.pdfTripLoadingOvernight = true;
        }else{
            this.pdfTripLoading = true;
        }
        
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdfTrip(this.year.year, this.month.month, userid, isOvernight).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "ExpensesMileageReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfTripLoading = false;
                this.pdfTripLoadingOvernight = false;
            },
            err => console.error(err)
        )
    }



    isIndexEven() {
        this.index = Number(this.index);
        return this.index === 0 || !!(this.index && !(this.index%2));
    }

}
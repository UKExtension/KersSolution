import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from '../expense.service';

import { Router } from "@angular/router";

import { saveAs } from 'file-saver';
import { User, PlanningUnit, UserService } from "../../user/user.service";
import { Vehicle } from '../vehicle/vehicle.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
@Component({
    selector: 'expense-reports-month',
    template: `

<div class="expense-row" [class.row-even]="isIndexEven()">
  <div class="row">

    @if (rowDefault) {
      <div class="col-xs-7">
        <article class="media event">
          <div class="media-body">
            <a class="title" style="font-size:1.1em;">{{date | date:'MMMM'}} <small style="font-weight:normal;">({{year.year}})</small></a>
          </div>
        </article>
      </div>
    }
    @if (rowSummary) {
      <div class="col-xs-11">
        <h3>{{date | date:'MMMM'}}</h3>Monthly Expense Summary<br><br>
        <expense-reports-summary [month]="month" [year]="year" [user]="user"></expense-reports-summary>
      </div>
    }
    @if (rowDetails) {
      <div class="col-xs-11" style="padding-bottom: 40px;">
        <h3>{{date | date:'MMMM'}}</h3>Detailed Expense Records<br><br>
        <expense-reports-details [month]="month" [year]="year" [user]="user"></expense-reports-details>
      </div>
    }




    @if (rowDefault) {
      <div class="col-xs-5 text-right">
        <div class="btn-group" [class.open]="pdfMenuOpen" style="margin-top: -5px; margin-right: 5px;">
          <button type="button" class="btn btn-info btn-xs"><i class="fa fa-download"></i> Pdf</button>
          <button type="button" class="btn btn-info btn-xs dropdown-toggle" (click)="pdfMenuOpen = !pdfMenuOpen">
            <span class="caret"></span>
            <span class="sr-only">Toggle Dropdown</span>
          </button>
          <ul class="dropdown-menu" role="menu">
            <li>
              @if (!isMileage && !pdfTripLoading) {
                <a (click)="printTrip()" >Mileage Log - Personal Vehicle</a>
              }
              @if (isMileage && !pdfTripLoading) {
                <a (click)="printMileage()" >Mileage Log - Personal Vehicle</a>
              }
              @if (pdfTripLoading) {
                <loading [type]="'bars'"></loading>
              }
            </li>
            @if (enabledVehicles.length > 0) {
              <li>
                @if (!isMileage && !pdfTripLoadingOvernight) {
                  <a (click)="printTrip(false, false)">Mileage Log - County Vehicle</a>
                }
                @if (isMileage && !pdfTripLoadingOvernight) {
                  <a (click)="printMileage(false, false)">Mileage Log - County Vehicle</a>
                }
                @if (pdfTripLoadingOvernight) {
                  <loading [type]="'bars'"></loading>
                }
              </li>
            }
            @if (!isMileage) {
              <li class="divider"></li>
            }
            <li>@if (!isMileage && rowDefault && !pdfLoading) {
              <a (click)="print()">Detailed Monthly Report</a>
            }
            @if (pdfLoading) {
              <loading [type]="'bars'"></loading>
            }
          </li>
        </ul>
      </div>
      <a class="btn btn-info btn-xs" (click)="summary()" ><i class="fa fa-cog"></i> Summary</a>
      @if (rowDefault) {
        <a class="btn btn-info btn-xs" (click)="details()"><i class="fa fa-cogs"></i> Details</a>
      }
    </div>
  }

  @if (!rowDefault) {
    <div class="col-xs-1 text-right">
      <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
    </div>
  }
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
        `],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ExpenseReportsMonthComponent { 

    errorMessage: string;
    @Input() month;
    @Input() index;
    @Input() year;
    @Input() user:User;
    date:Date;

    currentPlanningUnit: PlanningUnit;
    enabledVehicles:Vehicle[] = [];

    pdfTripLoadingOvernight = false;
    pdfTripLoading = false;
    pdfMenuOpen = false;

    rowDefault =true;
    rowSummary = false;
    rowDetails = false;
    
    loading = false;
    pdfLoading = false;

    isMileage = false;


    constructor( 
        private service:ExpenseService,
        private userService:UserService,
        private planningUnitService: PlanningunitService
    )   
    {}

    ngOnInit(){
        this.date = new Date();
        this.date.setDate(15);
        this.date.setMonth(this.month.month - 1);
        if( (this.month.month > 10 && this.year.year > 2019) || this.year.year > 2020 ) this.isMileage = true;
        if(this.user != null){
            this.planningUnitService.id(this.user.rprtngProfile.planningUnitId).subscribe(
                res => {
                this.currentPlanningUnit = res;
                this.enabledVehicles = this.currentPlanningUnit.vehicles.filter( v => v.enabled);
                }
            );
        }else{
            this.userService.current().subscribe(
                res=> { 
                    
                    this.user = <User>res;
                    this.planningUnitService.id(this.user.rprtngProfile.planningUnitId).subscribe(
                      res => {
                         this.currentPlanningUnit = res;
                         this.enabledVehicles = this.currentPlanningUnit.vehicles.filter( v => v.enabled);
                      }
                  )
                    
                },
                error => this.errorMessage = <any>error
              );
        }
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
                this.pdfMenuOpen = false;
            },
            err => console.error(err)
        )
    }

    printTrip(isOvernight:boolean = false, isPersonal = true){
        if( isPersonal )
        {
            this.pdfTripLoadingOvernight = true;
        }else{
            this.pdfTripLoading = true;
        }
        
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdfTrip(this.year.year, this.month.month, userid, isOvernight, isPersonal).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "MileageReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfTripLoading = false;
                this.pdfTripLoadingOvernight = false;
                this.pdfMenuOpen = false;
            },
            err => console.error(err)
        )
    }

    printMileage(isOvernight:boolean = false, isPersonal = true){
        if( isPersonal )
        {
            this.pdfTripLoadingOvernight = true;
        }else{
            this.pdfTripLoading = true;
        }
        
        var userid = 0;
        if(this.user != null){
            userid = this.user.id;
        }
        this.service.pdfMileage(this.year.year, this.month.month, userid, isOvernight, isPersonal).subscribe(
            data => {
                var blob = new Blob([data], {type: 'application/pdf'});
                saveAs(blob, "MileageReport_" + this.year.year + "_" + this.month.month + ".pdf");
                this.pdfTripLoading = false;
                this.pdfTripLoadingOvernight = false;
                this.pdfMenuOpen = false;
            },
            err => console.error(err)
        )
    }



    isIndexEven() {
        this.index = Number(this.index);
        return this.index === 0 || !!(this.index && !(this.index%2));
    }

}
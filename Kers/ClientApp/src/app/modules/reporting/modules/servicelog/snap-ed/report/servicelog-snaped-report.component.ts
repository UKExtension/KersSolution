import { Component } from '@angular/core';
import {ReportingService} from '../../../../components/reporting/reporting.service';
import { UserService, User, PlanningUnit } from '../../../user/user.service';
import { FiscalyearService, FiscalYear } from '../../../admin/fiscalyear/fiscalyear.service';
import { SnapBudgetReimbursementsNepAssistant, SnapedAdminService, SnapBudgetReimbursementsCounty } from '../../../admin/snaped/snaped-admin.service';

@Component({
  template: `
    <div class="alert alert-danger alert-dismissible fade in" role="alert" *ngIf="errorMessage">
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
        </button>
        <strong>Error: </strong> {{errorMessage}}
    </div>
    <div *ngIf="user">
    <fiscal-year-switcher [type]="'snapEd'" [initially]="'current'" [showNext]="false" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    <br>
        <div class="col-xs-12">
            <snape-ed-stats *ngIf="fiscalYear" [user]="user" [fiscalYear]="fiscalYear"></snape-ed-stats>
        </div>
        <snape-ed-commitment-stats *ngIf="fiscalYear" [user]="user" [fiscalYear]="fiscalYear"></snape-ed-commitment-stats>
        <br><br>
        <!--
        <div *ngIf="isSnapEdAssistant">
            <h2>Reimbursements</h2>
            <div *ngIf="budget">
                <table class="table table-striped" *ngIf="reimbursments">
                    <thead *ngIf="reimbursments.length > 0">
                        <tr>
                            <th>Notes</th>
                            <th class="text-right">Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr *ngFor="let reimbursment of reimbursments">
                            <td>{{reimbursment.notes}}</td>
                            <td class="text-right">{{reimbursment.amount  | currency:'USD':'symbol' }}</td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>Remaining budget: <strong>{{remainingBudget | currency:'USD':'symbol' }}</strong></td>
                            <td class="text-right">Total Reimbursements: <strong>{{totalReimbursementsAmount | currency:'USD':'symbol' }}</strong></td>
                        </tr>
                    </tfoot>               
                </table>
            </div>
        </div>
        -->
        <h2>County SNAP-Ed Budget</h2>
        <div *ngIf="countyBudget">
        <table class="table table-striped" *ngIf="countyReimbursements">
            <thead *ngIf="countyReimbursements.length > 0">
                <tr>
                    <th>Notes</th>
                    <th class="text-right">Amount</th>
                </tr>
            </thead>
            <tbody>
                <tr *ngFor="let countyReimbursment of countyReimbursements">
                    <td>{{countyReimbursment.notes}}</td>
                    <td class="text-right">{{countyReimbursment.amount  | currency:'USD':'symbol' }}</td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td>Remaining budget: <strong>{{remainingCountyBudget | currency:'USD':'symbol' }}</strong></td>
                    <td class="text-right">Total Reimbursements: <strong>{{totalCountyReimbursementsAmount | currency:'USD':'symbol' }}</strong></td>
                </tr>
            </tfoot>               
        </table>
    </div>
    </div>
  `
})
export class ServicelogSnapedReportComponent { 

    user:User;
    fiscalYear:FiscalYear;
    errorMessage: string;
    reimbursments:SnapBudgetReimbursementsNepAssistant[];
    totalReimbursementsAmount = 0;
    budget:number;
    remainingBudget:number;
    isSnapEdAssistant:boolean = false;

    county:PlanningUnit;
    countyBudget:number;
    remainingCountyBudget:number;
    totalCountyReimbursementsAmount = 0;
    countyReimbursements:SnapBudgetReimbursementsCounty[];

    constructor( 
        private service:SnapedAdminService,
        private reportingService: ReportingService,
        private fiscalyearService:FiscalyearService,
        private userService: UserService 
    )   
    {}

    ngOnInit(){
        /* this.fiscalyearService.current('snapEd').subscribe(
            res => {
              this.fiscalYear = res;
            },
            err => this.errorMessage = <any>err
          ); */
        this.userService.current().subscribe(
            res => {
                this.user = <User>res;
                this.county = this.user.rprtngProfile.planningUnit;
                this.checkIfAssistant();
            },
            err => this.errorMessage = <any>err
        );
        this.defaultTitle();
    }

    getDefaultBudgets(){
        if(this.isSnapEdAssistant){
            this.service.assistantReimbursments(this.user.id, this.fiscalYear.name).subscribe(
                res => {
                  this.reimbursments = <SnapBudgetReimbursementsNepAssistant[]>res;
                  this.service.assistantBudget(this.fiscalYear.name).subscribe(
                    res => {
                      this.budget = <number>res;
                      this.calculateTotalReinbursments();
                    },
                    err => this.errorMessage = <any> res
                  );
                },
                err => this.errorMessage = <any> err
            );
        }
        


        this.service.countyBudget(this.county.id, this.fiscalYear.name).subscribe(
            res=>{
                this.countyBudget = <number>res;
                this.service.countyReimbursments(this.county.id, this.fiscalYear.name).subscribe(
                    res => {
                        this.countyReimbursements = res;
                        this.calculateTotalCountyReinbursments();
                    },
                    err => this.errorMessage = <any>err
                )
            },
            err => this.errorMessage = <any>err
        );
            
    }


    calculateTotalReinbursments(){
        this.totalReimbursementsAmount = 0;
        for( let r of this.reimbursments){
          this.totalReimbursementsAmount += r.amount;
        }
        this.remainingBudget = this.budget - this.totalReimbursementsAmount;
    }


    calculateTotalCountyReinbursments(){
        this.totalCountyReimbursementsAmount = 0;
        for( let r of this.countyReimbursements){
          this.totalCountyReimbursementsAmount += r.amount;
        }
        this.remainingCountyBudget = this.countyBudget - this.totalCountyReimbursementsAmount;
    }

    checkIfAssistant(){
        var assistantRecords = this.user.specialties.filter(s => s.specialty.code == 'snapEd' || s.specialty.code == 'progNEP');
        if(assistantRecords.length > 0){
            this.isSnapEdAssistant = true;
        }else{
            this.isSnapEdAssistant = false;
        }
    }

    fiscalYearSwitched(event:FiscalYear){
        this.fiscalYear = null;
        var scope = this;
        setTimeout(function(){
            
            scope.fiscalYear = event;
            scope.getDefaultBudgets();
        }, 200);
    }

    defaultTitle(){
        this.reportingService.setTitle("Snap Ed Report");
    }
}
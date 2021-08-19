import { Component } from '@angular/core';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';
import { PlanningUnit, User } from '../../user/user.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { FormBuilder, Validators, FormControl, AbstractControl } from "@angular/forms";
import { switchMap } from 'rxjs/operators';

@Component({
  templateUrl: 'snaped-county.component.html',
  styles: [
    `
    .bar_tabs li{
      cursor: pointer;
    }
    `
  ]
})
export class SnapedCountyComponent { 

    fiscalYear:FiscalYear;
    fiscalYearData:FiscalYear;
    committed: number;
    reported:number;

    budgetForm;
    reimbursmentForm;

    newReinbursment = false;

    reimbursments;

    totalReimbursementsAmount = 0;
    remainingBudget = 0;

    editBudget = false;

    county:PlanningUnit;
    assistants: User[];
    countyBudget:number;

    countyName:string;

    errorMessage: string;

    tabAssistants = true;
    tabWithCommitment = false;
    tabAll = false;

    constructor( 
        private service:SnapedAdminService,
        private fiscalyearService:FiscalyearService,
        private reportingService:ReportingService,
        private route: ActivatedRoute,
        private fb: FormBuilder,
        private planningUnitService: PlanningunitService
    )   
    {
      this.budgetForm = this.fb.group(
        {
          annualBudget: [0, this.isPositiveInt]
        });
      
    }

    ngOnInit(){

      this.route.params.pipe(
            switchMap( (params: Params) => this.planningUnitService.id(params['id']) )
          ).
          subscribe(
            res=>{
              var countyRes = res;
              
              this.county = countyRes;
              this.countyName = this.county.name.substring(0, this.county.name.length - 4);
              this.reportingService.setTitle(this.countyName + " Snap-Ed Admin Dashboard");

              this.service.countyBudget(this.county.id).subscribe(
                res=>{
                  this.countyBudget = <number>res;
                  this.budgetForm.patchValue({annualBudget: this.countyBudget});
                },
                err => this.errorMessage = <any>err
              );
              this.service.countyReimbursments(this.county.id).subscribe(
                res => {
                  this.reimbursments = res;
                  this.calculateTotalReinbursments();
                },
                err => this.errorMessage = <any>err
              )

            },
            err => this.errorMessage = <any> err
          );
    }
    fiscalYearSwitchedData(event:FiscalYear){
      if(this.fiscalYear == null ){
        this.fiscalYear = event;
      }
      this.fiscalYearData = event;

    }

    calculateTotalReinbursments(){
      this.totalReimbursementsAmount = 0;
      for( let r of this.reimbursments){
        this.totalReimbursementsAmount += r.amount;
      }
      this.remainingBudget = this.countyBudget - this.totalReimbursementsAmount;
    }


    storeHours(event){
      this.reported = event;
      if(this.committed != undefined){
        this.addStats();
      }
     
    }
    storeCommitment(event){
      this.committed = event;
      if(this.reported != undefined){
        this.addStats();
      }
    }

    newReinbursmentSubmit(event){
      this.reimbursments.push(event);
      this.newReinbursment = false;
      this.calculateTotalReinbursments();
    }


    addStats(){


      let percent = 0;
      if(this.committed == 0){
          percent = 100;
      }else{
          if(this.committed > 0){
              percent = Math.round(this.reported / this.committed * 100);
           }
      }
      
      
      this.reportingService.addStats(`
      
      <div class="row top_tiles">
      <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
        <div class="tile-stats">
          <div class="icon"><i class="fa fa-check-square-o"></i></div>
          <div class="count">`+ this.fiscalYear.name  +`</div>
          <h3>Fiscal</h3>
          <p>Year</p>
        </div>
      </div>
      <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
        <div class="tile-stats">
          <div class="icon"><i class="fa fa-comments-o"></i></div>
          <div class="count">`+ this.committed +`</div>
          <h3>Total</h3>
          <p>Commited Hours</p>
        </div>
      </div>
      <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
        <div class="tile-stats">
          <div class="icon"><i class="fa fa-bookmark-o"></i></div>
          <div class="count">` + this.reported + `</div>
          <h3>Total</h3>
          <p>Reported Hours</p>
        </div>
      </div>
      <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
        <div class="tile-stats">
          <div class="icon"><i class="fa fa-flag-o"></i></div>
          <div class="count">` + percent + `</div>
          <h3>Percent</h3>
          <p>To Commitment</p>
        </div>
      </div>
    </div>
      
      `);
    }

    onCountyBudgetSubmit(){
      this.service.updateCountyBudget(this.county.id, this.budgetForm.value).subscribe(
        res => {
          this.countyBudget = res["annualBudget"];
          this.editBudget = false;
          this.calculateTotalReinbursments();
        }
      )
    }
    reimbursementUpdated(event){
      var filtered = this.reimbursments.filter( r => r.id == event.id);
      if(filtered.length > 0){
        filtered[0].amount = event.amount;
        this.calculateTotalReinbursments();
      }
    }
    reimbursementDeleted(event){
      let index: number = this.reimbursments.indexOf(event);
      if (index !== -1) {
          this.reimbursments.splice(index, 1);
          this.calculateTotalReinbursments();
      }
    }


    /************************
      
      Validators
    
     ***********************/

    isIntOrFloat(control:FormControl){
      if(control.value == +control.value && +control.value >= 0){
          return null;
      }
      return {"notDigit":true};
    }

    isPositiveInt(control:FormControl){
        
        if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
            return null;
        }
        return {"notInt":true};
    }

    ngOnDestroy(){
      this.reportingService.addStats('');
    }

}
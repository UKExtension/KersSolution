import { Component } from '@angular/core';
import { SnapedAdminService, SnapBudgetReimbursementsNepAssistant } from './snaped-admin.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';
import { PlanningUnit, User, UserService } from '../../user/user.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { Observable } from 'rxjs/Observable';
import { SnapedService } from '../../servicelog/snaped.service';

@Component({
  templateUrl: 'snaped-user.component.html'
})
export class SnapedUserComponent { 

    fiscalYear:FiscalYear;
    committed: number;
    reported:number;
    newReinbursment = false;

    reimbursments:SnapBudgetReimbursementsNepAssistant[];
    totalReimbursementsAmount = 0;
    
    assistant: User;

    budget:number;
    remainingBudget:number;
    errorMessage: string;
    

    constructor( 
        private service:SnapedAdminService,
        private snapService:SnapedService,
        private fiscalyearService:FiscalyearService,
        private reportingService:ReportingService,
        private route: ActivatedRoute,
        private userService: UserService
    )   
    {}

    ngOnInit(){
      this.fiscalyearService.current('snapEd').subscribe(
        res => {
          this.fiscalYear = res;
        }
      );
      this.route.params
            .switchMap( (params: Params) => this.userService.byId(params['id'])).
            subscribe(
              res=>{
                this.assistant = <User>res;
                this.reportingService.setTitle("Snap-Ed Assistant " + this.assistant.personalProfile.firstName + ' ' + this.assistant.personalProfile.lastName)
                this.reportingService.setSubtitle(this.assistant.rprtngProfile.planningUnit.name);
                this.service.assistantReimbursments(this.assistant.id).subscribe(
                  res => {
                    this.reimbursments = <SnapBudgetReimbursementsNepAssistant[]>res;
                    this.service.assistantBudget().subscribe(
                      res => {
                        this.budget = <number>res;
                        this.calculateTotalReinbursments();
                      },
                      err => this.errorMessage = <any> res
                    );
                  },
                  err => this.errorMessage = <any> res
                );
                
                
              },
              err => this.errorMessage = <any> err
            );



        /*
        this.service.reported().subscribe(
          res=>{
            this.reported = res;
            this.service.commited().subscribe(
              res => {
                this.committed = res;
                this.fiscalyearService.current('snapEd').subscribe(
                  res => {
                    this.fiscalYear = res;
                    this.addStats();
                  }
                )
              }
            )
          }
        );
        */
    }
    storeHours(event){
      this.reported = <number>event;
      this.snapService.committedhours(this.assistant.id).subscribe(
        res => {
          this.committed = res;
          
          this.addStats();

        }
      )
    }

    newReinbursmentSubmit(event){
      this.reimbursments.push(event);
      this.newReinbursment = false;
      this.calculateTotalReinbursments();
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


    calculateTotalReinbursments(){
      this.totalReimbursementsAmount = 0;
      for( let r of this.reimbursments){
        this.totalReimbursementsAmount += r.amount;
      }
      this.remainingBudget = this.budget - this.totalReimbursementsAmount;
    }

    addStats(){


      let percent = 0;
      if(this.committed == 0){
          percent = 100;
      }else{
          if(this.committed > 0){
              percent = Math.round( this.reported / this.committed * 100);
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

    ngOnDestroy(){
      this.reportingService.addStats('');
      this.reportingService.setSubtitle('');
    }
}
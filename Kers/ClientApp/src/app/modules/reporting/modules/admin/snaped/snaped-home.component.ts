import { Component } from '@angular/core';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { saveAs } from 'file-saver';

@Component({
  template: `
    <fiscal-year-switcher [type]="'snapEd'" [initially]="'current'" [showNext]="true" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    <br><br><div>
      <a (click)="ccond = !ccond" style="cursor:pointer;"><i class="fa fa-plus-square" *ngIf="!ccond"></i><i class="fa fa-minus-square" *ngIf="ccond"></i> Counties </a>
      <div *ngIf="ccond">
        <planningunit-list [link]="link"></planningunit-list>
      </div>
    </div>
    <div>
      <a (click)="cond = !cond" style="cursor:pointer;"><i class="fa fa-plus-square" *ngIf="!cond"></i><i class="fa fa-minus-square" *ngIf="cond"></i> Snap-Ed Assistants </a>
      <div *ngIf="cond">
        <snaped-assistants-list></snaped-assistants-list>
      </div>
    </div>

<br><br>
<div *ngIf="fiscalYear">
  <h2>Data Downloads</h2>
  <div *ngIf="fiscalYear.start.getTime() < currentDate.getTime() ">
    <h5>Fiscal Year Totals: </h5>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'totalbymonth'" [filename]="'TotalByMonth'" [label]="'by Month'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'totalbycounty'" [filename]="'TotalByCounty'" [label]="'by County'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'totalbyemployee'" [filename]="'TotalByEmployee'" [label]="'by Employee'"></snaped-download-button>|
  </div>
  <h5>Commitment: </h5>
  <snaped-download-button [fiscalYear]="fiscalYear" [location]="'commitmentsummary'" [filename]="'CommitmentSummary'" [label]="'Summary'"></snaped-download-button>|
  <snaped-download-button [fiscalYear]="fiscalYear" [location]="'commitmenthoursdetail'" [filename]="'CommitmentHoursDetail'" [label]="'Hours Detail'"></snaped-download-button>|
  <snaped-download-button [fiscalYear]="fiscalYear" [location]="'agentswithoutcommitment'" [filename]="'AgentsWithoutCommitment'" [label]="'Agents With No Commitment Hours'"></snaped-download-button>  
  <br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'summarybyplanningunit'" [filename]="'CommitmentSummaryByPlanningUnit'" [label]="'Summary By Planning Unit'"></snaped-download-button>| 
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'summarybyplanningunitnotassistants'" [filename]="'CommitmentSummaryByPlanningUnitNotNEPAssistants'" [label]="'Summary By Planning Unit (Excludes NEP Assistants)'"></snaped-download-button> 
   <!-- <br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'reinforcementitems'" [filename]="'ReinforcementItems'" [label]="'Reinforcement Items'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'reinforcementitemspercounty'" [filename]="'ReinforcementItemsPerCounty'" [label]="'Reinforcement Items Per County'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'suggestedincentiveitems'" [filename]="'SuggestedIncentiveItems'" [label]="'Suggested Incentive Items'"></snaped-download-button> 
    <br> -->
  <div *ngIf="fiscalYear.start.getTime() < currentDate.getTime() ">
    <h5>Reimbursement Year-To-Date Totals: </h5>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'reimbursementnepassistants'" [filename]="'ReimbursementNepAssistants'" [label]="'NEP Assistants'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'reimbursementcounty'" [filename]="'ReimbursementCounty'" [label]="'County (Not NEP Assistants)'"></snaped-download-button> 
    <h5>Copy Reimbursement Requests:</h5>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'copiessummarybycountyagents'" [filename]="'CopiesSummarybyCountyAgents'" [label]="'Summary by Month/County (Agents)'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'copiessummarybycountynotagents'" [filename]="'CopiesSummarybyCountyNotAgents'" [label]="'Summary by Month/County (Other than agents)'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'copiesdetailagents'" [filename]="'CopiesDetailAgents'" [label]="'Detail (Agents)'"></snaped-download-button>|
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'copiesdetailnotagents'" [filename]="'CopiesDetailNotAgents'" [label]="'Detail (Other than Agents)'"></snaped-download-button>
    <h5>Monthly Community Summary: </h5>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'byaimedtowardsimprovement'" [filename]="'ByAimedTowardsImprovement'" [label]="'by Aimed towards improvement in'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'bypartnercategory'" [filename]="'ByPartnerCategory'" [label]="'by Partner category'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'agentcommunityeventdetail'" [filename]="'AgentCommunityEventDetail'" [label]="'Agent Community Event Detail Report'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'numberofdeliverysitesbytypeofsetting'" [filename]="'NumberofDeliverySitesbyTypeofSetting'" [label]="'Number of Delivery Sites by Type of Setting'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'sessiontypebymonth'" [filename]="'SessionTypebyMonth'" [label]="'Session Type by Month'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'sessiontypebyproject'" [filename]="'SessionTypebyProject'" [label]="'Session Type by Project'"></snaped-download-button><br>
    
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'estimatedsizeofaudiencesreached'" [filename]="'EstimatedSizeofAudiencesReached'" [label]="'Estimated Size of Audiences Reached - Communication/Events'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'methodsusedrecordcount'" [filename]="'MethodsUsedRecordCount'" [label]="'Methods Used - Record Count'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'personnelhourdetails'" [filename]="'PersonnelHourDetails'" [label]="'Personnel Hour Details'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'individualcontacttotals'" [filename]="'IndividualContactTotals'" [label]="'Individual Contact Totals'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'specificsitenamesbymonth'" [filename]="'SpecificSitenamesbyMonth'" [label]="'Specific Site Names By Month'"></snaped-download-button><br>
    
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'specificsitedetails'" [filename]="'SpecificSiteDetails'" [label]="'Specific Site Details'"></snaped-download-button><br>
    
    
    
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'directbypersonbymonth'" [filename]="'DirectByPersonPerMonth'" [label]="'Direct Sites By Person, By Month - including number of contacts'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'indirectbyemployee'" [filename]="'IndirectByEmployee'" [label]="'Indirects per Person per Number Reached'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'indirectbyproject'" [filename]="'IndirectByProject'" [label]="'Indirect by Project'"></snaped-download-button><br>
    <snaped-download-button [fiscalYear]="fiscalYear" [location]="'audienceagecategory'" [filename]="'AudienceAgeCategory'" [label]="'Audience Age and Category Totals'"></snaped-download-button><br>
    <br><br>
    <button class="btn btn-success btn-xs" routerLink="/reporting/admin/snaped/reports">Custom Reports</button>
  </div>
</div>
  `
})
export class SnapedHomeComponent { 

    fiscalYear:FiscalYear;
    committed: number;
    reported:number;
    link = "/reporting/admin/snaped/county/"
    ccond = false;
    cond = false;
    commitmentHours = false;
    currentDate = new Date();

    constructor( 
        private service:SnapedAdminService,
        private fiscalyearService:FiscalyearService,
        private reportingService:ReportingService
    )   
    {}

    ngOnInit(){
        
        this.reportingService.setTitle("Snap-Ed Admin Dashboard");
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

    fiscalYearSwitched(event:FiscalYear){
      this.fiscalYear = event;
      this.fiscalYear.start = new Date( this.fiscalYear.start );
      this.reportingService.addStats('');
      this.service.reported(this.fiscalYear.name).subscribe(
        res=>{
          this.reported = res;
          this.service.commited(this.fiscalYear.name).subscribe(
            res => {
              this.committed = res;
              this.addStats();
            }
          )
        }
      );
      
    }


    ngOnDestroy(){
      this.reportingService.addStats('');
    }

}
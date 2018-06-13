import { Component } from '@angular/core';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { saveAs } from 'file-saver';

@Component({
  template: `
    
    <div>
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
    <div>
    <a (click)="commitmentHours = !commitmentHours" style="cursor:pointer;"><i class="fa fa-plus-square" *ngIf="!commitmentHours"></i><i class="fa fa-minus-square" *ngIf="commitmentHours"></i> Commitment Hours</a>
    <div *ngIf="commitmentHours">
     
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="/kers_mobile/SnapEd2018CommitmentWorksheet.aspx?admin=1">Commitment Hours Worksheet per individual</a>
      <br>
    </div>
  </div><br><br>
  <h2>Data Downloads</h2>
  <h5>Fiscal Year Totals: </h5>
  <button (click)="csvTotalByMonth()" class="btn btn-info btn-xs" *ngIf="!totalByMonth_loading">by Month</button><loading [type]="'bars'" *ngIf="totalByMonth_loading"></loading>|   
  <button (click)="csvTotalByCounty()" class="btn btn-info btn-xs" *ngIf="!totalByCounty_loading">by County</button><loading [type]="'bars'" *ngIf="totalByCounty_loading"></loading>|  
  <button (click)="csvTotalByEmployee()" class="btn btn-info btn-xs" *ngIf="!totalByEmployee_loading">by Employee</button><loading [type]="'bars'" *ngIf="totalByEmployee_loading"></loading> 
  <h5>Commitment FY2019: </h5>
  <button (click)="csvCommitmentSummary()" class="btn btn-info btn-xs" *ngIf="!commitmentSummary_loading">Summary</button><loading [type]="'bars'" *ngIf="commitmentSummary_loading"></loading>|
  <button (click)="csvCommitmentHoursDetail()" class="btn btn-info btn-xs" *ngIf="!commitmentHoursDetail_loading">Hours Detail</button><loading [type]="'bars'" *ngIf="commitmentHoursDetail_loading"></loading>|  
  <button (click)="csvAgentsWithoutCommitment()" class="btn btn-info btn-xs" *ngIf="!agentswithoutcommitment_loading">Agents With No Commitment Hours</button><loading [type]="'bars'" *ngIf="agentswithoutcommitment_loading"></loading>  
  <br>
  <button (click)="csvSummaryByPlanningUnit()" class="btn btn-info btn-xs" *ngIf="!SummaryByPlanningUnit_loading">Summary By Planning Unit</button><loading [type]="'bars'" *ngIf="SummaryByPlanningUnit_loading"></loading>|  
  <button (click)="csvSummaryByPlanningUnitNotNEPAssistants()" class="btn btn-info btn-xs" *ngIf="!summaryByPlanningUnitNotNEPAssistants_loading">Summary By Planning Unit (Excludes NEP Assistants)</button><loading [type]="'bars'" *ngIf="summaryByPlanningUnitNotNEPAssistants_loading"></loading>  
  <br>
  <button (click)="csvReinforcementItems()" class="btn btn-info btn-xs" *ngIf="!reinforcementItems_loading">Reinforcement Items</button><loading [type]="'bars'" *ngIf="reinforcementItems_loading"></loading>|
  <button (click)="csvReinforcementItemsPerCounty()" class="btn btn-info btn-xs" *ngIf="!reinforcementItemsPerCounty_loading">Reinforcement Items Per County</button><loading [type]="'bars'" *ngIf="reinforcementItemsPerCounty_loading"></loading>|  
  <button (click)="csvSuggestedIncentiveItems()" class="btn btn-info btn-xs" *ngIf="!suggestedIncentiveItems_loading">Suggested Incentive Items</button><loading [type]="'bars'" *ngIf="suggestedIncentiveItems_loading"></loading>  
  <br>
  <!--
  <h5>Commitment: </h5>
  <a href="https://kers.ca.uky.edu/kers_mobile/SnapEd2018AdminDashboard.aspx">Commitment reports are available here</a>
  Summary  |   Detail  |   Agents With No Commitment Hours
  Summary By Planning Unit  |   Summary By Planning Unit (Excludes NEP Assistants)
  Reinforcement Items  |   Reinforcement Items By County  |   Suggested Incentive Items 
  -->
  <h5>Reimbursement Year-To-Date Totals: </h5>
  <button (click)="csvReimbursementNepAssistants()" class="btn btn-info btn-xs" *ngIf="!reimbursementNepAssistants_loading">NEP Assistants</button><loading [type]="'bars'" *ngIf="reimbursementNepAssistants_loading"></loading>|
  <button (click)="csvReimbursementCounty()" class="btn btn-info btn-xs" *ngIf="!reimbursementCounty_loading">County (Not NEP Assistants)</button><loading [type]="'bars'" *ngIf="reimbursementCounty_loading"></loading> 
  <h5>Copy Reimbursement Requests:</h5>
  <button (click)="csvCopiesSummarybyCountyAgents()" class="btn btn-info btn-xs" *ngIf="!copiesSummarybyCountyAgents_loading">Summary by Month/County (Agents)</button><loading [type]="'bars'" *ngIf="copiesSummarybyCountyAgents_loading"></loading>|
  <button (click)="csvCopiesSummarybyCountyNotAgents()" class="btn btn-info btn-xs" *ngIf="!copiesSummarybyCountyNotAgents_loading">Summary by Month/County (Other than agents)</button><loading [type]="'bars'" *ngIf="copiesSummarybyCountyNotAgents_loading"></loading><br>
  <button (click)="csvCopiesDetailAgents()" class="btn btn-info btn-xs" *ngIf="!copiesDetailAgents_loading">Detail (Agents)</button><loading [type]="'bars'" *ngIf="copiesDetailAgents_loading"></loading>|
  <button (click)="csvCopiesDetailNotAgents()" class="btn btn-info btn-xs" *ngIf="!copiesDetailNotAgents_loading">Detail (Other than Agents)</button><loading [type]="'bars'" *ngIf="copiesDetailNotAgents_loading"></loading> 
  <h5>Monthly Community Summary: </h5>
  <button (click)="csvByAimedTowardsImprovement()" class="btn btn-info btn-xs" *ngIf="!byAimedTowardsImprovement_loading">by Aimed towards improvement in</button><loading [type]="'bars'" *ngIf="byAimedTowardsImprovement_loading"></loading>| 
  <button (click)="csvByPartnerCategory()" class="btn btn-info btn-xs" *ngIf="!byPartnerCategory_loading">by Partner category</button><loading [type]="'bars'" *ngIf="byPartnerCategory_loading"></loading>|
  <button (click)="csvAgentCommunityEventDetail()" class="btn btn-info btn-xs" *ngIf="!agentCommunityEventDetail_loading">Agent Community Event Detail Report</button><loading [type]="'bars'" *ngIf="agentCommunityEventDetail_loading"></loading> <br><br>  
  <button (click)="csvNumberofDeliverySitesbyTypeofSetting()" class="btn btn-info btn-xs" *ngIf="!numberofDeliverySitesbyTypeofSetting_loading">Number of Delivery Sites by Type of Setting</button><loading [type]="'bars'" *ngIf="numberofDeliverySitesbyTypeofSetting_loading"></loading><br>
  <button (click)="csvSessionTypebyMonth()" class="btn btn-info btn-xs" *ngIf="!sessionTypebyMonth_loading">Session Type by Month</button><loading [type]="'bars'" *ngIf="sessionTypebyMonth_loading"></loading><br>
  <button (click)="csvEstimatedSizeofAudiencesReached()" class="btn btn-info btn-xs" *ngIf="!estimatedSizeofAudiencesReached_loading">Estimated Size of Audiences Reached - Communication/Events</button><loading [type]="'bars'" *ngIf="estimatedSizeofAudiencesReached_loading"></loading><br>
  <button (click)="csvMethodsUsedRecordCount()" class="btn btn-info btn-xs" *ngIf="!methodsUsedRecordCount_loading">Methods Used - Record Count</button><loading [type]="'bars'" *ngIf="methodsUsedRecordCount_loading"></loading> <br>
  <button (click)="csvPersonnelHourDetails()" class="btn btn-info btn-xs" *ngIf="!personnelHourDetails_loading">Personnel Hour Details</button><loading [type]="'bars'" *ngIf="personnelHourDetails_loading"></loading><br>
  <button (click)="csvIndividualContactTotals()" class="btn btn-info btn-xs" *ngIf="!individualContactTotals_loading">Individual Contact Totals</button><loading [type]="'bars'" *ngIf="individualContactTotals_loading"></loading><br>
  <button (click)="csvSpecificSiteNameByMonth()" class="btn btn-info btn-xs" *ngIf="!specificSiteNameByMonth_loading">Specific Site Names By Month</button><loading [type]="'bars'" *ngIf="specificSiteNameByMonth_loading"></loading><br>
  <button (click)="csvDirectByPersonByMonth()" class="btn btn-info btn-xs" *ngIf="!directByPersonByMonth_loading">Direct Sites By Person, By Month - including number of contacts</button><loading [type]="'bars'" *ngIf="directByPersonByMonth_loading"></loading><br>

  <button (click)="csvIndirectByEmployee()" class="btn btn-info btn-xs" *ngIf="!indirectByEmployee_loading">Indirects per Person per Number Reached</button><loading [type]="'bars'" *ngIf="indirectByEmployee_loading"></loading><br>



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

    constructor( 
        private service:SnapedAdminService,
        private fiscalyearService:FiscalyearService,
        private reportingService:ReportingService
    )   
    {}

    ngOnInit(){
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





    totalByMonth_loading = false;
    csvTotalByMonth(){
      this.totalByMonth_loading = true;
      this.service.csv('totalbymonth/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.totalByMonth_loading = false;
              saveAs(blob, "TotalByMonth_2018.csv");
          },
          err => console.error(err)
      )
    }
    totalByCounty_loading = false;
    csvTotalByCounty(){
      this.totalByCounty_loading = true;
      this.service.csv('totalbycounty/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.totalByCounty_loading = false;
              saveAs(blob, "TotalByCounty_2018.csv");
          },
          err => console.error(err)
      )
    }
    totalByEmployee_loading = false;
    csvTotalByEmployee(){
      this.totalByEmployee_loading = true;
      this.service.csv('totalbyemployee/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.totalByEmployee_loading = false;
              saveAs(blob, "TotalByEmployee_2018.csv");
          },
          err => console.error(err)
      )
    }

    directByPersonByMonth_loading = false;
    csvDirectByPersonByMonth(){
      this.directByPersonByMonth_loading = true;
      this.service.csv('directbypersonbymonth/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.directByPersonByMonth_loading = false;
              saveAs(blob, "DirectByPersonPerMonth_2018.csv");
          },
          err => console.error(err)
      )
    }

    specificSiteNameByMonth_loading = false;
    csvSpecificSiteNameByMonth(){
      this.specificSiteNameByMonth_loading = true;
      this.service.csv('specificsitenamesbymonth/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.specificSiteNameByMonth_loading = false;
              saveAs(blob, "DirectByPersonPerMonth_2018.csv");
          },
          err => console.error(err)
      )
    }


    individualContactTotals_loading = false;
    csvIndividualContactTotals(){
      this.individualContactTotals_loading = true;
      this.service.csv('individualcontacttotals/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.individualContactTotals_loading = false;
              saveAs(blob, "IndividualContactTotals_2018.csv");
          },
          err => console.error(err)
      )
    }


    personnelHourDetails_loading = false;
    csvPersonnelHourDetails(){
      this.personnelHourDetails_loading = true;
      this.service.csv('personnelhourdetails/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.personnelHourDetails_loading = false;
              saveAs(blob, "PersonnelHourDetails_2018.csv");
          },
          err => console.error(err)
      )
    }

    methodsUsedRecordCount_loading = false;
    csvMethodsUsedRecordCount(){
      this.methodsUsedRecordCount_loading = true;
      this.service.csv('methodsusedrecordcount/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.methodsUsedRecordCount_loading = false;
              saveAs(blob, "MethodsUsedRecordCount_2018.csv");
          },
          err => console.error(err)
      )
    }

    estimatedSizeofAudiencesReached_loading = false;
    csvEstimatedSizeofAudiencesReached(){
      this.estimatedSizeofAudiencesReached_loading = true;
      this.service.csv('estimatedsizeofaudiencesreached/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.estimatedSizeofAudiencesReached_loading = false;
              saveAs(blob, "EstimatedSizeofAudiencesReached_2018.csv");
          },
          err => console.error(err)
      )
    }

    sessionTypebyMonth_loading = false;
    csvSessionTypebyMonth(){
      this.sessionTypebyMonth_loading = true;
      this.service.csv('sessiontypebymonth/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.sessionTypebyMonth_loading = false;
              saveAs(blob, "SessionTypebyMonth_2018.csv");
          },
          err => console.error(err)
      )
    }

    numberofDeliverySitesbyTypeofSetting_loading = false;
    csvNumberofDeliverySitesbyTypeofSetting(){
      this.numberofDeliverySitesbyTypeofSetting_loading = true;
      this.service.csv('numberofdeliverysitesbytypeofsetting/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.numberofDeliverySitesbyTypeofSetting_loading = false;
              saveAs(blob, "NumberofDeliverySitesbyTypeofSetting_2018.csv");
          },
          err => console.error(err)
      )
    }

    agentCommunityEventDetail_loading = false;
    csvAgentCommunityEventDetail(){
      this.agentCommunityEventDetail_loading = true;
      this.service.csv('agentcommunityeventdetail/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.agentCommunityEventDetail_loading = false;
              saveAs(blob, "AgentCommunityEventDetail_2018.csv");
          },
          err => console.error(err)
      )
    }

    byPartnerCategory_loading = false;
    csvByPartnerCategory(){
      this.byPartnerCategory_loading = true;
      this.service.csv('bypartnercategory/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.byPartnerCategory_loading = false;
              saveAs(blob, "ByPartnerCategory_2018.csv");
          },
          err => console.error(err)
      )
    }

    byAimedTowardsImprovement_loading = false;
    csvByAimedTowardsImprovement(){
      this.byAimedTowardsImprovement_loading = true;
      this.service.csv('byaimedtowardsimprovement/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.byAimedTowardsImprovement_loading = false;
              saveAs(blob, "ByAimedTowardsImprovement_2018.csv");
          },
          err => console.error(err)
      )
    }

    copiesSummarybyCountyAgents_loading = false;
    csvCopiesSummarybyCountyAgents(){
      this.copiesSummarybyCountyAgents_loading = true;
      this.service.csv('copiessummarybycountyagents/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.copiesSummarybyCountyAgents_loading = false;
              saveAs(blob, "CopiesSummarybyCountyAgents_2018.csv");
          },
          err => console.error(err)
      )
    }

    copiesSummarybyCountyNotAgents_loading = false;
    csvCopiesSummarybyCountyNotAgents(){
      this.copiesSummarybyCountyNotAgents_loading = true;
      this.service.csv('copiessummarybycountynotagents/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.copiesSummarybyCountyNotAgents_loading = false;
              saveAs(blob, "CopiesSummarybyCountyNotAgents_2018.csv");
          },
          err => console.error(err)
      )
    }

    copiesDetailAgents_loading = false;
    csvCopiesDetailAgents(){
      this.copiesDetailAgents_loading = true;
      this.service.csv('copiesdetailagents/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.copiesDetailAgents_loading = false;
              saveAs(blob, "CopiesDetailAgents_2018.csv");
          },
          err => console.error(err)
      )
    }

    copiesDetailNotAgents_loading = false;
    csvCopiesDetailNotAgents(){
      this.copiesDetailNotAgents_loading = true;
      this.service.csv('copiesdetailnotagents/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.copiesDetailNotAgents_loading = false;
              saveAs(blob, "CopiesDetailAgents_2018.csv");
          },
          err => console.error(err)
      )
    }


    reimbursementNepAssistants_loading = false;
    csvReimbursementNepAssistants(){
      this.reimbursementNepAssistants_loading = true;
      this.service.csv('reimbursementnepassistants/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.reimbursementNepAssistants_loading = false;
              saveAs(blob, "ReimbursementNepAssistants_2018.csv");
          },
          err => console.error(err)
      )
    }
    reimbursementCounty_loading = false;
    csvReimbursementCounty(){
      this.reimbursementCounty_loading = true;
      this.service.csv('reimbursementcounty/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.reimbursementCounty_loading = false;
              saveAs(blob, "ReimbursementCounty_2018.csv");
          },
          err => console.error(err)
      )
    }


    

    indirectByEmployee_loading = false;
    csvIndirectByEmployee(){
      this.indirectByEmployee_loading = true;
      this.service.csv('indirectbyemployee/2018').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.indirectByEmployee_loading = false;
              saveAs(blob, "IndirectByEmployee_2018.csv");
          },
          err => console.error(err)
      )
    }


    commitmentSummary_loading = false;
    csvCommitmentSummary(){
      this.commitmentSummary_loading = true;
      this.service.csv('commitmentsummary/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.commitmentSummary_loading = false;
              saveAs(blob, "CommitmentSummary_2019.csv");
          },
          err => console.error(err)
      )
    }

    commitmentHoursDetail_loading = false;
    csvCommitmentHoursDetail(){
      this.commitmentHoursDetail_loading = true;
      this.service.csv('commitmenthoursdetail/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.commitmentHoursDetail_loading = false;
              saveAs(blob, "CommitmentHoursDetail_2019.csv");
          },
          err => console.error(err)
      )
    }

    agentswithoutcommitment_loading = false;
    csvAgentsWithoutCommitment(){
      this.agentswithoutcommitment_loading = true;
      this.service.csv('agentswithoutcommitment/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.agentswithoutcommitment_loading = false;
              saveAs(blob, "AgentsWithoutCommitment_2019.csv");
          },
          err => console.error(err)
      )
    }

    SummaryByPlanningUnit_loading = false;
    csvSummaryByPlanningUnit(){
      this.SummaryByPlanningUnit_loading = true;
      this.service.csv('summarybyplanningunit/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.SummaryByPlanningUnit_loading = false;
              saveAs(blob, "SummaryByPlanningUnit_2019.csv");
          },
          err => console.error(err)
      )
    }

    summaryByPlanningUnitNotNEPAssistants_loading = false;
    csvSummaryByPlanningUnitNotNEPAssistants(){
      this.summaryByPlanningUnitNotNEPAssistants_loading = true;
      this.service.csv('summarybyplanningunitnotassistants/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.summaryByPlanningUnitNotNEPAssistants_loading = false;
              saveAs(blob, "SummaryByPlanningUnitNotNEPAssistants_2019.csv");
          },
          err => console.error(err)
      )
    }

    reinforcementItems_loading = false;
    csvReinforcementItems(){
      this.reinforcementItems_loading = true;
      this.service.csv('reinforcementitems/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.reinforcementItems_loading = false;
              saveAs(blob, "ReinforcementItems_2019.csv");
          },
          err => console.error(err)
      )
    }

    reinforcementItemsPerCounty_loading = false;
    csvReinforcementItemsPerCounty(){
      this.reinforcementItemsPerCounty_loading = true;
      this.service.csv('reinforcementitemspercounty/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.reinforcementItemsPerCounty_loading = false;
              saveAs(blob, "ReinforcementItemsPerCounty_2019.csv");
          },
          err => console.error(err)
      )
    }

    suggestedIncentiveItems_loading = false;
    csvSuggestedIncentiveItems(){
      this.suggestedIncentiveItems_loading = true;
      this.service.csv('suggestedincentiveitems/2019').subscribe(
          data => {
              var blob = new Blob([data], {type: 'text/csv'});
              this.suggestedIncentiveItems_loading = false;
              saveAs(blob, "SuggestedIncentiveItems_2019.csv");
          },
          err => console.error(err)
      )
    }

    




    ngOnDestroy(){
      this.reportingService.addStats('');
    }

}
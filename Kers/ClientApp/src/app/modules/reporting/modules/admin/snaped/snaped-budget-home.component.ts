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
  </div><br><br>
  <h2>Data Downloads</h2>
  <h5>Reimbursement Year-To-Date Totals: </h5>
  <button (click)="csvReimbursementNepAssistants()" class="btn btn-info btn-xs" *ngIf="!reimbursementNepAssistants_loading">NEP Assistants</button><loading [type]="'bars'" *ngIf="reimbursementNepAssistants_loading"></loading>|
  <button (click)="csvReimbursementCounty()" class="btn btn-info btn-xs" *ngIf="!reimbursementCounty_loading">County (Not NEP Assistants)</button><loading [type]="'bars'" *ngIf="reimbursementCounty_loading"></loading> 
  


  `
})
export class SnapedBudgetHomeComponent { 

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


    




    ngOnDestroy(){
      this.reportingService.addStats('');
    }

}
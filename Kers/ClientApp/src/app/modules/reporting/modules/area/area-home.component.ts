import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ReportingService } from '../../components/reporting/reporting.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';
import { ExtensionArea } from '../state/state.service';
import { AreaService } from './area.service';

@Component({
  selector: 'area-home',
  template: `
  <county-list [type]="'area'" [areaId]="0"></county-list> 
  
  <div class="col-xs-12">
        <div class="x_panel">
            <div class="x_title">
                <h2>Number of Submitted Activities per Agent</h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <district-employees *ngIf="area" [area]="area"></district-employees>
            </div>
        </div>
    </div>




    <div class="col-xs-12" *ngIf="lastFiscalYear && currentFiscalYear && area">
      <div class="x_panel">
        <div class="x_title">
            <h2>Assignments</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content">
          <div class="row">
              <div class="col-xs-10" *ngIf="!assignmentPlansOfWorkOpen">
                  <article class="media event ng-star-inserted">
                      <div class="media-body">
                      <a class="title">Plans of Work</a>
                      <p>List of counties with no FY{{lastFiscalYear.name}} Plans of Work</p>
                      </div>
                  </article>
              </div>
              <div class="col-xs-2 text-right" *ngIf="!assignmentPlansOfWorkOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=true">show</a>                
              </div>
              <div class="col-xs-12 text-right" *ngIf="assignmentPlansOfWorkOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentPlansOfWorkOpen=false">close</a>                
              </div>
              <assignment-plans-of-work [areaId]="area.id" [type]="area" [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentPlansOfWorkOpen"></assignment-plans-of-work>  
          </div>
          <div class="row">
              <div class="ln_solid"></div>
              <div class="col-xs-10"  *ngIf="!assignmentAffirmativeReportOpen">
                  <article class="media event ng-star-inserted">
                      <div class="media-body">
                      <a class="title">Affirmative Action Report</a>
                      <p>List of counties with no FY{{currentFiscalYear.name}} Affirmative Action Report</p>
                      </div>
                  </article>
              </div>
              <div class="col-xs-2 text-right" *ngIf="!assignmentAffirmativeReportOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentAffirmativeReportOpen=true">show</a>                
              </div>
              <div class="col-xs-12 text-right" *ngIf="assignmentAffirmativeReportOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentAffirmativeReportOpen=false">close</a>                
              </div>
              <assignment-affirmative-report [areaId]="area.id" [type]="area" [fiscalYearId]="currentFiscalYear.name"  *ngIf="assignmentAffirmativeReportOpen"></assignment-affirmative-report>
          </div>
          <div class="row">
              <div class="ln_solid"></div>
              <div class="col-xs-10" *ngIf="!assignmentAffirmativePlanOpen">
                  <article class="media event ng-star-inserted">
                      <div class="media-body">
                      <a class="title">Affirmative Action Plan</a>
                      <p>List of counties with no FY{{lastFiscalYear.name}} Affirmative Action Plan</p>
                      </div>
                  </article>
              </div>
              <div class="col-xs-2 text-right" *ngIf="!assignmentAffirmativePlanOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentAffirmativePlanOpen=true">show</a>                
              </div>
              <div class="col-xs-12 text-right" *ngIf="assignmentAffirmativePlanOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentAffirmativePlanOpen=false">close</a>                
              </div>
              <assignment-affirmative-plan [areaId]="area.id" [type]="area" [fiscalYearId]="lastFiscalYear.name" *ngIf="assignmentAffirmativePlanOpen"></assignment-affirmative-plan>  
          </div>
          <div class="row">
              <div class="ln_solid"></div>
              <div class="col-xs-10" *ngIf="!assignmentProgramIndicatorsOpen">
                  <article class="media event ng-star-inserted">
                      <div class="media-body">
                      <a class="title">Program Indicators</a>
                      <p>List of counties that submitted no Program Indicators for FY{{currentFiscalYear.name}}.</p>
                      </div>
                  </article>
              </div>
              <div class="col-xs-2 text-right" *ngIf="!assignmentProgramIndicatorsOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentProgramIndicatorsOpen=true">show</a>                
              </div>
              <div class="col-xs-12 text-right" *ngIf="assignmentProgramIndicatorsOpen">
                  <a class="btn btn-info btn-xs" (click)="assignmentProgramIndicatorsOpen=false">close</a>                
              </div>
              <assignment-program-indicators [areaId]="area.id" [type]="area" [fiscalYearId]="currentFiscalYear.name" *ngIf="assignmentProgramIndicatorsOpen"></assignment-program-indicators>  
          </div>
        </div>
      </div>
  </div>














  `,
  styles: []
})
export class AreaHomeComponent implements OnInit {

  @Input() areaId:number;


  area:ExtensionArea;

  assignmentPlansOfWorkOpen = false;
  assignmentAffirmativeReportOpen = false;
  assignmentAffirmativePlanOpen = false;
  assignmentProgramIndicatorsOpen = false;

  lastFiscalYear:FiscalYear;
  currentFiscalYear:FiscalYear;



  constructor(
      private route: ActivatedRoute,
      private router:Router,
      private service: AreaService,
      private fiscalYearService:FiscalyearService,
      private reportingService: ReportingService
  ) { }

  ngOnInit() {

    this.fiscalYearService.last().subscribe(
        res => this.lastFiscalYear = res
    )
    this.fiscalYearService.current().subscribe(
        res => this.currentFiscalYear = res
    );
    this.route.params
            .switchMap(  (params: Params) => {
                if(params['id'] != undefined){
                    this.areaId = params['id'];
                }
                return this.service.get(this.areaId ) 
            }
                        )
                          .subscribe(
                            res => {
                              this.area = res;
                              this.defaultTitle();
                            }
                          );
  }


  ngOnDestroy(){
      this.reportingService.setTitle( 'Kentucky Extension Reporting System' );
      this.reportingService.setSubtitle('');
  }

  defaultTitle(){
      this.reportingService.setTitle("Extension Area " + this.area.name );
      //this.reportingService.setSubtitle(this.area.name);
  }

  

}

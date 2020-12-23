import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ReportingService } from '../../components/reporting/reporting.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
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
      private reportingService: ReportingService
  ) { }

  ngOnInit() {

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

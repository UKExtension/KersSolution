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
  
  <p>

      area-home works!
    </p>
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
      this.reportingService.setTitle(this.area.areaName );
      this.reportingService.setSubtitle(this.area.name);
  }

}

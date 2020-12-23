import { Component, Input, OnInit } from '@angular/core';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

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

  assignmentPlansOfWorkOpen = false;
  assignmentAffirmativeReportOpen = false;
  assignmentAffirmativePlanOpen = false;
  assignmentProgramIndicatorsOpen = false;

  lastFiscalYear:FiscalYear;
  currentFiscalYear:FiscalYear;



  constructor() { }

  ngOnInit() {
  }

}

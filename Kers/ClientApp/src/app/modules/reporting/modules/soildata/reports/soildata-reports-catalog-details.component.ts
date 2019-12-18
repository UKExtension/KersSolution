import { Component, OnInit, Input } from '@angular/core';
import { SoilReportBundle } from '../soildata.report';

@Component({
  selector: '[soildata-reports-catalog-details]',
  template: `
    <td *ngIf="default">{{report.sampleLabelCreated | date:'mediumDate'}}</td>
    <td *ngIf="default">{{ report.farmerForReport == null ? 'None' : report.farmerForReport.first + ' ' + report.farmerForReport.last }}</td>
    <td *ngIf="default">{{ report.lastStatus == null ? 'Received' : report.lastStatus.soilReportStatus.name }}</td>
    <td *ngIf="default" class="text-right"><a class="btn btn-info btn-xs" (click)="editView()"><i class="fa fa-pencil"></i> review</a><a class="btn btn-info btn-xs"><i class="fa fa-download"></i> pdf</a></td>
    <td *ngIf="edit" colspan="4">
      <div class="row">
        <div class="col-xs-12">
          <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close</a>
        </div>
      </div>
      <soildata-report-form [report]="report"></soildata-report-form>
    </td>
  `,
  styles: []
})
export class SoildataReportsCatalogDetailsComponent implements OnInit {
  @Input('soildata-reports-catalog-details') report: SoilReportBundle;

  default = true;
  edit = false;

  constructor() { }

  ngOnInit() {
  }

  defaultView(){
    this.default = true;
    this.edit = false;
  }
  editView(){
    this.default = false;
    this.edit = true;
  }

}

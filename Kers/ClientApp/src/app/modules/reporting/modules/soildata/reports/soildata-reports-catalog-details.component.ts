import { Component, OnInit, Input } from '@angular/core';
import { SoilReportBundle } from '../soildata.report';
import { saveAs } from 'file-saver';
import { SoildataService } from '../soildata.service';

@Component({
  selector: '[soildata-reports-catalog-details]',
  template: `
    <td *ngIf="default">{{report.labTestsReady | date:'mediumDate'}}</td>
    <td *ngIf="default">{{report.typeForm.code}}</td>
    <td *ngIf="default">{{report.coSamnum}}</td>
    <td *ngIf="default">{{ report.farmerForReport == null ? 'None' : report.farmerForReport.first + ' ' + report.farmerForReport.last }}</td>
    <td *ngIf="default" class="{{ report.lastStatus == null ? 'soil-report-status-recieved' : report.lastStatus.soilReportStatus.cssClass }}">{{ report.lastStatus == null ? 'Received' : report.lastStatus.soilReportStatus.name }}</td>
    <td *ngIf="default" class="text-right">
      <a class="btn btn-info btn-xs" (click)="editView()"><i class="fa fa-pencil"></i> review</a>
      <a class="btn btn-info btn-xs" (click)="print()" *ngIf="!pdfLoading"><i class="fa fa-download"></i> pdf</a>
      <loading [type]="'bars'" *ngIf="pdfLoading"></loading>
      <a class="btn btn-info btn-xs" (click)="email()" ><i class="fa fa-envelope"></i> email</a>
    </td>
    <td *ngIf="edit" colspan="6">
      <div class="row">
        <div class="col-xs-12">
          <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close</a>
        </div>
      </div>
      <soildata-report-form [report]="report"></soildata-report-form>
    </td>
  `,
  styles: [`
  .soil-report-status-recieved{
    color: #3498DB;
  }
  .soil-report-status-reviewed{
    color: #1ABB9C;
  }
  .soil-report-status-archived{
    color: #34495E;
  }

  `]
})
export class SoildataReportsCatalogDetailsComponent implements OnInit {
  @Input('soildata-reports-catalog-details') report: SoilReportBundle;

  default = true;
  edit = false;
  pdfLoading = false;

  constructor(
    private service:SoildataService
  ) { }

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
  print(){
    this.pdfLoading = true;
    
    this.service.pdf(this.report.uniqueCode).subscribe(
        data => {
            if(this.report.lastStatus != null) this.report.lastStatus.soilReportStatus.name = "Archived";
            var blob = new Blob([data], {type: 'application/pdf'});
            saveAs(blob, "SoilTestResults.pdf");
            this.pdfLoading = false;
        },
        err => console.error(err)
    )
  }

  email(){
    this.service.updateBundleStatusToArchived(this.report.id, this.report).subscribe(
      res => this.report.lastStatus.soilReportStatus.name = res.lastStatus.soilReportStatus.name
    )
    var email = "";
    if(this.report.farmerForReport.emailAddress != undefined){
      email=this.report.farmerForReport.emailAddress;
    }
    var mailText = "mailto:"+email+"?subject=Soil Test Results&body=https://kers.ca.uky.edu/core/api/PdfSoilData/report/"+this.report.uniqueCode;
    window.location.href = mailText;
  }

}

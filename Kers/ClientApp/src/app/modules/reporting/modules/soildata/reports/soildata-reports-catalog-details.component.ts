import { Component, OnInit, Input } from '@angular/core';
import { SoilReportBundle } from '../soildata.report';
import { saveAs } from 'file-saver';
import { SoildataService } from '../soildata.service';
import { UserService, User } from '../../user/user.service';

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
    background-color:#50C1CFg;
  }
  .soil-report-status-reviewed{
    background-color: #1ABB9C;
  }
  .soil-report-status-archived{
    background-color: #e5e5e5;
  }

  `]
})
export class SoildataReportsCatalogDetailsComponent implements OnInit {
  @Input('soildata-reports-catalog-details') report: SoilReportBundle;

  default = true;
  edit = false;
  pdfLoading = false;
  user:User;

  constructor(
    private service:SoildataService,
    private  userService: UserService
  ) { }

  ngOnInit() {
    this.userService.current().subscribe(
      res => this.user = res
    )
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
    var body = "https://kers.ca.uky.edu/core/api/PdfSoilData/report/"+this.report.uniqueCode+'%0D%0A%0D%0A';
    body += this.report.farmerForReport.first + ',';
    body += "%0D%0A%0D%0AThe link above contains your soil test report for";
    body += "%0D%0AOwner Sample ID: "+this.report.reports[0].osId+".%0D%0A%0D%0A";
    body += "Click on the link or copy and paste it into a web browser and hit Enter. Choose to open the pdf file.";
    body += '%0D%0A%0D%0A' + this.user.rprtngProfile.planningUnit.name;
    body += '%0D%0A' + this.user.rprtngProfile.planningUnit.phone;
    var mailText = "mailto:"+email+"?subject=Soil Test Results&body=" + body;
    window.location.href = mailText;
  }

}

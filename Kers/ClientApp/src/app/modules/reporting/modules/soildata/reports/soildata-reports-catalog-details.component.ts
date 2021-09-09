import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SoilReportBundle, SoilReportStatus } from '../soildata.report';
import { saveAs } from 'file-saver';
import { SoildataService } from '../soildata.service';
import { UserService, User } from '../../user/user.service';
import { Observable } from 'rxjs';

@Component({
  selector: '[soildata-reports-catalog-details]',
  template: `
    <td *ngIf="default">{{report.labTestsReady | date:'mediumDate'}}</td>
    <td *ngIf="default">{{report.typeForm.code}}</td>
    <td *ngIf="default">{{report.coSamnum}}</td>
    <td *ngIf="default">{{ report.farmerForReport == null ? 'None' : report.farmerForReport.first + ' ' + report.farmerForReport.last }}</td>
    <td *ngIf="default" class="{{ report.lastStatus == null ? 'soil-report-status-recieved' : report.lastStatus.soilReportStatus.cssClass }}">
      <div *ngIf="!statusLoading">
        <a (click)="statusChangeClicked=!statusChangeClicked" style="cursor:pointer;">
          {{ report.lastStatus == null ? 'Received' : report.lastStatus.soilReportStatus.name }} <i class="fa fa-angle-down"></i>
        </a>
        <div *ngIf="statusChangeClicked" style="position:absolute;">
          <table class="table status-choice">
            <tbody>
              <tr *ngFor="let st of $statuses | async">
                <td><a style="cursor:pointer;" (click)="changeStatusTo(st.id)">{{st.name}}</a></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <loading *ngIf="statusLoading" [type]="bars"></loading>
    </td>
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
  .status-choice{
    background-color: #fff;
    border-bottom: 1px solid #ccc;
    border-left: 1px solid #ccc;
    border-right: 1px solid #ccc;
    margin-top: 5px;
  }
  .status-choice td{
    padding: 10px 15px;
    border: 0;
  }
  `]
})
export class SoildataReportsCatalogDetailsComponent implements OnInit {
  @Input('soildata-reports-catalog-details') report: SoilReportBundle;

  @Output() onStatusChange = new EventEmitter<SoilReportStatus>();

  default = true;
  edit = false;
  pdfLoading = false;
  statusLoading = false;
  user:User;
  statusChangeClicked = false;
  $statuses:Observable<SoilReportStatus[]>;

  constructor(
    private service:SoildataService,
    private  userService: UserService
  ) { }

  ngOnInit() {
    this.userService.current().subscribe(
      res => this.user = res
    );
    this.$statuses = this.service.reportStatuses();
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
  changeStatusTo(id:number){
    this.service.changestatus(id, this.report.id).subscribe(
      res => {
        this.report.lastStatus.soilReportStatus = res;
        this.onStatusChange.emit(res);
      }
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

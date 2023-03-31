import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SoilReportBundle, SoilReportStatus } from '../soildata.report';
import { saveAs } from 'file-saver';
import { SoildataService } from '../soildata.service';
import { UserService, User } from '../../user/user.service';
import { Observable } from 'rxjs';

@Component({
  selector: '[soildata-reports-catalog-details]',
  template: `
    <td *ngIf="default">{{report.sampleLabelCreated | date:'mediumDate'}}</td>
    <td *ngIf="default">{{report.typeForm.code}}</td>
    <td *ngIf="default">{{report.coSamnum}}</td>
    <td *ngIf="default">{{ report.farmerForReport == null ? 'None' : report.farmerForReport.first + ' ' + report.farmerForReport.last }}</td>
    <td *ngIf="default" class="{{ report.lastStatus == null ? 'soil-report-status-recieved' : report.lastStatus.soilReportStatus.cssClass }}">
      <div *ngIf="processedStatuses == null && report.lastStatus !=null">
        {{report.lastStatus.soilReportStatus.name}}
      </div>
      <ng-container *ngIf="processedStatuses != null">
        <div *ngIf="!statusLoading">
          <a (click)="statusChangeClicked=!statusChangeClicked" style="cursor:pointer;">
            {{ report.lastStatus == null ? 'Received' : report.lastStatus.soilReportStatus.name }} <i class="fa fa-angle-down"></i>
          </a>
          <div *ngIf="statusChangeClicked" style="position:absolute;">
            <table class="table status-choice">
              <tbody>
                <tr *ngFor="let st of processedStatuses">
                  <td><a style="cursor:pointer;" (click)="changeStatusTo(st.id)">{{st.name}}</a></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        <loading *ngIf="statusLoading" [type]="bars"></loading>
      </ng-container>
    </td>
    <td *ngIf="default" class="text-right">
      <a class="btn btn-info btn-xs" (click)="sampleEditView()" *ngIf="isEditButtonAvailable()"><i class="fa fa-pencil"></i> edit</a>
      <a class="btn btn-info btn-xs" (click)="sampleCopy()" *ngIf="isCopyButtonAvailable()"><i class="fa fa-copy"></i> copy</a>
      <a class="btn btn-info btn-xs" (click)="editView()" *ngIf="isReviewButtonAvailable()"><i class="fa fa-pencil"></i> review</a>
      <a class="btn btn-info btn-xs" (click)="print()" *ngIf="isPdfButtonAvailable()"><i class="fa fa-download"></i> pdf</a>
      <loading [type]="'bars'" *ngIf="pdfLoading"></loading>
      <a class="btn btn-info btn-xs" (click)="email()"  *ngIf="isEmailButtonAvailable()"><i class="fa fa-envelope"></i> email</a>
      <a *ngIf="isAlterButtonAvailable()" class="btn btn-info btn-xs" (click)="altCropView()" ><i class="fa fa-download"></i> Alter</a>
    </td>
    <td *ngIf="edit" colspan="6">
      <div class="row">
        <div class="col-xs-6">
        <loading [type]="'bars'" *ngIf="deleteLoading"></loading>
          <a (click)="openDelete=!openDelete" *ngIf="!openDelete" style="cursor: pointer;"><i class="fa fa-ellipsis-v"></i></a>
          <div *ngIf="openDelete&&!openConfirmDelete"><a (click)="openConfirmDelete=!openConfirmDelete" style="cursor: pointer;">Delete</a> | <a (click)="openDelete=!openDelete" style="cursor: pointer;">Cancel</a><br><br></div>
          <div *ngIf="openConfirmDelete&&!deleteLoading">
            <span class="blue">Do you really want to permanently delete this soil sample record?</span><br>
            <a (click)="onDelete()" style="cursor: pointer;">Confirm Delete</a> | <a (click)="openDelete=!openDelete;openConfirmDelete=!openConfirmDelete" style="cursor: pointer;">Cancel</a><br><br> </div>
        </div>
        <div class="col-xs-6">
          <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close</a>
        </div>
      </div>
      <soildata-report-form [report]="report" (onCropNoteUpdated)="cropNoteUpdated()"></soildata-report-form>
    </td>
    <td *ngIf="sampleEdit" colspan="6">
      <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close</a>
      <soil-sample-form [sample]="report" (onFormCancel)="SampleFormCanceled()" (onFormSubmit)="SampleFormSubmit($event)"></soil-sample-form>
    </td>
    <td *ngIf="altCrop" colspan="6">
      <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close</a>
      <soil-sample-form [sample]="report" [isThisAltCrop]="true" (onFormCancel)="SampleFormCanceled()" (onFormSubmit)="SampleFormSubmit($event)"></soil-sample-form>
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
  .soil-report-status-recieved{
    background-color: #ceab54;
  }
  .soil-repot-status-inlab{
    background-color: #ebb1e2;
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

  @Output() onStatusChange = new EventEmitter<SoilReportStatus | null>();
  @Output() onNoteChange = new EventEmitter<SoilReportStatus>();
  @Output() onCopySample = new EventEmitter<SoilReportBundle>();
  @Output() isItReport = new EventEmitter<boolean>();
  @Output() isItSample = new EventEmitter<boolean>();

  default = true;
  edit = false;
  altCrop = false;
  sampleEdit = false;
  pdfLoading = false;
  deleteLoading = false;
  statusLoading = false;
  openDelete = false;
  openConfirmDelete = false;
  user:User;
  statusChangeClicked = false;
  $statuses:Observable<SoilReportStatus[]>;
  processedStatuses:SoilReportStatus[];

  constructor(
    private service:SoildataService,
    private  userService: UserService
  ) { }

  ngOnInit() {
    this.userService.current().subscribe(
      res => this.user = res
    );
    this.$statuses = this.service.reportStatuses();
    this.ProcessStatuses();
    if(this.report.reports == null) this.report.reports = [];
    this.report.coSamnum = this.report.coSamnum.replace(/^0+/, '');
    if( this.report.reports.length > 0 ){
      this.isItReport.emit(true);
    }else{
      this.isItReport.emit(false);
      if(this.report.sampleInfoBundles != null && this.report.sampleInfoBundles.length > 0 && this.report.lastStatus != null && this.report.lastStatus.soilReportStatus.name == "Entered" ){
        this.isItSample.emit(true);
      }else{
        this.isItSample.emit(false);
      }
    }
    
  }

  defaultView(){
    this.default = true;
    this.edit = false;
    this.sampleEdit = false;
    this.altCrop = false;
  }
  editView(){
    this.default = false;
    this.edit = true;
    this.sampleEdit = false;
    this.altCrop = false;
  }
  sampleEditView(){
    this.default = false;
    this.edit = false;
    this.sampleEdit = true;
    this.altCrop = false;
  }
  altCropView(){
    this.default = false;
    this.edit = false;
    this.sampleEdit = false;
    this.altCrop = true;

  }

  isEditButtonAvailable():boolean{
    if( this.report.sampleInfoBundles != null 
          && 
        this.report.sampleInfoBundles.length > 0 
          && 
        this.report.reports.length == 0
    ){
      return true;
    }
    return false;
  }

  isCopyButtonAvailable():boolean{
    if( this.report.sampleInfoBundles != null 
          && 
        this.report.sampleInfoBundles.length > 0 
          && 
        this.report.reports.length == 0
          &&
        this.report.lastStatus.soilReportStatus.name != 'InLab'
    ){
      return true;
    }
    return false;
  }
  isReviewButtonAvailable():boolean{
    if( 
      this.report.reports != null 
        && 
      this.report.reports.length > 0
        &&
      (this.report.lastStatus != null && this.report.lastStatus.soilReportStatus.name != 'AltCrop')
    ){
      return true;
    }
    return false;
  }

  isPdfButtonAvailable():boolean{
    if( 
      !this.pdfLoading 
        && 
      this.report.reports != null 
        && 
      this.report.reports.length > 0
        &&
      (this.report.lastStatus != null && this.report.lastStatus.soilReportStatus.name != 'AltCrop')
    ){
      return true;
    }
    return false;
  }
  isEmailButtonAvailable():boolean{
    if( 
      this.report.reports != null 
        && 
      this.report.reports.length > 0
        &&
      (this.report.lastStatus != null && this.report.lastStatus.soilReportStatus.name != 'AltCrop')
    ){
      return true;
    }
    return false;
  }
  isAlterButtonAvailable():boolean{
    if( 
      this.report.reports != null 
        && 
      this.report.reports.length > 0
    ){
      return true;
    }
    return false;
  }

  cropNoteUpdated(){
    this.defaultView();
    this.onNoteChange.emit(this.report.lastStatus.soilReportStatus);
  }



  print(){
    this.pdfLoading = true;
    this.service.pdf(this.report.uniqueCode).subscribe(
        data => {
            if(this.report.lastStatus != null){
              this.service.reportStatuses().subscribe(
                res => {
                  var archivedStatus = res.filter( s => s.name == "Archived")[0];
                  this.report.lastStatus.soilReportStatus = archivedStatus;

                }
              );
            } 
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
        this.report.lastStatus.soilReportStatusId = res.id;
        this.onStatusChange.emit(res);
        this.statusChangeClicked = false;
      }
    )
  }

  sampleCopy(){
    this.onCopySample.emit(this.report);
  }


  onDelete(){
    this.deleteLoading = true;
    this.service.deleteReport(this.report.id).subscribe(
      res => {
        this.deleteLoading = false;
        this.onStatusChange.emit(null);
      }
    )
    
  }
  SampleFormCanceled(){
    this.defaultView();
  }
  SampleFormSubmit(event:SoilReportBundle){
    this.onStatusChange.emit(null);
    this.report = event;
    this.report.coSamnum = this.report.coSamnum.replace(/^0+/, '');
    this.defaultView();
  }

  ProcessStatuses(){
    this.userService.currentUserHasAnyOfTheRoles(["STLA"]).subscribe(

      res => {

        if(res){
          this.$statuses.subscribe(

            res => {
              this.processedStatuses = res;
            }
            
          )
        }else if( this.report.lastStatus != null )
                
                if (
                  this.report.lastStatus.soilReportStatus.roleCode == undefined 
                    || 
                  this.report.lastStatus.soilReportStatus.roleCode == ""
                ){
                  this.$statuses.subscribe(
                    res => {
                      this.processedStatuses = res.filter( f => ((f.roleCode == undefined || f.roleCode == "") && f.group == this.report.lastStatus.soilReportStatus.group));
                    }
                  )
                }
      }



    );
    
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
    body += "%0D%0AOwner Sample ID: "+this.report.ownerID+".%0D%0A%0D%0A";
    body += "Click on the link or copy and paste it into a web browser and hit Enter. Choose to open the pdf file.";
    body += '%0D%0A%0D%0A' + this.user.rprtngProfile.planningUnit.name;
    body += '%0D%0A' + this.user.rprtngProfile.planningUnit.phone;
    var mailText = "mailto:"+email+"?subject=Soil Test Results&body=" + body;
    window.location.href = mailText;
  }

}

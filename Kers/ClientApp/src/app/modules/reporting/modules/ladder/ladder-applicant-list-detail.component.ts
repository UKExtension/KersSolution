import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';
import { LadderService } from './ladder.service';
import { Router } from '@angular/router';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'ladder-applicant-list-detail',
  templateUrl: './ladder-applicant-list-detail.component.html',
  styles: []
})
export class LadderApplicantListDetailComponent implements OnInit {
  @Input() application:LadderApplication;

  rowDefault = true;
  rowDelete = false;
  rowDetails = false;
  rowEdit = false;


  constructor(
    private service:LadderService,
    private router:Router,
    private reportingService: ReportingService,
  ) { }

  ngOnInit() {
  }

  formSaved(event:LadderApplication){
    this.application = event;
    this.default();

  }

  edit(){
    this.rowDefault = false;
    this.rowDelete = false;
    this.rowDetails = false;
    this.rowEdit = true;
  }

  delete(){
    this.rowDefault = false;
    this.rowDelete = true;
    this.rowDetails = false;
    this.rowEdit = false;
  }

  confirmDelete(){
    this.service.delete(this.application.id).subscribe(
      _ => {
        this.reportingService.setAlert("Professional Promotion Application draft has been deleted.");
        this.router.navigate(['']);
      }
    )
  }

  default(){
    this.rowDefault = true;
    this.rowDelete = false;
    this.rowDetails = false;
    this.rowEdit = false;

  }

  details(){
    this.rowDefault = false;
    this.rowDelete = false;
    this.rowDetails = true;
    this.rowEdit = false;
  }

}

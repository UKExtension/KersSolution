import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';

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


  constructor() { }

  ngOnInit() {
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

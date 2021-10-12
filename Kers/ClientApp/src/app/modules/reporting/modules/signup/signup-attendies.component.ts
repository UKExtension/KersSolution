import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { Servicelog } from '../servicelog/servicelog.service';
import { ActivitySignUpEntry, SignupService } from './signup.service';
import { saveAs } from 'file-saver';

@Component({
  selector: 'signup-attendies',
  template: `
  <h2>Attended By</h2>
  <div class="row" *ngIf="!displayNewEntry">
    <div class="col-xs-6">
      <a class="btn btn-info btn-xs" (click)="displayNewEntry = !displayNewEntry">+</a>
    </div>
    <div class="col-xs-6 text-right">
      <!-- <a class="btn btn-info btn-xs" (click)="print()"><i class="fa fa-download"></i> Pdf</a> -->
      <a class="btn btn-info btn-xs" (click)="csvDownload()()"><i class="fa fa-download"></i> Csv</a>
    </div>
  </div>
  <div *ngIf="displayNewEntry">
    <signup-form [activity]="activity" [dalayConfirm]="false" (Submit)="newEntry();"></signup-form>
  </div>
  <table class="table">
    <tbody>
      <tr *ngFor="let attendie of attendies$ | async" [signup-list-row]="attendie"></tr>
    </tbody>
  </table>
  `,
  styles: [
  ]
})
export class SignupAttendiesComponent implements OnInit {
  attendies$:Observable<ActivitySignUpEntry[]>;
  @Input() activity:Servicelog = null;
  @Output() removed = new EventEmitter<ActivitySignUpEntry>();
  @Output() added = new EventEmitter<ActivitySignUpEntry>();
  @Output() edited = new EventEmitter<ActivitySignUpEntry>();



  displayNewEntry = false;
  loading = false;

  constructor(
    private service:SignupService
  ) { 
    
  }

  ngOnInit(): void {
    this.attendies$=this.service.attendedBy(this.activity.id);
  }
  print(){

  }

  newEntry(){
    this.displayNewEntry = false;
    this.attendies$=this.service.attendedBy(this.activity.id);
  }

  csvDownload(){
    this.loading = true;
    this.service.csv( this.activity.id ).subscribe(
        data => {
            if(data["size"] == undefined){
              var blob = new Blob(["An Error Occured"], {type: 'text/csv'});
              this.loading = false;
              saveAs(blob, "Attendies.csv");
            }else{
              var blob = new Blob([data], {type: 'text/csv'});
              this.loading = false;
              saveAs(blob, "Attendies.csv");
            }
        },
        err => console.error(err)
    )
  }



}

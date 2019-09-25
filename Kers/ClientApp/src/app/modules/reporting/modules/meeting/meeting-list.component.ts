import { Component, OnInit, Input } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { IMyDrpOptions, IMyDateRangeModel } from "mydaterangepicker";
import { startWith, flatMap, delay, map, tap } from 'rxjs/operators';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { TrainingSearchCriteria } from '../training/training';
import { MeetingService, Meeting } from './meeting.service';

@Component({
  selector: 'meeting-list',
  template: `
  <h3>CES Events</h3>
  <div class="row">
    <div class="col-sm-6 col-xs-12" style="margin-top: 3px;">
      <input type="text" [(ngModel)]="criteria.search" placeholder="search by title" (keyup)="onSearch($event)" class="form-control" name="Search" />
    </div>
    <div class="col-sm-6 col-xs-12 text-right" style="margin-top: 3px;">
        <my-date-range-picker name="mydaterange" [(ngModel)]="model" [options]="myDateRangePickerOptions" (dateRangeChanged)="dateCnanged($event)"></my-date-range-picker>
    </div>
  
  </div>
  
  
  <a (click)="condition = !condition" style="cursor: pointer;"><i class="fa fa-minus-square" *ngIf="condition"></i>
    <i class="fa fa-plus-square" *ngIf="!condition"></i> more search options
  </a>
  <div class="row">
    <form *ngIf="condition" class="form-horizontal form-label-left">
      <div class="col-sm-offset-3 col-sm-9">
        <h2>Refine Search</h2>
      </div>
      <div class="form-group" >
          <label class="control-label col-md-3 col-sm-3 col-xs-12">Day of the week</label>
          <div class="col-md-6 col-sm-6 col-xs-12">
              <select [(ngModel)]="criteria.day" class="form-control" (change)="onDayChange($event)" name="dayOfTheWeek">
                  <option value="null">-- Select --</option>
                  <option value="1">Monday</option>
                  <option value="2">Tuesday</option>
                  <option value="3">Wednesday</option>
                  <option value="4">Thursday</option>
                  <option value="5">Friday</option>
                  <option value="6">Saturday</option>
                  <option value="0">Sunday</option>
              </select>
          </div>
      </div>
      <div class="form-group" >
          <label class="control-label col-md-3 col-sm-3 col-xs-12">Contact</label>
          <div class="col-md-6 col-sm-6 col-xs-12">
              <input class="form-control" [(ngModel)]="criteria.contacts" name="contact" (keyup)="onSearchContact($event)" />
          </div>
      </div>
      <div class="form-group" >
          <label class="control-label col-md-3 col-sm-3 col-xs-12">Order by</label>
          <div class="col-md-6 col-sm-6 col-xs-12">
              <div class="btn-group" data-toggle="buttons">
                  <label class="btn btn-default" [class.active]="type=='dsc'">
                  <input type="radio" name="type" id="option2" (click)="switchOrder('dsc')"> Date Descending
                  </label>
                  <label class="btn btn-default" [class.active]="type=='asc'">
                    <input type="radio" name="type" id="option3" (click)="switchOrder('asc')"> Date Ascending
                  </label>
                  <label class="btn btn-default" [class.active]="type=='alph'">
                      <input type="radio" name="type" id="option4" (click)="switchOrder('alph')"> Alphabetically
                    </label>
              </div>
          </div>
      </div>  
      
      
      
    </form>
  </div><br>
  
  
  
  <div class="table-responsive">
    
    <table class="table table-bordered table-striped" *ngIf="trainings$ | async as trainings">
        <thead>
            <tr>
              <th>Date(s)</th>
              <th>Title</th>
              <th>Location</th>
              <th>Time</th>
              <th></th>
            </tr>
        </thead>
        <tbody *ngIf="!loading>
          <tr [meeting-detail]="training" [criteria]="criteria" (onDeleted)="deletedTraining($event)" *ngFor="let training of trainings"></tr>
        </tbody>
    </table>
    <loading *ngIf="loading"></loading>
  </div>
  `,
  styles: []
})
export class MeetingListComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  meetings$:Observable<Meeting[]>;
  type="dsc";

  @Input() criteria:TrainingSearchCriteria;
  @Input() startDate:Date;
  @Input() endDate:Date;

  condition = false;


  myDateRangePickerOptions: IMyDrpOptions = {
      // other options...
      dateFormat: 'mmm dd, yyyy',
      showClearBtn: false,
      showApplyBtn: false,
      showClearDateRangeBtn: false
  };
  model = {beginDate: {year: 2018, month: 10, day: 9},
                             endDate: {year: 2018, month: 10, day: 19}};

  constructor(
    private service:MeetingService
  ) { }

  ngOnInit() {
    //Criterias are NOT passed
    if( this.startDate == null){
      this.startDate = new Date();
    }
    if( this.endDate == null ){
      this.endDate = new Date();
      this.endDate.setMonth( this.endDate.getMonth() + 5);
    }
    
    this.criteria = {
      start: this.startDate.toISOString(),
      end: this.endDate.toISOString(),
      search: "",
      status: 'all',
      contacts: "",
      day: null,
      order: 'dsc',
      withseats: false,
      attendance: false
    }

    this.model.beginDate = {year: this.startDate.getFullYear(), month: this.startDate.getMonth() + 1, day: this.startDate.getDate()};
    this.model.endDate = {year: this.endDate.getFullYear(), month: this.endDate.getMonth() + 1, day: this.endDate.getDate()};

    this.refresh = new Subject();

    this.meetings$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }



  dateCnanged(event: IMyDateRangeModel){
    this.startDate = event.beginJsDate;
    this.endDate = event.endJsDate;
    this.criteria["start"] = event.beginJsDate.toISOString();
    this.criteria["end"] = event.endJsDate.toISOString();
    this.onRefresh();
    //this.trainings$ = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  onSearchContact(event){
    this.criteria["contacts"] = event.target.value;
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  deletedTraining(_: any){
    this.onRefresh();
  }
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  onSeatsChange(event){
    this.criteria["withseats"] = event.target.checked;
    this.onRefresh();
  }

  onDayChange(event){
    this.criteria["day"] = event.target.value;
    this.onRefresh();
  }





}




/* 

  dateCnanged(event: IMyDateRangeModel){
    this.startDate = event.beginJsDate;
    this.endDate = event.endJsDate;
    this.criteria["start"] = event.beginJsDate.toISOString();
    this.criteria["end"] = event.endJsDate.toISOString();
    this.onRefresh();
    //this.trainings$ = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  onSearchContact(event){
    this.criteria["contacts"] = event.target.value;
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  deletedTraining(_: any){
    this.onRefresh();
  }
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  onSeatsChange(event){
    this.criteria["withseats"] = event.target.checked;
    this.onRefresh();
  }

  onDayChange(event){
    this.criteria["day"] = event.target.value;
    this.onRefresh();
  }

}
 */



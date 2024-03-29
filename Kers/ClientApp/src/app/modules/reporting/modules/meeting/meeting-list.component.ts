import { Component, OnInit, Input } from '@angular/core';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { Observable, Subject } from 'rxjs';
import { startWith, flatMap, delay, map, tap } from 'rxjs/operators';
import { TrainingSearchCriteria } from '../training/training';
import { MeetingService, Meeting, MeetingWithTime } from './meeting.service';

@Component({
  selector: 'meeting-list',
  template: `
  <div>
      <div class="text-right">
          <a class="btn btn-info btn-xs" *ngIf="!newMeeting" (click)="newMeeting = true">+ new event</a>
      </div>
      <meeting-form *ngIf="newMeeting" (onFormCancel)="newMeeting=false" (onFormSubmit)="newMeetingSubmitted($event)"></meeting-form>
  </div><br><br>
  <div class="row">
    <div class="col-sm-6 col-xs-12" style="margin-top: 3px;">
      <input type="text" [(ngModel)]="criteria.search" placeholder="search by title" (keyup)="onSearch($event)" class="form-control" name="Search" />
    </div>
    <div class="col-sm-4 col-xs-8 pull-right" style="margin-top: 3px;">
        


        <div class="input-group" style="width:260px; float:right;">
          
          <input type="text" class="form-control input-box" placeholder="Click to select a date" 
            angular-mydatepicker name="mydate" (click)="dp.toggleCalendar()" 
            [(ngModel)]="model" [options]="myDpOptions" 
            #dp="angular-mydatepicker" (dateChanged)="onDateChanged($event)">
          <span class="input-group-addon" id="basic-addon1" (click)="dp.toggleCalendar()" style="cursor: pointer;"><i class="fa fa-calendar"></i></span>
        </div>
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
    
    <table class="table table-bordered table-striped" *ngIf="meetings$ | async as trainings">
        <thead>
            <tr>
              <th>Date(s)</th>
              <th>Title</th>
              <th>Location</th>
              <th>Contact</th>
              <th></th>
            </tr>
        </thead>
        <tbody *ngIf="!loading">
          <tr [meeting-list-detail]="training" (onEdited)="editedMeeting($event)" (onDeleted)="deletedMeeting($event)" *ngFor="let training of trainings"></tr>
        </tbody>
    </table>
    <loading *ngIf="loading"></loading>
  </div>
  `,
  styles: [`
  .input-box-container {
    position: relative;
  }
  .input-box {
    padding: 4px 8px;
    border-radius: 4px;
    border: 1px solid #ccc;
    font-size: 16px;
  }
  `]
})
export class MeetingListComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  meetings$:Observable<MeetingWithTime[]>;
  type="dsc";
  newMeeting = false;

  @Input() criteria:TrainingSearchCriteria;
  @Input() startDate:Date;
  @Input() endDate:Date;

  condition = false;

  myDpOptions: IAngularMyDpOptions = {
    dateRange: true,
    dateFormat: 'mmm dd, yyyy',
    alignSelectorRight: true
    // other options are here...
  };



  onDateChanged(event: IMyDateModel): void {
    this.startDate = event.dateRange.beginJsDate;
    this.endDate = event.dateRange.endJsDate;
    this.criteria["start"] = event.dateRange.beginJsDate.toISOString();
    this.criteria["end"] = event.dateRange.endJsDate.toISOString();
    this.onRefresh();
  }

  
  model: IMyDateModel = null;

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
      attendance: false,
      admin: false,
      core:false
    }

    //this.model.beginDate = {year: this.startDate.getFullYear(), month: this.startDate.getMonth() + 1, day: this.startDate.getDate()};
    //this.model.endDate = {year: this.endDate.getFullYear(), month: this.endDate.getMonth() + 1, day: this.endDate.getDate()};


    // Initialize to specific date range with IMyDate object. 
        // Begin date = today. End date = today + 3.
        let begin: Date = this.startDate;
        let end: Date = this.endDate;
        this.model = {
                        isRange: true, 
                        singleDate: null, 
                        dateRange: {
                          beginDate: {
                            year: begin.getFullYear(), month: begin.getMonth() + 1, day: begin.getDate()
                          },
                          endDate: {
                            year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()
                          }
                        }
                      };




    this.refresh = new Subject();

    this.meetings$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }


/* 
  dateCnanged(event: IMyDateRangeModel){
    this.startDate = event.beginJsDate;
    this.endDate = event.endJsDate;
    this.criteria["start"] = event.beginJsDate.toISOString();
    this.criteria["end"] = event.endJsDate.toISOString();
    this.onRefresh();
    //this.trainings$ = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  } */

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

  newMeetingSubmitted($event:MeetingWithTime){
    this.newMeeting=false;
    this.onRefresh();
  }
  editedMeeting($event){
    this.onRefresh();
  }
  deletedMeeting($event){
    this.onRefresh();
  }
}
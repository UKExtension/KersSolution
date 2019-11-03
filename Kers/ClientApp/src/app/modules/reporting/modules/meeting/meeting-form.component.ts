import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Meeting, MeetingService, MeetingWithTime } from './meeting.service';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { FormBuilder, Validators, AbstractControl } from '@angular/forms';

@Component({
  selector: 'meeting-form',
  template: `
<loading *ngIf="loading"></loading>

<div class="row" *ngIf="!loading">
  <div class="col-sm-offset-3 col-sm-9">
      <h2 *ngIf="!meeting">New CES Event</h2>
      <h2 *ngIf="meeting">Update CES Event</h2>
      <br><br>
  </div>
  <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="meetingForm">
    <div class="form-group">
        <label class="control-label col-md-3 col-sm-3 col-xs-12" for="start">Start Date:</label>
        <div class="col-md-4 col-sm-6 col-xs-7">
            <my-date-picker [options]="myDatePickerOptions" (dateChanged)="onDateChanged($event)" formControlName="start"></my-date-picker>
        </div>
        <label><input type="checkbox" formControlName="isAllDay" /> All Day Event</label>
    </div>
    <div class="form-group">
      <div *ngIf="!meetingForm.value.isAllDay" class="col-md-offset-3 col-sm-offset-3 col-md-4 col-sm-6 col-xs-7">
            <timepicker formControlName="starttime" [start]="6" [end]="23"></timepicker>
      </div>
    </div>
    <div *ngIf="!meetingForm.value.isAllDay">
      <div class="form-group">
          <label class="control-label col-md-3 col-sm-3 col-xs-12" for="end">End Date:</label>
          <div class="col-md-4 col-sm-6 col-xs-7">
              <my-date-picker [class.ng-invalid]="meetingForm.hasError('endDate')" [options]="myDatePickerOptionsEnd" (dateChanged)="onDateChanged($event)" formControlName="end"></my-date-picker>
          </div>
      </div>
    </div>
    <div class="form-group" *ngIf="!meetingForm.value.isAllDay">
      <div class="col-md-offset-3 col-sm-offset-3 col-md-4 col-sm-6 col-xs-7">
            <timepicker formControlName="endtime" [start]="6" [end]="23"></timepicker>
      </div>
    </div>
    <div class="form-group" *ngIf="!meetingForm.value.isAllDay">
        <label class="control-label col-md-3 col-sm-3 col-xs-12" for="etimezone">Timezone:</label>
        <div class="col-md-5 col-sm-7 col-xs-8">
                <div class="btn-group" data-toggle="buttons">
                    <label class="btn btn-default" (click)="isEastern(true)" [class.active]="easternTimezone">
                    <input type="radio" name="etimezone" formControlName="etimezone" [value]="true"> Eastern Timezone
                    </label>
                    <label class="btn btn-default" [class.active]="!easternTimezone" (click)="isEastern(false)">
                    <input type="radio" name="etimezone" formControlName="etimezone" [value]="false"> Central Timezone
                    </label>
                </div>
        </div>
    </div>
    <div class="form-group">
        <label for="subject" class="control-label col-md-3 col-sm-3 col-xs-12">Title:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="text" name="subject" formControlName="subject" id="subject" class="form-control col-xs-12" />
        </div>
    </div>
    <div class="form-group">
        <label for="body" class="control-label col-md-3 col-sm-3 col-xs-12">Description:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12" [class.description-invalid]="!meetingForm.controls.body.valid">
            <textarea [froalaEditor]="options" name="body" formControlName="body" id="body" class="form-control col-xs-12"></textarea>
        </div>
    </div>
    <div class="form-group">
        <label for="tContact" class="control-label col-md-3 col-sm-3 col-xs-12">Contact:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="text" name="tContact" formControlName="tContact" id="tContact" class="form-control col-xs-12" />
        </div>
    </div>
    <div class="form-group">
        <label for="tLocation" class="control-label col-md-3 col-sm-3 col-xs-12">Location:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="text" name="tLocation" formControlName="tLocation" id="tLocation" class="form-control col-xs-12" />
        </div>
    </div> 
    <div class="form-group" *ngIf="meeting != null">
        <label for="subject" class="control-label col-md-3 col-sm-3 col-xs-12">Canceled:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12">
            <input type="checkbox" name="isCancelled" formControlName="isCancelled" id="isCancelled" style="margin-top: 10px;" />
        </div>
    </div>  
    <div class="ln_solid"></div>
    <div class="form-group">
        <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
            <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
            <button type="submit" [disabled]="meetingForm.invalid"  class="btn btn-success">Submit</button>
        </div>
    </div>
      
  </form>
</div>
  `,
  styles: []
})
export class MeetingFormComponent implements OnInit {
  
  @Input() meeting:MeetingWithTime;
  date = new Date();
  meetingForm:any;
  isAllDay = true;
  easternTimezone = true;
  

  options = { 
    placeholderText: 'Your Description Here!',
    toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
    toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
    toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
    toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
    quickInsertButtons: ['ul', 'ol', 'hr'],    
  };
  loading = true;
  public myDatePickerOptions: IMyDpOptions = {
          dateFormat: 'mm/dd/yyyy',
          showTodayBtn: false,
          satHighlight: true,
          firstDayOfWeek: 'su',
          showClearDateBtn: false
      };
  public myDatePickerOptionsEnd: IMyDpOptions = {
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true,
            firstDayOfWeek: 'su'
        };
    
  defaultTime = "12:34:56.1000000 -04:00";
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Meeting>();

  constructor(
    private fb: FormBuilder,
    private service:MeetingService
  ) {
    this.date.setMonth(this.date.getMonth() + 2);
    this.meetingForm = this.fb.group(
      {
          start: [{
              date: {
                  year: this.date.getFullYear(),
                  month: this.date.getMonth() + 1,
                  day: this.date.getDate()}
              }, Validators.required],
          starttime: "",
          end: [{}],
          endtime: "",
          etimezone:true,
          subject: ["", Validators.required],
          body: [""],
          tContact: [""],
          tLocation: [""],
          isAllDay: true,
          isCancelled: false
    }, { validator: trainingValidator });
    let today = new Date();
    this.myDatePickerOptions.disableUntil = {year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()};
    
   }

  ngOnInit() {

    if(this.meeting){
      this.meetingForm.patchValue(this.meeting);
      var start = new Date( this.meeting.start);
      this.meetingForm.patchValue({
        start: {
          date:{
            year: start.getFullYear(),
            month: start.getMonth() + 1,
            day: start.getDate()
          }
        },
        etimezone: this.meeting.isAllDay ? true : this.meeting.etimezone
      })
      if( this.meeting.end ){
        var end = new Date(this.meeting.end);
        this.meetingForm.patchValue({
          end:{
            date:{
              year: end.getFullYear(),
              month: end.getMonth() + 1,
              day: end.getDate()
            }
          }
        })
      }
      this.easternTimezone = this.meeting.isAllDay ? true : this.meeting.etimezone;
    }
    this.loading = false;
  }
  onDateChanged(event: IMyDateModel) {
    
  }

  onSubmit(){
    var trning:MeetingWithTime = <MeetingWithTime> this.meetingForm.value;
    trning.start = new Date(this.meetingForm.value.start.date.year, this.meetingForm.value.start.date.month - 1, this.meetingForm.value.start.date.day);
    if( this.meetingForm.value.end != null && this.meetingForm.value.end.date != null ){
      trning.end = new Date(this.meetingForm.value.end.date.year, this.meetingForm.value.end.date.month - 1, this.meetingForm.value.end.date.day);
    }else{
      trning.end = null;
    }
    if( this.meeting == null ){
      this.loading = true;
      this.service.add( trning ).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit(res);
          this.meetingForm.reset();
        }
      );
    }else{
      this.loading = true;
      this.service.update( this.meeting.id, trning).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit( res);
        }
      )
    }
    
     
  }
  onCancel(){
    this.onFormCancel.emit();
  }
  isEastern(isIt:boolean){
    this.easternTimezone = isIt;
  }

  pad(num:number, size:number): string {
    let s = num+"";
    while (s.length < size) s = "0" + s;
    return s;
  }

}

export const trainingValidator = (control: AbstractControl): {[key: string]: boolean} => {

  let start = control.get('start');
  let end = control.get('end');

  if( end.value != null && end.value.date != null){
    let startDate = new Date(start.value.date.year, start.value.date.month - 1, start.value.date.day);
    let endDate = new Date(end.value.date.year, end.value.date.month - 1, end.value.date.day);
    if( startDate.getTime() > endDate.getTime()){
      return {"endDate":true};
    }
  }
  return null;
}



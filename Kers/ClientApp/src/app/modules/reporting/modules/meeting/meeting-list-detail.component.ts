import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Meeting, MeetingService, MeetingWithTime } from './meeting.service';

@Component({
    selector: '[meeting-list-detail]',
    template: `
<ng-container>
  @if (rowDefault) {
    <td>{{training.start | date:'mediumDate'}} @if (training.end) {
      <span><br>{{training.end | date:'mediumDate'}}</span>
    }</td>
  }
  @if (rowDefault) {
    <td>{{training.subject}}</td>
  }
  @if (rowDefault) {
    <td>{{training.tLocation}}</td>
  }
  @if (rowDefault) {
    <td>{{training.tContact}}</td>
  }
  @if (rowDefault) {
    <td class="text-right">
      @if (rowDefault) {
        <a class="btn btn-info btn-xs" (click)="edit()"><i class="fa fa-pencil"></i> Edit</a>
      }
      @if (rowDefault) {
        <a class="btn btn-info btn-xs" (click)="delete()"><i class="fa fa-trash-o"></i> Delete</a>
      }
      @if (!rowDefault) {
        <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
      }
    </td>
  }
  @if (rowEdit) {
    <td colspan="5">
      <div class="text-right">
        <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
      </div>
      <meeting-form [meeting]="training" (onFormCancel)="default()" (onFormSubmit)="trainingSubmitted($event)"></meeting-form>
    </td>
  }
  @if (rowDelete) {
    <td colspan="5">
      <div class="text-right">
        <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
      </div>
      <div>
        Do you really want to delete CES Event <strong>{{training.subject}}</strong>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
      </div>
    </td>
  }
</ng-container>


`,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class MeetingListDetailComponent implements OnInit {

  rowDefault =true;
  rowEdit = false;
  rowDelete = false;

  
  @Input('meeting-list-detail') training:MeetingWithTime;

  @Output() onDeleted = new EventEmitter<Meeting>();
  @Output() onEdited = new EventEmitter<Meeting>();

  constructor(
    private service:MeetingService
  ) { }

  ngOnInit() {
  }

  edit(){
    this.rowDefault = false;
    this.rowEdit = true;
    this.rowDelete = false;
  }
  delete(){
      this.rowDefault = false;
      this.rowEdit = false;
      this.rowDelete = true;
  }
  default(){
      this.rowDefault = true;
      this.rowEdit = false;
      this.rowDelete = false;
  }

  trainingSubmitted(training:MeetingWithTime){
      this.training = training;
      this.onEdited.emit(training);
      this.default();
  }

  confirmDelete(){   
      this.service.delete(this.training.id).subscribe(
          res=>{
              this.onDeleted.emit(this.training);
          }
      );
  }
}
/* 


import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TrainingService } from "./training.service";
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { Training, TrainingSearchCriteria } from './training';

@Component({
    selector: '[training-detail]',
    templateUrl: 'training-detail.component.html'
})
export class TrainingDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    currentFiscalYear:FiscalYear | null = null;
    displayEdit = false;

    
    @Input('training-detail') training:Training;
    @Input() admin:boolean = false;
    @Input() criteria:TrainingSearchCriteria;

    @Output() onDeleted = new EventEmitter<Training>();
    @Output() onEdited = new EventEmitter<Training>();
    
    errorMessage: string;

    constructor( 
        private service:TrainingService
    )   
    {}

    ngOnInit(){
    }
    edit(){
        this.rowDefault = false;
        this.rowEdit = true;
        this.rowDelete = false;
    }
    delete(){
        this.rowDefault = false;
        this.rowEdit = false;
        this.rowDelete = true;
    }
    default(){
        this.rowDefault = true;
        this.rowEdit = false;
        this.rowDelete = false;
    }

    trainingSubmitted(training:Training){
        this.training = training;
        this.onEdited.emit(training);
        this.default();
    }

    confirmDelete(){
        
        this.service.delete(this.training.id).subscribe(
            res=>{
                this.onDeleted.emit(this.training);
            },
            err => this.errorMessage = <any> err
        );
        
    }
    
}



*/
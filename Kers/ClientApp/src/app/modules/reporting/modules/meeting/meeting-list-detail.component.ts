import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Meeting, MeetingService } from './meeting.service';

@Component({
  selector: 'app-meeting-list-detail',
  template: `
<ng-container *ngIf="admin">
  <td *ngIf="rowDefault">{{training.start | date:'mediumDate'}} <span *ngIf="training.end"><br>{{training.end | date:'mediumDate'}}</span></td>
  <td *ngIf="rowDefault">{{training.subject}}</td>
  <td *ngIf="rowDefault">{{training.mLocation}}</td>
  <td *ngIf="rowDefault" class="text-right">
      <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault"><i class="fa fa-pencil"></i> Edit</a>
      <a class="btn btn-info btn-xs" (click)="delete()" *ngIf="rowDefault"><i class="fa fa-trash-o"></i> Delete</a>
      <a class="btn btn-primary btn-xs" (click)="default()" *ngIf="!rowDefault"><i class="fa fa-close"></i> Close</a>
  </td>
  <td *ngIf="rowEdit" colspan="6">
      <div class="text-right">
          <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
      </div>
      <meeting-form [meeting]="training" (onFormCancel)="default()" (onFormSubmit)="trainingSubmitted($event)"></meeting-form>
  </td>
  <td *ngIf="rowDelete" colspan="6">
      <div class="text-right">
          <a class="btn btn-primary btn-xs" (click)="default()"><i class="fa fa-close"></i> Close</a>
      </div>
      <div>
          Do you really want to delete training <strong>{{training.subject}}</strong>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
      </div>
  </td>
</ng-container>


  `,
  styles: []
})
export class MeetingListDetailComponent implements OnInit {

  rowDefault =true;
  rowEdit = false;
  rowDelete = false;

  
  @Input('meeting-detail') training:Meeting;

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

  trainingSubmitted(training:Meeting){
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
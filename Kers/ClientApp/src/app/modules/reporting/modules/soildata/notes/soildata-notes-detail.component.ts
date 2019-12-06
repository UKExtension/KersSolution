import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CountyNote } from '../soildata.service';

@Component({
  selector: 'soildata-notes-detail',
  template: `
  <div class="ln_solid"></div>
  <div class="row">
      <div class="col-xs-10">
          
          <article class="media event" *ngIf="rowDefault">
              <div class="media-body">
              <a class="title">{{note.name}}</a>
              </div>
          </article>
          <div class="col-xs-12" *ngIf="rowEdit">
              <soildata-notes-form [note]="note" (onFormCancel)="default()" (onFormSubmit)="noteSubmitted($event)"></soildata-notes-form>
          </div>
          <div class="col-xs-12" *ngIf="rowDelete">
          Do you really want to delete note {{note.name}}?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
          </div>

          
      </div>
      <div class="col-xs-2 text-right">
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault">edit</a>
          <a class="btn btn-info btn-xs" (click)="delete()" *ngIf="rowDefault">delete</a>
          <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
      </div>  
  </div>
  `,
  styles: []
})
export class SoildataNotesDetailComponent implements OnInit {
  @Input() note:CountyNote;

  rowDefault = true;
  rowEdit = false;
  rowDelete = false;

  @Output() onEdited = new EventEmitter<CountyNote>();
  @Output() deleted = new EventEmitter<CountyNote>();


  constructor() { }

  ngOnInit() {
  }
  edit(){
    this.rowEdit = true;
    this.rowDefault = false;
  }
  delete(){
    this.rowDelete = true;
    this.rowDefault = false;
  }
  default(){
    this.rowDefault = true;
    this.rowEdit = false;
    this.rowDelete = false;
  }

  noteSubmitted(event:CountyNote){
    this.note = event;
    this.default();
    this.onEdited.emit(event);
  }

  confirmDelete(){
    this.deleted.emit(this.note);
    this.default();
  }

}

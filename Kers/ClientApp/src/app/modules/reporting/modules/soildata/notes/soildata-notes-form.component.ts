import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CountyNote, SoildataService } from '../soildata.service';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'soildata-notes-form',
  template: `
  <loading *ngIf="loading"></loading>
  <div class="row" *ngIf="!loading">
      <div class="col-sm-offset-3 col-sm-9">
          <h2 *ngIf="!address">New Note Template</h2>
          <h2 *ngIf="address">Update Note Template</h2>
      </div>

      <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="noteForm">
          <div class="form-group">
              <label for="name" class="control-label col-md-3 col-sm-3 col-xs-12">Keyword:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="name" formControlName="name" id="name" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="note" class="control-label col-md-3 col-sm-3 col-xs-12">Note:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <textarea name="note" formControlName="note" id="note" class="form-control col-xs-12"></textarea>
              </div>
          </div>

          <div class="ln_solid"></div>
          <div class="form-group">
              <div class="col-md-6 col-sm-6 col-xs-12 col-sm-offset-3">
                  <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
                  <button type="submit" [disabled]="noteForm.invalid"  class="btn btn-success">Submit</button>
              </div>
          </div>
      </form>
  </div>
  `,
  styles: []
})
export class SoildataNotesFormComponent implements OnInit {

  @Input() note:CountyNote;
  noteForm:any;

  loading = false;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<CountyNote>();

  constructor(
    private fb: FormBuilder,
    private service:SoildataService
  ) { 
    this.noteForm = this.fb.group(
      { 
          name: ["", Validators.required],
          note: ["", Validators.required]
      });

  }

  ngOnInit() {
    if(this.note) this.noteForm.patchValue(this.note);
  }
  onCancel(){
    this.onFormCancel.emit();
  }

  onSubmit(){
      if(!this.note){
          var newNote:CountyNote = this.noteForm.value;
          console.log( newNote );
          this.service.addNote( newNote ).subscribe(
              res => {
                  this.onFormSubmit.emit(res);
              }
          );

      }else{

          var updatedNote:CountyNote = this.noteForm.value;
          console.log( updatedNote );
          this.service.updateNote(this.note.id,  updatedNote ).subscribe(
              res => {
                  this.note = res;
                  this.onFormSubmit.emit(res);
              }
          )
      }
      
  }

}

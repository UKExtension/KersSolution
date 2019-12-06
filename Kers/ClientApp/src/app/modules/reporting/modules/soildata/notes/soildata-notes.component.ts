import { Component, OnInit } from '@angular/core';
import { CountyNote, SoildataService } from '../soildata.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-soildata-notes',
  template: `
  <br>
  <h3>Report Note Templates</h3>
  <br>
  <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newNote" (click)="newNote = true">+ new note</a>
    </div>
    <soildata-notes-form *ngIf="newNote" (onFormCancel)="newNote=false" (onFormSubmit)="newNoteSubmitted($event)"></soildata-notes-form>
    <soildata-notes-detail *ngFor="let note of notes | async" [note]="note" (deleted)="noteDeleted($event)"></soildata-notes-detail>
  `,
  styles: []
})
export class SoildataNotesComponent implements OnInit {
  newNote = false;
  notes:Observable<CountyNote[]>;

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    this.notes = this.service.notesByCounty();
  }
  newNoteSubmitted(_:CountyNote){
    this.newNote = false;
    this.notes = this.service.notesByCounty();
  }

  noteDeleted(event:CountyNote){
    this.service.deleteNote(event.id).subscribe(
      _ => this.notes = this.service.notesByCounty()
    )

  }

}

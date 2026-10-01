import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CountyCode, CountyNote, SoildataService } from '../soildata.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'app-soildata-notes',
    template: `
  <br>
    <h3>Report Note Templates</h3>
    <br>
      <div class="text-right">
        @if (!newNote) {
          <a class="btn btn-info btn-xs" (click)="newNote = true">+ new note</a>
        }
      </div>
      @if (newNote) {
        <soildata-notes-form (onFormCancel)="newNote=false" (onFormSubmit)="newNoteSubmitted($event)"></soildata-notes-form>
      }
      @for (note of notes | async; track note) {
        <soildata-notes-detail [note]="note" (deleted)="noteDeleted($event)"></soildata-notes-detail>
      }
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SoildataNotesComponent implements OnInit {
  newNote = false;
  notes:Observable<CountyNote[]>;
  selectedCounty:CountyCode;

  constructor(
    private service:SoildataService
  ) { 
    this.service.selectedCountyChange.subscribe(
      res => {
        this.selectedCounty = res;
        this.notes = this.service.notesByCounty(this.selectedCounty.planningUnitId);
      }
    );


  }

  ngOnInit() {
    if(this.selectedCounty == null ){
      this.selectedCounty = this.service.selectedCountyCode;
      this.notes = this.service.notesByCounty(this.selectedCounty.planningUnitId);
    }
      

    
    
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

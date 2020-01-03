import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { SoilReport, TestResults } from '../soildata.report';
import { Observable } from 'rxjs';
import { SoildataService, CountyNote } from '../soildata.service';
import { FormBuilder, Validators } from '@angular/forms';
import { load } from '@angular/core/src/render3';

@Component({
  selector: 'soildata-report-crop',
  templateUrl: './soildata-report-crop.component.html',
  styles: [`
  .crop-comment{
    margin-top: 11px;
  }
  `]
})
export class SoildataReportCropComponent implements OnInit {
  @Input() crop:SoilReport;
  condition=false;
  testResults:Observable<TestResults[]>;
  loading = false;
  noteForm:any;
  notes:Observable<CountyNote[]>;


  constructor(
    private fb: FormBuilder,
    private service:SoildataService
  ) { 
    this.noteForm = this.fb.group(
      { 
          note: ["", Validators.required]
      });
  }
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<string>();

  ngOnInit() {
    this.testResults = this.service.labResults(this.crop.id);
    this.notes = this.service.notesByCounty();
    if( this.crop.agentNote != undefined) this.noteForm.patchValue({'note':this.crop.agentNote});
  }

  onCancel(){
    this.onFormCancel.emit();
    this.condition = false;
  }

  onNoteChange(event:string){
    var currentNote = this.noteForm.value.note;
    this.noteForm.patchValue({'note':currentNote + (currentNote == ""?"":"\n") + event});
  }

  onSubmit(){
    this.loading = true;
    this.crop.agentNote = this.noteForm.value.note;
    this.service.updateCropNote(this.crop.id, this.crop ).subscribe(
      res => {
        this.loading = false;
        this.condition = false;
        this.onFormSubmit.emit(res.agentNote);
      }
    )

    /* 
      if(!this.crop.agentNote){
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
       */
  }

}

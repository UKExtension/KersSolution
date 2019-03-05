import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Training } from './training';
import { TrainingService } from './training.service';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";

@Component({
  selector: 'training-form',
  templateUrl: './training-form.component.html'
})
export class TrainingFormComponent implements OnInit {
    @Input() training:Training;

    date = new Date();
    trainingForm = this.fb.group(
        {
            start: [{
                date: {
                    year: this.date.getFullYear(),
                    month: this.date.getMonth() + 1,
                    day: this.date.getDate()}
                }, Validators.required],
            end: [{}],
            subject: ["", Validators.required],
            body: [""],
            tAudience: [""],
            tContact: [""],
            tLocation: [""],
      });;
    options = { 
        placeholderText: 'Your Description Here!',
        toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
        toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
        quickInsertButtons: ['ul', 'ol', 'hr'],    
      };
    loading = true;
    private myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true,
            firstDayOfWeek: 'su'
        };

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Training>();

  constructor(
    private fb: FormBuilder,
    private service:TrainingService
  ) { }

  ngOnInit() {
    //this.trainingForm.patchValue(this.training);
    this.loading = false;
  }


  onDateChanged(event: IMyDateModel) {
    
    }

  onSubmit(){
      /* 
    this.loading = true;
    this.service.update( this.county.id, this.countyForm.value).subscribe(
      res => {
        this.loading = false;
        this.onFormSubmit.emit(res);
      }
    );
     */
  }
  onCancel(){
    this.onFormCancel.emit();
  }

}
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Training, TainingInstructionalHour, TrainingCancelEnrollmentWindow, TainingRegisterWindow } from './training';
import { TrainingService } from './training.service';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";
import { Observable } from 'rxjs';

@Component({
  selector: 'training-form',
  templateUrl: './training-form.component.html'
})
export class TrainingFormComponent implements OnInit {
    @Input() training:Training;
    iHours: Observable<TainingInstructionalHour[]>;
    cancelWindow: Observable<TrainingCancelEnrollmentWindow[]>;
    registerWindow: Observable<TainingRegisterWindow[]>;

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
            day1: ["", Validators.required],
            day2: [""],
            day3: [""],
            day4: [""],
            iHourId: "",
            cancelCutoffDaysId: "",
            registerCutoffDaysId: "",
            seatLimit: ""
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
      

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Training>();

  constructor(
    private fb: FormBuilder,
    private service:TrainingService
  ) {
    this.iHours = service.instructionalHours();
    this.cancelWindow = service.cancelEnrollmentWindows();
    this.registerWindow = service.registerWindows();
   }

  ngOnInit() {
    //this.trainingForm.patchValue(this.training);
    this.loading = false;
  }


  onDateChanged(event: IMyDateModel) {
    
  }

  onSubmit(){
    console.log(this.trainingForm.value)
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

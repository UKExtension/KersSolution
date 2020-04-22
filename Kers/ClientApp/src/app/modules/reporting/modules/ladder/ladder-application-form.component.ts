import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { FormBuilder, Validators } from '@angular/forms';
import { TrainingService } from '../training/training.service';

@Component({
  selector: 'ladder-application-form',
  templateUrl: './ladder-application-form.component.html',
  styles: []
})
export class LadderApplicationFormComponent implements OnInit {

    ratings: {
      year:number;
      rating:string;
    }[];
  
    date = new Date();
    trainingForm:any;

    proposed = false;
    coppied = false;

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
  @Output() onFormSubmit = new EventEmitter<void>();

  constructor(
    private fb: FormBuilder,
    private service:TrainingService
  ) {
    this.date.setMonth(this.date.getMonth() + 2);
    this.trainingForm = this.fb.group(
      {
          start: [{
              date: {
                  year: this.date.getFullYear() - 3,
                  month: this.date.getMonth() + 1,
                  day: this.date.getDate()}
              }, Validators.required],
          end: [{}],
          subject: [""],
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
          seatLimit: "",
          tStatus: "P",
          demoArray: this.fb.array([])
    },);
    let today = new Date();
    this.myDatePickerOptions.disableUntil = {year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()};

   }

  ngOnInit() {
    
    this.loading = false;
    this.ratings = [];
    this.addRating();
  }

  addRating() {
    this.ratings.push({year:2019, rating:""});
  }


  onDateChanged(event: IMyDateModel) {
    
  }

  onSubmit(){
    
    
     
  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

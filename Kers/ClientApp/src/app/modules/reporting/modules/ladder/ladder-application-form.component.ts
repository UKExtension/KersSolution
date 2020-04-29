import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { LadderService } from './ladder.service';
import { LadderLevel, LadderEducationLevel, LadderPerformanceRating } from './ladder';
import { Observable } from 'rxjs';

@Component({
  selector: 'ladder-application-form',
  templateUrl: './ladder-application-form.component.html',
  styles: []
})
export class LadderApplicationFormComponent implements OnInit {

    //ratings: LadderPerformanceRating[];
    get ratings() {
      return this.ladderForm.get('ratings') as FormArray;
    }
  
    date = new Date();
    ladderForm:any;
    levels:Observable<LadderLevel[]>;
    educationLevels:Observable<LadderEducationLevel[]>;

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
    private service:LadderService
  ) {
        this.ladderForm = this.fb.group(
        {
          ladderLevelId: [],
          lastPromotion: [{
              date: {
                  year: this.date.getFullYear() - 3,
                  month: this.date.getMonth() + 1,
                  day: this.date.getDate()}
              }, Validators.required],
          startDate: [{
                date: {
                    year: this.date.getFullYear() - 3,
                    month: this.date.getMonth() + 1,
                    day: this.date.getDate()}
                }, Validators.required],
          positionNumber: [""],
          ladderEducationLevelId:[],
          programOfStudy: [""],
          evidence: [""],
          numberOfYears: [""],
          ratings: this.fb.array([])
    },);
    let today = new Date();
    this.myDatePickerOptions.disableUntil = {year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()};

   }

  ngOnInit() {
    this.levels = this.service.levels();
    this.educationLevels = this.service.educationLevels();
    this.loading = false;
    this.addRating();
  }

  addRating() {
    const group = new FormGroup({
      year: new FormControl(''),
      ratting: new FormControl('')
    });
    this.ratings.push(group);
    //this.ratings.push({year:"", ratting:""} as LadderPerformanceRating);
  }
  removeRating(i:number){
    this.ratings.removeAt(i);
  }


  onDateChanged(event: IMyDateModel) {
    
  }

  onSubmit(){
    
    
     
  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

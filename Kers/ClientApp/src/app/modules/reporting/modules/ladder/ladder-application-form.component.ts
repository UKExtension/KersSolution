import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { LadderService } from './ladder.service';
import { LadderLevel, LadderEducationLevel, LadderPerformanceRating } from './ladder';
import { Observable } from 'rxjs';
import { TrainingService } from '../training/training.service';

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
  
    ladderForm:any;
    levels:Observable<LadderLevel[]>;
    educationLevels:Observable<LadderEducationLevel[]>;
    trainingDetails = false;

    lastPromotionDate:Date;
    hoursAttended:Observable<number>;

    today:Date;
    firstOfTheYear:Date;

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
    private service:LadderService,
    private trainingService:TrainingService
  ) {
        this.today = new Date();
        this.ladderForm = this.fb.group(
        {
          ladderLevelId: ["", Validators.required],
          lastPromotion: [{
              date: {
                  year: this.today.getFullYear() - 3,
                  month: 7,
                  day: 1}
              }, Validators.required],
          startDate: [{
                date: {
                    year: this.today.getFullYear() - 5,
                    month: 1,
                    day: 1}
                }, Validators.required],
          positionNumber: [""],
          ladderEducationLevelId:["", Validators.required],
          programOfStudy: [""],
          evidence: [""],
          numberOfYears: [""],
          ratings: this.fb.array([])
    },);
    
    this.myDatePickerOptions.disableSince = {year: this.today.getFullYear(), month: this.today.getMonth() + 1, day: this.today.getDate()};
    this.lastPromotionDate = new Date(this.today.getFullYear() -3, 6, 1);
    this.firstOfTheYear = new Date(this.today.getFullYear(), 0, 1)
   }

  ngOnInit() {
    this.levels = this.service.levels();
    this.educationLevels = this.service.educationLevels();
    this.loading = false;
    this.addRating();
    this.updateHours();
  }
  updateHours(){
    this.hoursAttended = this.trainingService.hoursByUser(0,this.lastPromotionDate, this.firstOfTheYear);
  }

  addRating() {
    const group = new FormGroup({
      year: new FormControl(''),
      ratting: new FormControl('')
    });
    this.ratings.push(group);
  }
  removeRating(i:number){
    this.ratings.removeAt(i);
  }


  onDateChanged(event: IMyDateModel) {
    this.lastPromotionDate = event.jsdate;
    this.updateHours();
    this.trainingDetails = false;
  }

  onSubmit(){
    
    console.log(this.ladderForm.value);
     
  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

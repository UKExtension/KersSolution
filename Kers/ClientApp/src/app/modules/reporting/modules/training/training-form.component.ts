import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { Training, TainingInstructionalHour, TrainingCancelEnrollmentWindow, TainingRegisterWindow } from './training';
import { TrainingService } from './training.service';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";
import { Observable } from 'rxjs';

@Component({
  selector: 'training-form',
  templateUrl: './training-form.component.html',
  styles:[`
  my-date-picker.ng-invalid.ng-touched >>> .mydp {
    border: 1px solid #CE5454;
  }
  `]
})
export class TrainingFormComponent implements OnInit {
    @Input() training:Training;
    iHours: Observable<TainingInstructionalHour[]>;
    cancelWindow: Observable<TrainingCancelEnrollmentWindow[]>;
    registerWindow: Observable<TainingRegisterWindow[]>;

    date = new Date();
    trainingForm:any;

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
    this.date.setMonth(this.date.getMonth() + 2);
    this.trainingForm = this.fb.group(
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
    }, { validator: trainingValidator });
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
    var trning:Training = <Training> this.trainingForm.value;
    trning.start = new Date(this.trainingForm.value.start.date.year, this.trainingForm.value.start.date.month - 1, this.trainingForm.value.start.date.day);
    if( this.trainingForm.value.end != null && this.trainingForm.value.end.date != null ){
      trning.end = new Date(this.trainingForm.value.end.date.year, this.trainingForm.value.end.date.month - 1, this.trainingForm.value.end.date.day);
    }else{
      trning.end = null;
    }
    if( this.training == null ){
      this.loading = true;
      this.service.add( trning ).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit(res);
        }
      );
    }
    
     
  }
  onCancel(){
    this.onFormCancel.emit();
  }
  

}



export const trainingValidator = (control: AbstractControl): {[key: string]: boolean} => {

  let start = control.get('start');
  let end = control.get('end');

  if( end.value != null && end.value.date != null){
    let startDate = new Date(start.value.date.year, start.value.date.month - 1, start.value.date.day);
    let endDate = new Date(end.value.date.year, end.value.date.month - 1, end.value.date.day);
    if( startDate.getTime() > endDate.getTime()){
      return {"endDate":true};
    }
  }

  /* 
 

  

  var isExpenseValid = true;

  let expenseFundingSource = control.get('fundingSourceNonMileageId');
  
  let registration = control.get('registration');
  let lodging = control.get('lodging');
  let mealRateBreakfast = control.get('mealRateBreakfastId');
  let mealRateLunch = control.get('mealRateLunchId');
  let mealRateDinner = control.get('mealRateDinnerId');
  let otherExpenseCost = control.get('otherExpenseCost');
  let vehicleTypeControl = control.get('vehicleType');
  let vehicleIdControl = control.get('countyVehicleId');

  let mileageAmount = control.get('mileage');
  let mileageFundingSource = control.get('fundingSourceMileageId');
  
  
  if(             !( mileageAmount.value == "" ||  mileageAmount.value == 0) 
              && 
                  mileageFundingSource.value == ""
              && 
              vehicleTypeControl.value != 2
              
              ){
      error["noMileageSource"] = true;
      hasError = true;
  }

  if( 
     ( !(registration.value == "" || registration.value == 0)
      || !(lodging.value == "" || lodging.value == 0)
      || !(otherExpenseCost.value == "" || otherExpenseCost.value == 0)
      || mealRateBreakfast.value != ""
      || mealRateLunch.value != ""
      || mealRateDinner.value != "" )
      && expenseFundingSource.value == ""   
  
  ){
      isExpenseValid = false;
  }
  
  if(vehicleTypeControl.value == 2 && vehicleIdControl.value == ""){
      error["noVehicleSelected"] = true;
      hasError = true;
  }

  if(!isExpenseValid){
      error["noExpenseSource"] = true;
      hasError = true;
  }
  if(hasError){
      return error;
  }
 */
  return null;
};

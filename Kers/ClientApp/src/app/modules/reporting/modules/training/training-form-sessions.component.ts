import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, AbstractControl, FormArray, FormControl } from '@angular/forms';
import { Training, TainingInstructionalHour, TrainingCancelEnrollmentWindow, TainingRegisterWindow, TrainingSession } from './training';
import { TrainingService } from './training.service';
import { Observable } from 'rxjs';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';


@Component({
  selector: 'training-form-sessions',
  templateUrl: './training-form-sessions.component.html',
  styles:[`
  my-date-picker.ng-invalid.ng-touched >>> .mydp {
    border: 1px solid #CE5454;
  }
  `]
})
export class TrainingFormSessionsComponent implements OnInit {
    @Input() training:Training;
    iHours: Observable<TainingInstructionalHour[]>;
    cancelWindow: Observable<TrainingCancelEnrollmentWindow[]>;
    registerWindow: Observable<TainingRegisterWindow[]>;

    date = new Date();
    trainingForm:any;

    proposed = false;
    easternTimezone = true;

    sessionsIndex = 0;

    get sessions() {
        return this.trainingForm.get('trainingSession') as FormArray;
    }
    options = { 
        placeholderText: 'Your Description Here!',
        toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
        toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
        quickInsertButtons: ['ul', 'ol', 'hr'],    
      };
    loading = true;
    public myDatePickerOptions: IAngularMyDpOptions = {
            dateFormat: 'mm/dd/yyyy',
            satHighlight: true,
            firstDayOfWeek: 'su'
        };
    public myDatePickerOptionsEnd: IAngularMyDpOptions = {
              dateFormat: 'mm/dd/yyyy',
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
    this.initialiseForm();
    let today = new Date();
    this.myDatePickerOptions.disableUntil = {year: today.getFullYear(), month: today.getMonth() + 1, day: today.getDate()};
    this.iHours = service.instructionalHours();
    this.cancelWindow = service.cancelEnrollmentWindows();
    this.registerWindow = service.registerWindows();
   }

   initialiseForm(){
    this.sessionsIndex = 0;
    this.trainingForm = this.fb.group(
      {
          trainingSession: this.fb.array([]),
          etimezone:true,
          subject: ["", Validators.required],
          body: [""],
          tAudience: [""],
          tContact: [""],
          tLocation: [""],
          iHourId: "",
          cancelCutoffDaysId: "",
          registerCutoffDaysId: "",
          seatLimit: "",
          tStatus: "P",
          isCore: [false]
    });
   }

  ngOnInit() {
    if(this.training){
      
      this.training.trainingSession = this.training.trainingSessionWithTimes;
      this.training.trainingSession.forEach( (item:TrainingSession) => {
        item = this.populateSession(item);
        this.addSession(item);
      });
      this.trainingForm.patchValue(this.training);
      this.easternTimezone = this.training.etimezone;
    }else{
      this.addSession();
    }
      
    this.loading = false;
  }

  populateSession(session:TrainingSession):TrainingSession{
    
    var start = new Date(<Date>session.date);
    let model: IMyDateModel = {isRange: false, singleDate: {jsDate: start}, dateRange: null};
    session.date = model;
    return session;
  }

  addSession(session:TrainingSession | null = null) {
    var nextDate = new Date();
    if(this.sessions.length > 0){
      var last = this.sessions.value[this.sessions.length - 1];
      var dt = last.date.singleDate.jsDate;
      nextDate = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate() + 1);
    }else{
      nextDate.setMonth(nextDate.getMonth() + 2)
    }
    if(session == null){
      let model: IMyDateModel = {isRange: false, singleDate: {jsDate: nextDate}, dateRange: null};
      var control = this.fb.control({
        date: model,
        note: "",
        starttime: "",
        endtime: "",
        index: this.sessionsIndex++
      });
      let t = this;
      setTimeout(() => {
        t.sessions.push(control);
      }, 0);
      
    }else{
      this.sessions.push(this.fb.control(session));
    }
    
  }
  sessionRemoved(event:number){
    if(this.sessions != undefined && this.sessions.length > 1){
      var elementIndex:number = undefined;
      this.sessions.controls.forEach( (item, index) => {
        if(item.value.index == event){
          elementIndex = index;
        }
      });
      if(elementIndex != undefined){
        this.sessions.removeAt(elementIndex);
        this.sessionsIndex--;
        this.sessions.controls.forEach( (item, index) => {
          if( item.value.index > elementIndex) item.value.index--;
        });
      } 




    }
  }
  isEastern(isIt:boolean){
    this.easternTimezone = isIt;
  }

  onDateChanged(event: IMyDateModel) {
    
  }

  onSubmit(){

    

    
    var trning:Training = <Training> this.trainingForm.value;
    var sessionIndex = 0;
    var sessionsWithTime = new Array<TrainingSession>();
    for( var sess of trning.trainingSession){
      var dt = this.trainingForm.value.trainingSession[sessionIndex].date.singleDate.jsDate;
      sess.date = dt;
      //sess.date = new Date(this.trainingForm.value.trainingSession[sessionIndex].date.date.year, this.trainingForm.value.trainingSession[sessionIndex].date.date.month - 1, this.trainingForm.value.trainingSession[sessionIndex].date.date.day);
      sessionIndex++;
      sessionsWithTime.push(sess);
    }
    trning.trainingSessionWithTimes = sessionsWithTime;
/* 
    trning.start = new Date(this.trainingForm.value.start.date.year, this.trainingForm.value.start.date.month - 1, this.trainingForm.value.start.date.day);
    if( this.trainingForm.value.end != null && this.trainingForm.value.end.date != null ){
      trning.end = new Date(this.trainingForm.value.end.date.year, this.trainingForm.value.end.date.month - 1, this.trainingForm.value.end.date.day);
    }else{
      trning.end = null;
    } */
    if( this.training == null ){
      this.loading = true;
      this.service.add( trning ).subscribe(
        res => {
          
          this.loading = false;
          this.onFormSubmit.emit(res);
          
          this.proposed = true;
          //console.log(this.trainingForm);
          //this.trainingForm.reset();
          this.initialiseForm();
          this.addSession();
        }
      );
    }else{
      this.loading = true;
      this.service.updateSessions( this.training.id, trning).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit( res);
        }
      )
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
  return null;
};

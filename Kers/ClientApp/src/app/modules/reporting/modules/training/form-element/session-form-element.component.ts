import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormGroup, Validators, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TrainingSession } from '../training';
import { IAngularMyDpOptions, IMyDateModel} from 'angular-mydatepicker';



@Component({
  selector: 'training-session',
  template: `
<div class="form-group" [formGroup]="sessionGroup">
    <div class="row">
        <div class="col-sm-4">
            
            <div class="input-group">
            
              <input type="text" class="form-control input-box" placeholder="Click to select a date" 
              angular-mydatepicker name="date" (click)="dp.toggleCalendar()" 
              formControlName="date" [options]="myDatePickerOptions" 
              #dp="angular-mydatepicker" (dateChanged)="onDateChanged($event)">


              <span class="input-group-addon" id="basic-addon1" (click)="dp.toggleCalendar()"><i class="fa fa-calendar"></i></span>
          </div>
            
            <small>(Date)</small>
        </div>
        <div class="col-sm-3">
            <timepicker formControlName="starttime" [start]="6" [end]="22"></timepicker>
            <small>(Start Time)</small>
        </div>
        <div class="col-sm-3">
            <timepicker formControlName="endtime" [start]="6" [end]="22"></timepicker>
            <small>(End Time)</small>
        </div>
  </div>
  <div class="row" style="padding: 8px 0;">
    <div class="col-sm-10">
        <input type="text" class="form-control" formControlName="note" />
        <small>(Note)</small>
    </div>
    <div class="col-sm-2">
            <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="position:relative; cursor:pointer; top: -18px;"><i class="fa fa-close"></i></a></span></div>
            <br>
        <small>&nbsp;</small>
    </div>
  </div>
</div>

  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => SessionFormElementComponent),
                  multi: true
                } ,
                {
                  provide: NG_VALIDATORS,
                  useExisting: forwardRef(() => SessionFormElementComponent),
                  multi: true
                }
                ]
})
export class SessionFormElementComponent extends BaseControlValueAccessor<TrainingSession> implements ControlValueAccessor, OnInit { 
    sessionGroup: FormGroup;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();
    public myDatePickerOptions: IAngularMyDpOptions = {
        dateFormat: 'mm/dd/yyyy',
        satHighlight: true,
        firstDayOfWeek: 'su',
    };
    date = new Date();
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      let model: IMyDateModel = {isRange: false, singleDate: {jsDate: this.date}, dateRange: null};
      this.sessionGroup = formBuilder.group({
        date: [model, Validators.required],
        note: [''],
        starttime:[null, Validators.required],
        endtime: [null, Validators.required],
        index:0
      });
  
      this.sessionGroup.valueChanges.subscribe(val => {
        this.value = <TrainingSession>val;
        this.onChange(this.value);
      });
    }
    onRemove(){
        this.removeMe.emit(this.value.index);
    } 

    ngOnInit(){
    }


    onDateChanged($event){
      
    }
    writeValue(session: TrainingSession) {
      this.value = session;
      this.sessionGroup.patchValue(this.value);
    }
    validate(c: AbstractControl): ValidationErrors | null{
      return this.sessionGroup.valid ? null : { invalidForm: {valid: false, message: "Session fields are invalid"}};
    }
}
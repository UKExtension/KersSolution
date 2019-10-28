import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { TrainingSession } from '../training';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";



@Component({
  selector: 'training-session',
  template: `
<div class="form-group" [formGroup]="sessionGroup">
    <div class="row">
        <div class="col-sm-3">
            <my-date-picker [options]="myDatePickerOptions" (dateChanged)="onDateChanged($event)" formControlName="start"></my-date-picker>
            <small>(Date)</small>
        </div>
        <div class="col-sm-3">
            <timepicker></timepicker>
            <small>(Date)</small>
        </div>
    </div>
  <div class="form-inline">
      <div class="form-group">
          <input type="text" class="form-control" formControlName="note" /><br>
          <small>(Source of Income)</small>
      </div>
      &nbsp;
      <div class="form-group">
      
      </div>
      <div class="form-group">
              <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="cursor:pointer;"><i class="fa fa-close"></i></a></span></div>
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
                } 
                ]
})
export class SessionFormElementComponent extends BaseControlValueAccessor<TrainingSession> implements ControlValueAccessor, OnInit { 
    sessionGroup: FormGroup;
    @Input('index') index:number;
    @Output() removeMe = new EventEmitter<number>();
    public myDatePickerOptions: IMyDpOptions = {
        dateFormat: 'mm/dd/yyyy',
        showTodayBtn: false,
        satHighlight: true,
        firstDayOfWeek: 'su',
        showClearDateBtn: false
    };
    date = new Date();
    constructor( 
      private formBuilder: FormBuilder
    )   
    {
      super();
      this.sessionGroup = formBuilder.group({
        start: [{
            date: {
                year: this.date.getFullYear(),
                month: this.date.getMonth() + 1,
                day: this.date.getDate()}
            }, Validators.required],
        note: ['']
      });
  
      this.sessionGroup.valueChanges.subscribe(val => {
        this.value = <TrainingSession>val;
        this.onChange(this.value);
      });
    }
    onRemove(){
        this.removeMe.emit(this.index);
    } 

    ngOnInit(){
        
    }
    writeValue(income: TrainingSession) {
      this.value = income;
      this.sessionGroup.patchValue(income);
    }
}
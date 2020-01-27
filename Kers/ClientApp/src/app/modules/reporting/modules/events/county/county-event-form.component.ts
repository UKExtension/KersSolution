import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormBuilder, Validators, AbstractControl} from '@angular/forms';
import { CountyEvent } from './county-event.service';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { Observable } from 'rxjs';
import { ProgramCategory, ProgramsService } from '../../admin/programs/programs.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';

@Component({
  selector: 'county-event-form',
  template: `
<div class="row">
  <div class="col-sm-offset-3 col-sm-9">
      <h2 *ngIf="!countyEvent">New County Event</h2>
      <h2 *ngIf="countyEvent">Update County Event</h2>
      <br><br>
  </div>
  <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="countyEventForm">
    <div class="form-group">
      <label class="control-label col-md-3 col-sm-3 col-xs-12" for="start">Start Date:</label>
      <div class="col-md-4 col-sm-6 col-xs-7">
          <my-date-picker [options]="myDatePickerOptions" (dateChanged)="onDateChanged($event)" formControlName="start"></my-date-picker>
      </div>
      <div *ngIf="!countyEventForm.value.isAllDay" class="col-md-4 col-md-offset-0 col-sm-6 col-xs-7 col-sm-offset-3"><timepicker formControlName="starttime"></timepicker></div>
    </div>
    <div *ngIf="!countyEventForm.value.isAllDay">
      <div class="form-group">
          <label class="control-label col-md-3 col-sm-3 col-xs-12" for="end">End Date:</label>
          <div class="col-md-4 col-sm-6 col-xs-7">
              <my-date-picker [class.ng-invalid]="countyEventForm.hasError('endDate')" [options]="myDatePickerOptionsEnd" (dateChanged)="onDateChanged($event)" formControlName="end"></my-date-picker>
          </div>
          <div class="col-md-4 col-md-offset-0 col-sm-6 col-xs-7 col-sm-offset-3"><timepicker formControlName="endtime"></timepicker></div>
      </div>
    </div>

    <div class="form-group" >
      <label class="control-label col-md-3 col-sm-3 col-xs-12" for="etimezone"><span *ngIf="!countyEventForm.value.isAllDay">Timezone:</span></label>
      <div *ngIf="!countyEventForm.value.isAllDay" class="col-md-5 col-sm-7 col-xs-8">
              <div class="btn-group" data-toggle="buttons">
                  <label class="btn btn-default" (click)="isEastern(true)" [class.active]="easternTimezone">
                  <input type="radio" name="etimezone" formControlName="etimezone" [value]="true"> Eastern Timezone
                  </label>
                  <label class="btn btn-default" [class.active]="!easternTimezone" (click)="isEastern(false)">
                  <input type="radio" name="etimezone" formControlName="etimezone" [value]="false"> Central Timezone
                  </label>
              </div>
      </div>
      <label>&nbsp; &nbsp;<input type="checkbox" formControlName="isAllDay" /> All Day Event</label>
    </div>
    <div class="form-group">
      <label for="subject" class="control-label col-md-3 col-sm-3 col-xs-12">Title:</label>           
      <div class="col-md-9 col-sm-9 col-xs-12">
          <input type="text" name="subject" formControlName="subject" id="subject" class="form-control col-xs-12" />
      </div>
    </div>
    <div class="form-group">
        <label for="body" class="control-label col-md-3 col-sm-3 col-xs-12">Description:</label>           
        <div class="col-md-9 col-sm-9 col-xs-12" [class.description-invalid]="!countyEventForm.controls.body.valid">
            <textarea [froalaEditor]="options" name="body" formControlName="body" id="body" class="form-control col-xs-12"></textarea>
        </div>
    </div>
    <div class="form-group">
      <label for="webLink" class="control-label col-md-3 col-sm-3 col-xs-12">Url:</label>           
      <div class="col-md-9 col-sm-9 col-xs-12">
          <input type="text" name="webLink" formControlName="webLink" id="webLink" class="form-control col-xs-12" />
          <small> A web address for more information (optional).</small>
      </div>
    </div>
    <div class="form-group" *ngIf="programCategoriesOptions.length > 0">
        <label class="control-label col-md-3 col-sm-3 col-xs-12" for="specialties">Program Category:</label>
        <div class="col-md-9 col-sm-9 col-xs-12">
            <ng-select
                id="programCategories"
                formControlName="programCategories"
                [options]="programCategoriesOptions"
                [multiple]="true"
                placeholder = "(select any/all that apply)">
            </ng-select>
        </div>
    </div>
    <div class="form-group">
      <label for="multycounty" class="control-label col-md-3 col-sm-3 col-xs-12">Multy County Event:</label>           
      <div class="col-md-9 col-sm-9 col-xs-12">
          <label class="switch">
            <input type="checkbox" id="multycounty" formControlName="multycounty">
            <div class="slider round" (click)="onMultyChecked()"></div>
        </label>
      </div>
    </div>


    <div class="form-group" *ngIf="planningUnitsOptions.length > 0 && multycounty">
        <label class="control-label col-md-3 col-sm-3 col-xs-12" for="specialties">Counties:</label>
        <div class="col-md-9 col-sm-9 col-xs-12">
            <ng-select
                id="units"
                formControlName="units"
                [options]="planningUnitsOptions"
                [multiple]="true"
                placeholder = "(select any/all that apply)">
            </ng-select>
            <small>The rule for entering an event is that the event takes place in your county. If this is a multi-county event only the host county should enter the event. As the host county, select from the list above each county (in addition to your county) that is associated with this multi-county event.
        </small>
        </div>
        
    </div>


    <div formGroupName="location">
      <div class="col-md-9 col-sm-9 col-xs-12 col-sm-offset-3">
        <h3>Location</h3>
      </div>
      <div formGroupName="address">
        <div class="form-group">
          <label for="building" class="control-label col-md-3 col-sm-3 col-xs-12">Building:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="building" formControlName="building" id="building" class="form-control col-xs-12" />
          </div>
        </div>
        <div class="form-group">
          <label for="street" class="control-label col-md-3 col-sm-3 col-xs-12">Address:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="street" formControlName="street" id="street" class="form-control col-xs-12" />
          </div>
        </div>
        <div class="form-group">
          <label for="city" class="control-label col-md-3 col-sm-3 col-xs-12">City:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="city" formControlName="city" id="city" class="form-control col-xs-12" />
          </div>
        </div>
        <div class="form-group">
          <label for="postalCode" class="control-label col-md-3 col-sm-3 col-xs-12">Zip:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="postalCode" formControlName="postalCode" id="postalCode" class="form-control col-xs-12" />
          </div>
        </div>
      </div>
    </div>

    <div class="ln_solid"></div>
    <div class="form-group">
        <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
            <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
            <button type="submit" [disabled]="countyEventForm.invalid"  class="btn btn-success">Submit</button>
        </div>
    </div>
      
  </form>
</div>
  `,
  styles: [
    `
    /* The switch - the box around the slider */
    .switch {
      position: relative;
      display: inline-block;
      width: 60px;
      height: 34px;
    }
    
    /* Hide default HTML checkbox */
    .switch input {display:none;}
    
    /* The slider */
    .slider {
      position: absolute;
      cursor: pointer;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: #ccc;
      -webkit-transition: .4s;
      transition: .4s;
    }
    
    .slider:before {
      position: absolute;
      content: "";
      height: 26px;
      width: 26px;
      left: 4px;
      bottom: 4px;
      background-color: white;
      -webkit-transition: .4s;
      transition: .4s;
    }
    
    input:checked + .slider {
      background-color: rgb(38, 185, 154);
      border-color: rgb(38, 185, 154); 
      box-shadow: rgb(38, 185, 154) 
    }
    
    input:focus + .slider {
      box-shadow: 0 0 1px rgb(38, 185, 154);
    }
    
    input:checked + .slider:before {
      -webkit-transform: translateX(26px);
      -ms-transform: translateX(26px);
      transform: translateX(26px);
    }
    
    /* Rounded sliders */
    .slider.round {
      border-radius: 34px;
    }
    
    .slider.round:before {
      border-radius: 50%;
    }
    `
  ]
})
export class CountyEventFormComponent implements OnInit {
  
  @Input() countyEvent:CountyEvent;
  date = new Date();
  countyEventForm:any;
  isAllDay = false;
  easternTimezone = true;
  programCategories: ProgramCategory[];
  planningUnits: PlanningUnit[];
  programCategoriesOptions = Array<any>();
  planningUnitsOptions = Array<any>();
  multycounty = false;

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
    
  defaultTime = "12:34:56.1000000 -04:00";

    
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<CountyEvent>();

  constructor(
    private fb: FormBuilder,
    private programsService: ProgramsService,
    private planningUnitService: PlanningunitService
  ) {
    
    this.countyEventForm = this.fb.group(
        {
          start: [{
            date: {
                year: this.date.getFullYear(),
                month: this.date.getMonth() + 1,
                day: this.date.getDate()}
            }, Validators.required],
          starttime: "",
          end: [{
            year: this.date.getFullYear(),
            month: this.date.getMonth() + 1,
            day: this.date.getDate()}],
          endtime: "",
          etimezone:true,
          isAllDay:false,
          subject:["", Validators.required],
          body:"",
          webLink:"",
          programCategories:'',
          units: '',
          multycounty: false,
          location:this.fb.group(
            {
              address: this.fb.group(
                {
                  building: [""],
                  street: [""],
                  city: [""],
                  postalCode: ""
                }
              )
            }
          ),
          isCancelled: false
            
        }, { validator: trainingValidator }
      );
    
   }

  ngOnInit() {
    if(this.countyEvent){
      this.countyEventForm.patchValue(this.countyEvent);
    }
    this.programsService.categories().subscribe(
      res =>{
        this.programCategories = res;
        var optns = Array<any>();
        this.programCategories.forEach(function(element){
          optns.push(
                {value: element.id, label: element.name}
            );
        });
        this.programCategoriesOptions = optns;
      } 
    );
    this.planningUnitService.counties().subscribe(
      res =>{
        this.planningUnits = res;
        var optns = Array<any>();
        this.planningUnits.forEach(function(element){
          optns.push(
                {value: element.id, label: element.name}
            );
        });
        this.planningUnitsOptions = optns;
      }
    );
  }
  onDateChanged(event: IMyDateModel) {
    
  }
  onMultyChecked(){
    this.multycounty = !this.multycounty;
  }
  isEastern(isIt:boolean){
    this.easternTimezone = isIt;
  }

  onSubmit(){
    console.log(this.countyEventForm.value);
    //this.onFormSubmit.emit(this.countyEventForm.value)
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
}
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { resetComponentState } from '@angular/core/src/render3/state';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import {IMyDpOptions} from 'mydatepicker';
import { ProgramCategory, ProgramsService } from '../admin/programs/programs.service';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExtensionEventLocationConnection } from '../events/location/location.service';
import { Expense, ExpenseFundingSource, ExpenseService } from '../expense/expense.service';
import { Vehicle } from '../expense/vehicle/vehicle.service';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { PlanningUnit, User, UserService } from '../user/user.service';
import { Mileage, MileageSegment } from './mileage';
import { MileageService } from './mileage.service';

@Component({
  selector: 'mileage-form',
  templateUrl: './mileage-form.component.html',
  styles: [`
  .segment-container{
    border: 1px solid #ccc;
    padding: 6px;
    margin-top: 10px;
    margin-bottom: 5px;
    border-radius: 10px;
  }
  
  `]
})
export class MileageFormComponent implements OnInit {


  @Input() mileage:Mileage;
  @Input() mileageDate:Date;
  @Input() isNewCountyVehicle = false;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Mileage>();

  itIsOvernight = false;
  itIsPersonalVehicle = true;
  mileageForm = null;
  enabledVehicles: Vehicle[];
  currentUser:User;
  currentPlanningUnit:PlanningUnit;
  startingLocationBrowser:boolean = false;

  programCategories: ProgramCategory[];
  fundingSources:ExpenseFundingSource[];

  get stLoc(){
    return this.mileageForm.get('startingLocation').value as ExtensionEventLocation;
  }
  set stLoc(lc: ExtensionEventLocation){
    this.mileageForm.patchValue({startingLocation:lc});
  }
  private myDatePickerOptions: IMyDpOptions = {
    // other options...
        dateFormat: 'mm/dd/yyyy',
        showTodayBtn: false,
        satHighlight: true,
        firstDayOfWeek: 'su'
    };
  get segments() {
    return this.mileageForm.get('segments') as FormArray;
  }

  constructor(
    private expenseService:ExpenseService,
    private userService: UserService,
    private fb: FormBuilder,
    private service:MileageService,
    private planningUnitService: PlanningunitService,
    private programsService: ProgramsService
  ) { 

    let date = new Date();
    this.mileageForm = fb.group(
        {
          
          expenseDate: [{
                            date: {
                                year: date.getFullYear(),
                                month: date.getMonth() + 1,
                                day: date.getDate()}
                            }, Validators.required],
          vehicleType:[''],
          countyVehicleId: [''],
          isOvernight: false,
          comment: "",
          startingLocation: null,
          segments: this.fb.array([])
        }, { validator: mileageValidator }
    );
    
    this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
    this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
    this.myDatePickerOptions.editableDateField = false;
    this.myDatePickerOptions.showClearDateBtn = false;
  

  }
  sectionRemoved(event:number){
    if( this.segments.controls.length > 1){
      this.segments.removeAt(event);
    }
  }

  ngOnInit() {
    this.userService.current().subscribe(
      res => {
          this.currentUser = res;
          this.planningUnitService.id(this.currentUser.rprtngProfile.planningUnitId).subscribe(
              res => {
                 this.currentPlanningUnit = res;
                 this.enabledVehicles = this.currentPlanningUnit.vehicles.filter( v => v.enabled);
                 if(this.enabledVehicles.length == 1){
                     this.mileageForm.patchValue({countyVehicleId: this.enabledVehicles[0].id});
                 }
              }
          )
      }
    );
    if(this.mileage == null){
      this.addSegment();
      this.planningUnitService.planningUnitLocation().subscribe(
        res => {
          if(res != null){
            this.stLoc = res;
            
          }else{
            this.startingLocationBrowser = true;
          }
        }
      )
    }else{
      this.mileageForm.patchValue(this.mileage);
      for( let segment of this.mileage.segments){
        this.addSegment(segment);
      }

      let date = new Date(this.mileage.expenseDate);
      this.mileage.expenseDate = date;
      this.isOvernight(this.mileage.isOvernight);
      this.mileageForm.patchValue({expenseDate: {
        date: {
            year: date.getFullYear(),
            month: date.getMonth() + 1,
            day: date.getDate()}
        }});



    }
    this.programsService.categories().subscribe(
      res => this.programCategories = res
    );
    this.expenseService.fundingSources().subscribe(
      res => this.fundingSources = res
    );
  }


  addSegment(segment:MileageSegment = null) {
    var group:FormControl; 

    if(segment == null){
      group = this.fb.control(
        {
          locationId: '',
          programCategoryId: '',
          businessPurpose: '',
          fundingSourceId: '',
          mileage: ''
        }
      );
    }else{
      group = this.fb.control(
        {
          locationId: segment.locationId,
          location: segment.location,
          programCategoryId: segment.programCategoryId,
          businessPurpose: segment.businessPurpose,
          fundingSourceId: segment.fundingSourceId,
          mileage: segment.mileage
        }
      );
    }
    
    this.segments.push(group);
  }
  isOvernight(val:boolean){
    this.itIsOvernight = val;
    this.mileageForm.patchValue({'isOvernight':val});
  }
  isPersonal(val:boolean){
      this.itIsPersonalVehicle = val;
      this.mileageForm.patchValue({'vehicleType':val?1:2});
  }

  locationSelected(event:ExtensionEventLocationConnection){
    this.stLoc = event.extensionEventLocation;
    this.startingLocationBrowser = false;
  }

  onSubmit(){
    var dateValue = this.mileageForm.value.expenseDate.date;
    var d = new Date(Date.UTC(dateValue.year, dateValue.month - 1, dateValue.day, 8, 5, 12));
    var ml = this.mileageForm.value as Mileage;
    ml.expenseDate = d;
    var i = 0;
    for( let s of ml.segments){
      s.order = i++;
    }
    if( !this.mileage ){
      this.service.add(ml).subscribe(
        res => this.onFormSubmit.emit( res )
      )
    }else{
      this.service.update(this.mileage.id, ml).subscribe(
          res => this.onFormSubmit.emit( res )
        )
    }
  }

  onCancel(){
    this.onFormCancel.emit();
  }

}


export const mileageValidator = (control: AbstractControl): {[key: string]: boolean} => {
/* 
  var error = {};
  var hasError = false;

  

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

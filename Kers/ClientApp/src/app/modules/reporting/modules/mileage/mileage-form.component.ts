import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import {IMyDpOptions} from 'mydatepicker';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExtensionEventLocationConnection } from '../events/location/location.service';
import { Vehicle } from '../expense/vehicle/vehicle.service';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { PlanningUnit, User, UserService } from '../user/user.service';
import { Mileage } from './mileage';
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


  @Input() mileage:Mileage = null;
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
  startingLocationBrowser:boolean = true;
  startingLocaiton:ExtensionEventLocation;

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
    private userService: UserService,
    private fb: FormBuilder,
    private service:MileageService,
    private planningUnitService: PlanningunitService
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
          startingLocation: {},
          segments: new FormArray([])
        }, { validator: mileageValidator }
    );
    
    this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
    this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
    this.myDatePickerOptions.editableDateField = false;
    this.myDatePickerOptions.showClearDateBtn = false;





  }
  sectionRemoved(event){

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
    )
    if(this.mileage == null){
      this.addSegment();
    }
  }


  addSegment() {
    const group = this.fb.group(
        {
          locationId: null,
          programCategoryId: null,
          businessPurpose: '',
          fundingSourceId: null,
          mileage: 0
        }
      );
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
    this.startingLocaiton = event.extensionEventLocation;
    this.stLoc = event.extensionEventLocation;
    this.startingLocationBrowser = false;
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

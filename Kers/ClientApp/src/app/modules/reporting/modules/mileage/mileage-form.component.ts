import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { ProgramCategory, ProgramsService } from '../admin/programs/programs.service';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExtensionEventLocationConnection } from '../events/location/location.service';
import { ExpenseFundingSource, ExpenseService } from '../expense/expense.service';
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
  loading = true;

  programCategories: ProgramCategory[];
  fundingSources:ExpenseFundingSource[];

  get isBackButtonAvailable():boolean{
    var formValue = this.mileageForm.value;
    if(formValue.startingLocation != undefined && formValue.segments.length == 1){
      var firstSegment = formValue.segments[0];
      console.log(firstSegment);
      if( firstSegment.locationId != ""
            &&
          firstSegment.programCategoryId != ""
            &&
          firstSegment.businessPurpose != ""
            &&
          (firstSegment.fundingSourceId != "" || !this.itIsPersonalVehicle)
            &&
          firstSegment.mileage != ""
          &&
          firstSegment.mileage != null
        ){
          return true;
      }
    } 
    return false;
  }

  stLocId:number;

  get stLoc(){
    return this.mileageForm.get('startingLocation').value as ExtensionEventLocation;
  }
  set stLoc(lc: ExtensionEventLocation){
    this.mileageForm.patchValue({startingLocation:lc});
  }
  private myDatePickerOptions: IAngularMyDpOptions = {
    // other options...
        dateFormat: 'mm/dd/yyyy',
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

                  isRange: false, singleDate: {jsDate: date}

                            }, Validators.required],
          vehicleType:[''],
          countyVehicleId: [''],
          isOvernight: false,
          comment: "",
          startingLocation: [null, Validators.required],
          segments: this.fb.array([])
        }
    );
    
    this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
    this.myDatePickerOptions.disableUntil = {year: 2020, month: 10, day: 31};
  

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
                this.loading = false;
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
            this.stLocId = res.id;
          }else{
            this.startingLocationBrowser = true;
          }
        }
      );
      if(this.mileageDate != null){
        this.mileageForm.patchValue({expenseDate: {
          isRange: false, singleDate: {jsDate: this.mileageDate}

            }});
      }
      if(this.isNewCountyVehicle){
         this.mileageForm.patchValue({vehicleType: 2});
         this.isPersonal(false);
      }
    }else{
      this.mileageForm.patchValue(this.mileage);
      for( let segment of this.mileage.segments){
        this.addSegment(segment);
      }
      let date = new Date(this.mileage.expenseDate);
      this.mileage.expenseDate = date;
      this.isOvernight(this.mileage.isOvernight);
      this.isPersonal(this.mileage.vehicleType != 2);
      this.mileageForm.patchValue({expenseDate: {
            isRange: false, singleDate: {jsDate: date}
        }});
    }
    this.programsService.categories().subscribe(
      res => this.programCategories = res
    );
    this.expenseService.fundingSources().subscribe(
      res => this.fundingSources = res
    );
  }

  getBack(){
    var formValue = this.mileageForm.value;
    if( formValue.startingLocation != undefined && formValue.segments.length == 1){
      var firstSegment = formValue.segments[0];
      var group:FormControl = this.fb.control(
        {
          locationId: this.stLocId,
          location: this.stLoc,
          programCategoryId: firstSegment.programCategoryId,
          businessPurpose:  firstSegment.businessPurpose,
          fundingSourceId:  firstSegment.fundingSourceId,
          mileage:  firstSegment.mileage
        }
      );
      this.segments.push(group);
    }
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
      if(!val){
        this.mileageForm.get("countyVehicleId").setValidators([Validators.required]);
      }else{
        this.mileageForm.get("countyVehicleId").clearValidators();
      }
      this.mileageForm.get("countyVehicleId").updateValueAndValidity();
  }

  locationSelected(event:ExtensionEventLocationConnection){
    this.stLocId = event.extensionEventLocationId;
    this.stLoc = event.extensionEventLocation;
    this.startingLocationBrowser = false;
  }

  onSubmit(){
    this.loading = true;
    
    var dateValue = this.mileageForm.value.expenseDate.singleDate.jsDate;
    var d = new Date(Date.UTC(dateValue.getFullYear(), dateValue.getMonth(), dateValue.getDate(), 8, 5, 12));
    
    var ml = this.mileageForm.value as Mileage;
    ml.expenseDate = d;
    var i = 0;
    for( let s of ml.segments){
      s.order = i++;
    }
    if( !this.mileage ){
      this.service.add(ml).subscribe(
        res =>{
          this.loading = false;
          this.onFormSubmit.emit( res );
        }
      )
    }else{
      this.service.update(this.mileage.id, ml).subscribe(
          res =>{
            this.loading = false;
            this.onFormSubmit.emit( res );
          } 
        )
    }
  }

  onCancel(){
    this.onFormCancel.emit();
  }

}


import { Component, Output, EventEmitter, Input } from '@angular/core';
import {ExpenseService, ExpenseFundingSource, ExpenseMealRate} from '../expense/expense.service';
import { FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { Observable } from 'rxjs';
import {IMyDpOptions} from 'mydatepicker';
import { ProgramCategory, ProgramsService } from '../admin/programs/programs.service';
import { User, UserService, PlanningUnit } from '../user/user.service';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { Vehicle } from '../expense/vehicle/vehicle.service';
import { Mileage } from './mileage';
import { MileageService } from './mileage.service';


@Component({
    selector: 'expense-compatability-form',
    templateUrl: 'expense-compatability-form.component.html'
})
export class ExpenseCompatabilityFormComponent { 

    @Input() expense:Mileage = null;
    @Input() expenseDate:Date;
    @Input() isNewCountyVehicle = false;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Mileage>();

    fundingSources:Observable<ExpenseFundingSource[]>;
    programCategories: Observable<ProgramCategory[]>;
    loading = false;
    expenseForm = null;
    private myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true,
            firstDayOfWeek: 'su'
        };

    customBrakefastRate = false;
    customLunchRate = false;
    customDinnerRate = false;
    itIsOvernight = false;
    itIsPersonalVehicle = true;

    errorMessage: string;

    currentUser:User;
    currentPlanningUnit:PlanningUnit;
    enabledVehicles: Vehicle[];

    constructor( 
        private fb: FormBuilder,
        private expenseService:ExpenseService,
        private service:MileageService,
        private programsService: ProgramsService,
        private userService: UserService,
        private planningUnitService: PlanningunitService
    )   
    {
        let date = new Date();
        this.expenseForm = fb.group(
            {
              
              expenseDate: [{
                                date: {
                                    year: date.getFullYear(),
                                    month: date.getMonth() + 1,
                                    day: date.getDate()}
                                }, Validators.required],
              vehicleType:[''],
              countyVehicleId: [''],
              startingLocationType: [ 1, Validators.required],
              expenseLocation: ['', Validators.required],
              isOvernight: false,
              programCategoryId: ["", Validators.required],
              businessPurpose: ["", Validators.required],
              fundingSourceNonMileageId: [""],
              fundingSourceMileageId: [""],
              mileage: ["", this.isIntOrFloat],
              registration: ["", this.isIntOrFloat],
              lodging:["", this.isIntOrFloat],
              mealRateBreakfastId:[""],
              mealRateBreakfastCustom:["", this.isIntOrFloat],
              mealRateLunchId:"",
              mealRateLunchCustom:["", this.isIntOrFloat],
              mealRateDinnerId:"",
              mealRateDinnerCustom:["", this.isIntOrFloat],
              otherExpenseCost:["", this.isIntOrFloat],
              otherExpenseExplanation:"",
              departTime: "",
              returnTime: "",
              comment: ""
            }, { validator: expenseValidator }
        );
        
        this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
        this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
        this.myDatePickerOptions.editableDateField = false;
        this.myDatePickerOptions.showClearDateBtn = false;

    }

    ngOnInit(){
         this.fundingSources = this.expenseService.fundingSources();
         this.programCategories = this.programsService.categories();
         if(this.expense != null){
             let date = new Date(this.expense.expenseDate);
             this.expense.expenseDate = date;
             this.expenseForm.patchValue(this.expense);
             this.isOvernight(this.expense.isOvernight);
             this.expenseForm.patchValue({expenseDate: {
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }});
             if(this.expense.departTime != null){
                var a = this.expense.departTime.toString().split(/[^0-9]/);
                let t = a[3]+ ':' + a[4];
                this.expenseForm.patchValue({departTime:t})
             }
             if(this.expense.returnTime != null){
                var a = this.expense.returnTime.toString().split(/[^0-9]/);
                let r = a[3]+ ':' + a[4];
                this.expenseForm.patchValue({returnTime:r})
             }
             if( this.expense.programCategoryId == 0){
                this.expenseForm.patchValue({programCategoryId:""});
             }
             if( this.expense.vehicleType == 2 ){
                this.isPersonal(false);
             }
         }else if(this.expenseDate != null){
            this.expenseForm.patchValue({expenseDate: {
                date: {
                    year: this.expenseDate.getFullYear(),
                    month: this.expenseDate.getMonth() + 1,
                    day: this.expenseDate.getDate()}
                }});
         }else{
             if(this.isNewCountyVehicle){
                this.expenseForm.patchValue({vehicleType: 2});
                this.isPersonal(false);
             }
         }
         this.userService.current().subscribe(
             res => {
                 this.currentUser = res;
                 this.planningUnitService.id(this.currentUser.rprtngProfile.planningUnitId).subscribe(
                     res => {
                        this.currentPlanningUnit = res;
                        this.enabledVehicles = this.currentPlanningUnit.vehicles.filter( v => v.enabled);
                        if(this.enabledVehicles.length == 1){
                            this.expenseForm.patchValue({countyVehicleId: this.enabledVehicles[0].id});
                        }
                     }
                 )
             }
         )
    }


    pad(num:number){
        return ("00" + num).substr(-2,2);
    }

    breakfastChange(event){
        if(event.target.value == "custom"){
            this.customBrakefastRate = true;
        }else{
            this.customBrakefastRate = false;
        }
        
    }

    lunchChange(event){
        if(event.target.value == "custom"){
            this.customLunchRate = true;
        }else{
            this.customLunchRate = false;
        }
        
    }

    dinnerChange(event){
        if(event.target.value == "custom"){
            this.customDinnerRate = true;
        }else{
            this.customDinnerRate = false;
        }
        
    }

    isIntOrFloat(control:FormControl){
        if(control.value == +control.value && +control.value >= 0){
            return null;
        }
        return {"notDigit":true};
    }

    onSubmit(){
        var dateValue = this.expenseForm.value.expenseDate.date;
        var d = new Date(Date.UTC(dateValue.year, dateValue.month - 1, dateValue.day, 8, 5, 12));
        var val = this.expenseForm.value;
        val.expenseDate = d;
        val.departTime = (val.departTime == ""? null : val.departTime );
        val.returnTime = (val.returnTime == ""? null: val.returnTime );
        if(this.itIsOvernight){
            val.mealRateBreakfastId = (val.mealRateBreakfastId == 'custom'? 0 : val.mealRateBreakfastId);
            val.mealRateLunchId = (val.mealRateLunchId == 'custom'? 0 : val.mealRateLunchId);
            val.mealRateDinnerId = (val.mealRateDinnerId == 'custom'? 0 : val.mealRateDinnerId);
        }else{
            val.mealRateBreakfastId = null;
            val.mealRateLunchId = null;
            val.mealRateDinnerId = null;
            val.mealRateBreakfastCustom = null;
            val.mealRateLunchCustom = null;
            val.mealRateBDinnerCustom = null;
        }
        this.loading = true;
        if(this.expense == null){
            this.service.add(val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Mileage>res);
                },
                err => this.errorMessage = <any>err
            );
        }else{
            this.service.update(this.expense.id, val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Mileage>res);
                },
                err => this.errorMessage = <any>err
            );
        }
    }
    onCancel(){
        this.onFormCancel.emit();
    }

    isOvernight(val:boolean){
        this.itIsOvernight = val;
        this.expenseForm.patchValue({'isOvernight':val});
    }
    isPersonal(val:boolean){
        this.itIsPersonalVehicle = val;
        this.expenseForm.patchValue({'vehicleType':val?1:2});
    }


}


export const expenseValidator = (control: AbstractControl): {[key: string]: boolean} => {

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

    return null;
};
import { Component, Output, EventEmitter, Input } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate} from './expense.service';
import { FormBuilder, Validators, FormControl, AbstractControl } from '@angular/forms';
import { Observable } from "rxjs/Observable";
import {IMyDpOptions} from 'mydatepicker';


@Component({
    selector: 'expense-form',
    templateUrl: 'expense-form.component.html'
})
export class ExpenseFormComponent { 

    @Input() expense:Expense = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Expense>();

    fundingSources:Observable<ExpenseFundingSource[]>;
    mealRates:Observable<ExpenseMealRate>;
    loading = false;
    expenseForm = null;
    private myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true
        };

    customBrakefastRate = false;
    customLunchRate = false;
    customDinnerRate = false;

    errorMessage: string;

    constructor( 
        private fb: FormBuilder,
        private expenseService:ExpenseService
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
              expenseLocation: ['', Validators.required],
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
              returnTime: ""
            }, { validator: expenseValidator }
        );
        
        this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
        this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
        this.myDatePickerOptions.editableDateField = false;
        this.myDatePickerOptions.showClearDateBtn = false;

    }

    ngOnInit(){
         this.fundingSources = this.expenseService.fundingSources();
         this.mealRates = this.expenseService.mealRates();
         if(this.expense != null){
             let date = new Date(this.expense.expenseDate);
             this.expense.expenseDate = date;
             this.expenseForm.patchValue(this.expense);
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
             if(this.expense.mealRateBreakfastId == 0 && this.expense.mealRateBreakfastCustom != null){
                 this.expenseForm.patchValue({mealRateBreakfastId:"custom"});
                 this.customBrakefastRate = true;
             }
             if(this.expense.mealRateLunchId == 0 && this.expense.mealRateLunchCustom != null){
                 this.expenseForm.patchValue({mealRateLunchId:"custom"});
                 this.customLunchRate = true;
             }
             if(this.expense.mealRateDinnerId == 0 && this.expense.mealRateDinnerCustom != null){
                 this.expenseForm.patchValue({mealRateDinnerId:"custom"});
                 this.customDinnerRate = true;
             }
         }
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
        /*
        d.setMonth(dateValue.month - 1);
        d.setDate(dateValue.day);
        d.setFullYear(dateValue.year);
        d.setHours(8);
        */
        var val = this.expenseForm.value;
        val.expenseDate = d;
        val.departTime = (val.departTime == ""? null : val.departTime );
        val.returnTime = (val.returnTime == ""? null: val.returnTime );
        val.mealRateBreakfastId = (val.mealRateBreakfastId == 'custom'? 0 : val.mealRateBreakfastId);
        val.mealRateLunchId = (val.mealRateLunchId == 'custom'? 0 : val.mealRateLunchId);
        val.mealRateDinnerId = (val.mealRateDinnerId == 'custom'? 0 : val.mealRateDinnerId);
        this.loading = true;
        if(this.expense == null){
            this.expenseService.add(val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Expense>res);
                },
                err => this.errorMessage = <any>err
            );
        }else{
            this.expenseService.update(this.expense.id, val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Expense>res);
                },
                err => this.errorMessage = <any>err
            );
        }
    }
    onCancel(){
        this.onFormCancel.emit();
    }


}


export const expenseValidator = (control: AbstractControl): {[key: string]: boolean} => {
    


    var isMilgValid = true;

    let mileageAmount = control.get('mileage');
    let mileageFundingSource = control.get('fundingSourceMileageId');
    if(  !( mileageAmount.value == "" ||  mileageAmount.value == 0) && mileageFundingSource.value == ""){
        isMilgValid = false;
        //return { noMileageSource: true };
    }


    var isExpenseValid = true;

    let expenseFundingSource = control.get('fundingSourceNonMileageId');
    
    let registration = control.get('registration');
    let lodging = control.get('lodging');
    let mealRateBreakfast = control.get('mealRateBreakfastId');
    let mealRateLunch = control.get('mealRateLunchId');
    let mealRateDinner = control.get('mealRateDinnerId');
    let otherExpenseCost = control.get('otherExpenseCost');

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
        //return { noExpenseSource: true };
    }


    if(!isExpenseValid && !isMilgValid){
        return { noExpenseSource: true, noMileageSource: true };
    }else if( !isExpenseValid ){
        return { noExpenseSource: true };
    }else if( !isMilgValid ){
        return { noMileageSource: true };
    }


    /*
    const isSnap = control.get('isSnap');
    if (isSnap.value === false) return null;

    let male = control.get('male');
    let female = control.get('female');

    if(parseInt(male.value) + parseInt(female.value) <= 0 ){
        return null;
    }



    let site = control.get('snapClassic').get('direct').get('snapDirectDeliverySiteID');
    let session = control.get('snapClassic').get('direct').get('snapDirectSessionTypeID');
    if(!site.value && !session.value){
        return { nosite: true, nosession: true };
    }else if(!site.value){
        return { nosite: true };
    }else if(!session.value){
        return { nosession: true };
    }

    if(site.value == "1021"){
        let specificSite = control.get('snapClassic').get('direct').get('snapDirectSpecificSiteName');
        if(!specificSite.value){
            return { nospecificSite: true };
        }
    }

    */
    return null;
};
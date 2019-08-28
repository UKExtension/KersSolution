import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { BudgetPlanOfficeOperation, BudgetService, BudgetPlanRevision, BudgetPlanStaffExpenditure } from './budget.service';
import { FormBuilder, Validators, FormArray } from '@angular/forms';
import { UserService, User } from '../user/user.service';

@Component({
  selector: 'budget-plan-form',
  templateUrl: './budget-plan-form.component.html',
  styles: [`
  .form_wizard ul li{
    cursor: pointer;
  }
  .math-sign, .math-result{
    font-weight: bold;
    font-size: 1.5em;
    color: #34495E;
  }
  `]
})
export class BudgetPlanFormComponent implements OnInit {
  step = 1;
  @Input() budget:BudgetPlanRevision;
  budgetForm = this.fb.group(
    { 
        realPropertyAssesment: "",
        realPropertyTaxRate: "",
        personalPropertyAssesment: "",
        personalPropertyTaxRate: "",
        motorVehicleAssesment: "",
        motorVehicleTaxRate: "",
        anticipatedDelinquency: "",
        collection: "",
        otherExtDistTaxes1: "",
        otherExtDistTaxes2: "",
        coGenFund: "",
        userDefinedIncome: this.fb.array([]),
        check: false,
        interest: "",
        reserve: "",
        capitalImpFund: "",
        equipmentFund: "",
        anticipatedCarryover: "",
        budgetPlanStaffExpenditures: this.fb.array([
          {
            personId: "",
            personNameIfNotAUser: "",
            hourlyRate: "",
            hoursPerWeek: "",
            benefitRateInPercents: 50,
            expenditureType: 1,
            index: 0
          },
          {
            personId: "",
            personNameIfNotAUser: "",
            hourlyRate: "",
            hoursPerWeek: "",
            benefitRateInPercents: 9,
            expenditureType: 2,
            index: 1
          },
          {
            personId: "",
            personNameIfNotAUser: "",
            hourlyRate: "",
            hoursPerWeek: "",
            benefitRateInPercents: 38,
            expenditureType: 3,
            index: 2
          },
          {
            personId: "",
            personNameIfNotAUser: "",
            hourlyRate: "",
            hoursPerWeek: "",
            benefitRateInPercents: 0,
            expenditureType: 4,
            index: 3
          },
          {
            personId: "",
            personNameIfNotAUser: "",
            hourlyRate: "",
            hoursPerWeek: "",
            benefitRateInPercents: 9,
            expenditureType: 5,
            index: 4
          },
        ]),
        baseAgentContribution: "",
        travelExpenses: this.fb.array([
          {
            personId: [0],
            personNameIfNotAUser: "",
            staffTypeId: 0,
            amount: "",
            index: 0
          }
        ]),
        professionalImprovemetnExpenses: this.fb.array([]),
        numberOfProfessionalStaff: "",
        amontyPerProfessionalStaff: "",
        additionalOperationalCostPerPerson: "",
        ukPostage: "",
        ukPublications: "",
        capitalImprovementFundForEmergency: "",
        equipmentFundForEmergency: "",
        officeOperationValues: this.fb.array([
          
        ]),
        
    });

  loading = false;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<BudgetPlanOfficeOperation>();

  operations:BudgetPlanOfficeOperation[];
  unitEmployees:User[];
  
  get userDefinedIncomes() {
    return this.budgetForm.get('userDefinedIncome') as FormArray;
  }
  get officeOperationValues() {
    return this.budgetForm.get('officeOperationValues') as FormArray;
  }

  get staffSupportExpenditures() {
    return this.budgetForm.get('budgetPlanStaffExpenditures') as FormArray;
  }
  get staffSupportExpenditures1(){
    var filtered = this.staffSupportExpenditures.controls.filter( e => e.value.expenditureType == 1);
    return filtered;
  }
  get staffSupportExpenditures2(){
    var filtered = this.staffSupportExpenditures.controls.filter( e => e.value.expenditureType == 2);
    return filtered;
  }
  get staffSupportExpenditures3(){
    var filtered = this.staffSupportExpenditures.controls.filter( e => e.value.expenditureType == 3);
    return filtered;
  }
  get staffSupportExpenditures4(){
    var filtered = this.staffSupportExpenditures.controls.filter( e => e.value.expenditureType == 4);
    return filtered;
  }
  get staffSupportExpenditures5(){
    var filtered = this.staffSupportExpenditures.controls.filter( e => e.value.expenditureType == 5);
    return filtered;
  }
  get travelExpenses(){
    return this.budgetForm.get('travelExpenses') as FormArray;
  }
  get professionalImprovemetnExpenses(){
    return this.budgetForm.get('professionalImprovemetnExpenses') as FormArray;
  }

  supportStaffIndex = 5;

  constructor(
    private fb: FormBuilder,
    private service:BudgetService,
    private userService: UserService
  ) {
    
  }

  ngOnInit() {
    this.service.getOfficeOperations().subscribe(
      res => {
        let control = <FormArray>this.budgetForm.controls.officeOperationValues;
        for( let val of res ){
          control.push(this.fb.group({
            budgetPlanOfficeOperation: val,
            value: 0
          }));
        }
        this.operations = res;
      }
    );
    this.userService.unitEmployees().subscribe(
      res => this.unitEmployees = res
    )
  }

  onCancel(){
    this.onFormCancel.emit();
  }

  onSubmit(){
    console.log(this.budgetForm.value);
  }
  addUserDefinedIncome() {
    this.userDefinedIncomes.push(this.fb.control({
      name: "",
      value: ""
    }));
  }
  addSupportStaffExpenditure(rate:number = 50, type:number = 1) {
    this.staffSupportExpenditures.push(this.fb.control({
      personId: 0,
      personNameIfNotAUser: "",
      hourlyRate: "",
      hoursPerWeek: "",
      benefitRateInPercents: rate,
      expenditureType: type,
      index: this.supportStaffIndex
    }));
    this.supportStaffIndex++;
  }
  addTravelExpenditure(){
    this.travelExpenses.push(this.fb.control({
      personId: [0],
      personNameIfNotAUser: "",
      staffTypeId: 0,
      amount: "",
      index: 0
    }));
  }
  

  /**************************/
  // Calculations
  /**************************/
  assesmentAndTaxRate( assesmentName:string, taxRateName:string):number{
    var vl = +this.budgetForm.get(assesmentName).value/100 * +this.budgetForm.get(taxRateName).value/100;
    return vl;
  }
  realProperty():number{
    var vl = this.assesmentAndTaxRate( "realPropertyAssesment", "realPropertyTaxRate");
    return vl;
  }
  personalProperty():number{
    var vl = this.assesmentAndTaxRate( "personalPropertyAssesment", "personalPropertyTaxRate");
    return vl;
  }
  vehicleProperty():number{
    var vl = this.assesmentAndTaxRate( "motorVehicleAssesment", "motorVehicleTaxRate");
    return vl;
  }
  totalGrossIncome(){
    return this.realProperty() + this.personalProperty() + this.vehicleProperty();
  }
  withDelinquency(){
    var grossIncome = this.totalGrossIncome();
    return grossIncome - (+this.budgetForm.get("anticipatedDelinquency").value / 100 ) * grossIncome;
  }

  subtotalGrossIncome(){
    var grossIncome = this.withDelinquency();
    return grossIncome - (+this.budgetForm.get("collection").value / 100 ) * grossIncome;
  }
  supportStaffRowPerson(row:BudgetPlanStaffExpenditure):string{
    if(row.personId == null) return row.personNameIfNotAUser;
    if(row.personId == 0) return "";
    var person = this.unitEmployees.filter( u => u.id == row.personId);
    if(person.length > 0){
      return person[0].personalProfile.firstName + " " + person[0].personalProfile.lastName;
    }
    return "";
  }
  supportStaffRowSum(row:BudgetPlanStaffExpenditure):number{
    return row.hourlyRate * row.hoursPerWeek + row.hourlyRate * row.hoursPerWeek * (row.benefitRateInPercents / 100);
  }
  supportStaffTotalSalaries():number{
    var total = 0;
    for(var row of this.budgetForm.value.budgetPlanStaffExpenditures){
      total += row.hourlyRate * row.hoursPerWeek;
    }
    return total;
  }
  supportStaffTotalBenefits():number{
    var total = 0;
    for(var row of this.budgetForm.value.budgetPlanStaffExpenditures){
      total += row.hourlyRate * row.hoursPerWeek * (row.benefitRateInPercents / 100);
    }
    return total;
  }
  supportStaffTotalAmount():number{
    var total = 0;
    for(var row of this.budgetForm.value.budgetPlanStaffExpenditures){
      total += row.hourlyRate * row.hoursPerWeek + row.hourlyRate * row.hoursPerWeek * (row.benefitRateInPercents / 100);
    }
    return total;
  }

  /***************************/
  // Events
  /***************************/

  userDefinedIncomeRemoved(event:number){
    this.userDefinedIncomes.removeAt(event);
  }

  supportStaffExpenditureRemoved(event:number){
    var elementIndex:number = undefined;
    this.staffSupportExpenditures.controls.forEach( (item, index) => {
      if(item.value.index == event){
        elementIndex = index;
      }else if(elementIndex != undefined){
        item.value.index--;
      }
    });
    if(elementIndex != undefined){
      this.staffSupportExpenditures.removeAt(elementIndex);
      this.supportStaffIndex--;
    } 
  }


}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { BudgetPlanOfficeOperation, BudgetService, BudgetPlanRevision } from './budget.service';
import { FormBuilder, Validators, FormArray } from '@angular/forms';

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
        userDefinedIncome: this.fb.array([
          
        ]),
        interest: "",
        reserve: "",
        capitalImpFund: "",
        equipmentFund: "",
        anticipatedCarryover: "",
        budgetPlanStaffExpenditures: this.fb.array([
          
        ]),
        baseAgentContribution: "",
        travelExpenses: this.fb.array([
          
        ]),
        professionalImprovemetnExpenses: this.fb.array([
          
        ]),
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

  operations:Observable<BudgetPlanOfficeOperation[]>;
  
  get userDefinedIncomes() {
    return this.budgetForm.get('userDefinedIncome') as FormArray;
  }

  constructor(
    private fb: FormBuilder,
    private service:BudgetService
  ) {
    
  }

  ngOnInit() {
    this.operations = this.service.getOfficeOperations();
  }

  onCancel(){
    this.onFormCancel.emit();
  }

  onSubmit(){
    console.log(this.budgetForm.value);
  }
  addUserDefinedIncome() {
    this.userDefinedIncomes.push(this.fb.control(''));
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

  /***************************/
  // Events
  /***************************/

  userDefinedIncomeRemoved(event:number){
    this.userDefinedIncomes.removeAt(event);
  }


}

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
        interest: "",
        reserve: "",
        capitalImpFund: "",
        equipmentFund: "",
        anticipatedCarryover: "",
        budgetPlanStaffExpenditures: [
          [
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
          ]
        ],
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
        officeOperationValues: [[
          {
            budgetPlanOfficeOperationId: 2,
            value: 2
          },
          {
            budgetPlanOfficeOperationId: 3,
            value: 3
          },
          {
            budgetPlanOfficeOperationId: 4,
            value: 4
          },
        ]],
        
    });

  loading = false;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<BudgetPlanOfficeOperation>();

  unitEmployees:User[];
 
  
  get travelExpenses(){
    return this.budgetForm.get('travelExpenses') as FormArray;
  }
  get professionalImprovemetnExpenses(){
    return this.budgetForm.get('professionalImprovemetnExpenses') as FormArray;
  }

  

  constructor(
    private fb: FormBuilder,
    private service:BudgetService,
    private userService: UserService
  ) {
    
  }

  ngOnInit() {
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
  
  
  addTravelExpenditure(){
    this.travelExpenses.push(this.fb.control({
      personId: [0],
      personNameIfNotAUser: "",
      staffTypeId: 0,
      amount: "",
      index: 0
    }));
  }

  increaseStep(){
    this.setStep( this.step + 1);
  }
  decreaseStep(){
    this.setStep( this.step - 1);
  }
  setStep(step:number){
    this.step = step;
  }
  






}

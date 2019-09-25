import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormBuilder, FormControl, FormGroup, FormArray, AbstractControl } from '@angular/forms';
import { BaseControlValueAccessor } from '../../../core/BaseControlValueAccessor';
import { BudgetPlanOfficeOperationValue, BudgetService, BudgetPlanOfficeOperation, BudgetPlanStaffExpenditure } from '../budget.service';
import { UserService, User } from '../../user/user.service';



@Component({
  selector: 'personel-form',
  template: `
  <div [formGroup]="personel">
    <h2>Professional Staff
    Support Staff & Base Program Support</h2><br><br>


    <div class="form-group" formArrayName="budgetPlanStaffExpenditures">
      <div *ngFor="let type of types">
        <h3>{{type.label}}</h3><br>
        <div class="col-xs-12">
            <div *ngFor="let userDefinedIncome of this['staffSupportExpenditures'+type.id]; let i=index" class="form-group">
                <budget-personel-support-staff [employees]="unitEmployees" (removeMe)="supportStaffExpenditureRemoved($event)" [index]="userDefinedIncome.value.index" [formControlName]="userDefinedIncome.value.index"></budget-personel-support-staff>
            </div>
            <a class="btn btn-info btn-xs" (click)="addSupportStaffExpenditure(type.benefitsRate, type.id)" style="margin-top: 8px;"> + add </a>
        </div>
      <div class="clear"><br><br></div>
    </div>
    <div class="form-group">
      <label class="control-label col-md-4 col-sm-6 col-xs-12" for="first-name">Base Agent Contribution (see guidelines)
      </label>
      <div class="col-md-8 col-sm-6 col-xs-12">
            <div class="row">
                <div class="col-sm-10">
                    <select class="form-control">
                      <option value="0">0</option>
                      <option value="3500">3,500</option>
                      <option value="58200">58,200</option>
                      <option value="68400">68,400</option>
                      <option value="78600">78,600</option>
                      <option value="89700">89,700</option>
                      <option value="99300">99,300</option>
                    </select>
                </div>
            </div>
      </div>
    </div>
  </div>
  <table class="table table-bordered" style="background-color:rgba(233, 226, 122, 0.3);">
      <thead>
          <tr>
              <th>Person</th>
              <th>Salary</th>
              <th>Benefits</th>
              <th>Amount</th>
          </tr>
      </thead>
      <tbody>
        <ng-container *ngFor="let type of types">
          <tr>
            <td colspan="4"><strong>{{type.label}}</strong></td>
          </tr>
          <tr *ngFor="let row of this['staffSupportExpenditures'+type.id]">
            <td>{{supportStaffRowPerson(row.value)}}</td>
            <td class="text-right">{{row.value.hourlyRate * row.value.hoursPerWeek | currency:'USD':'symbol':'.2-2'}}</td>
            <td class="text-right">{{row.value.hourlyRate * row.value.hoursPerWeek * (row.value.benefitRateInPercents / 100) | currency:'USD':'symbol':'.2-2'}}</td>
            <td class="text-right">{{supportStaffRowSum(row.value) | currency:'USD':'symbol':'.2-2'}}</td>
          </tr>
        </ng-container>
              <tr>
                <td></td>  
              </tr>
      </tbody>
      <tfoot>
          <tr>
              <td><strong>Total All Salaries and Benefits</strong></td>
              <td class="text-right"><strong>{{supportStaffTotalSalaries() | currency:'USD':'symbol':'.2-2'}}</strong></td>
              <td class="text-right"><strong>{{supportStaffTotalBenefits() | currency:'USD':'symbol':'.2-2'}}</strong></td>
              <td class="text-right"><strong>{{supportStaffTotalAmount() | currency:'USD':'symbol':'.2-2'}}</strong></td>
          </tr>
      </tfoot>
    </table>
  </div>


  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => PersonelFormComponent),
                  multi: true
                } 
                ]
})
export class PersonelFormComponent extends BaseControlValueAccessor<BudgetPlanStaffExpenditure[]> implements ControlValueAccessor, OnInit { 
    personel: FormGroup;
    writtenValues:BudgetPlanStaffExpenditure[];
    operations:BudgetPlanStaffExpenditure[];
    types:PersonalTypes[];

    get staffSupportExpenditures() {
      return this.personel.get('budgetPlanStaffExpenditures') as FormArray;
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

    supportStaffIndex = 0;
    unitEmployees:User[];
    
    constructor( 
      private formBuilder: FormBuilder,
      private service:BudgetService,
      private userService: UserService
    )   
    {
      super();
      this.personel = formBuilder.group(
        {
          budgetPlanStaffExpenditures: formBuilder.array([]),
          baseAgentContribution: ["0"]
        }
      )
  
      this.personel.valueChanges.subscribe(val => {
        this.value = val.officeOperationValues;
        this.onChange(this.value);
      });

      
    }
    

    ngOnInit(){
      this.userService.unitEmployees().subscribe(
        res => this.unitEmployees = res
      ); 
      
    }

    writeValue(operationValues: BudgetPlanStaffExpenditure[]) {
      this.value = operationValues;
      this.writtenValues = operationValues;
      for( var exp of operationValues){
        this.staffSupportExpenditures.push(this.formBuilder.control(exp));
      }
      this.supportStaffIndex = this.personel.get("budgetPlanStaffExpenditures").value.length;
      this.types = [
        <PersonalTypes>{
          id: 1,
          benefitsRate: 50,
          label: "Support Personnel at Benefit Rate of 50%"
        },
        <PersonalTypes>{
          id: 2,
          benefitsRate: 9,
          label: "Support Part-Time/Temp Personnel at Benefit Rate of 9%"
        },
        <PersonalTypes>{
          id: 3,
          benefitsRate: 38,
          label: "Agent Positions 4th and Up at Benefit Rate of 38%"
        },
        <PersonalTypes>{
          id: 4,
          benefitsRate: 0,
          label: "Temporarily County Funded Agent Positions"
        },
        <PersonalTypes>{
          id: 5,
          benefitsRate: 9,
          label: "Other at Benefit Rate of 9%"
        }
      ];
    }


    addSupportStaffExpenditure(rate:number = 50, type:number = 1) {
      this.staffSupportExpenditures.push(this.formBuilder.control({
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
    supportStaffExpenditureRemoved(event:number){
      var elementIndex:number = undefined;
      this.staffSupportExpenditures.controls.forEach( (item, index) => {
        if(item.value.index == event){
          elementIndex = index;
        }
      });
      if(elementIndex != undefined){
        this.staffSupportExpenditures.removeAt(elementIndex);
        this.supportStaffIndex--;
        this.staffSupportExpenditures.controls.forEach( (item, index) => {
          if( item.value.index > elementIndex) item.value.index--;
        });
      } 
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
      for(var row of this.personel.value.budgetPlanStaffExpenditures){
        total += row.hourlyRate * row.hoursPerWeek;
      }
      return total;
    }
    supportStaffTotalBenefits():number{
      var total = 0;
      for(var row of this.personel.value.budgetPlanStaffExpenditures){
        total += row.hourlyRate * row.hoursPerWeek * (row.benefitRateInPercents / 100);
      }
      return total;
    }
    supportStaffTotalAmount():number{
      var total = 0;
      for(var row of this.personel.value.budgetPlanStaffExpenditures){
        total += row.hourlyRate * row.hoursPerWeek + row.hourlyRate * row.hoursPerWeek * (row.benefitRateInPercents / 100);
      }
      return total;
    }    
}

class PersonalTypes{
  id:number;
  benefitsRate:number;
  label:string;
}
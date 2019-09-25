import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormArray, FormBuilder } from '@angular/forms';

@Component({
  selector: 'revenue-form-component',
  templateUrl: './revenue-form-component.component.html',
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
export class RevenueFormComponentComponent implements OnInit {
  @Input() budgetForm:FormGroup;

  get userDefinedIncomes() {
    return this.budgetForm.get('userDefinedIncome') as FormArray;
  }

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
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
    return grossIncome - this.delinquency;
  }

  subtotalGrossIncome():number{
    var grossIncome = this.withDelinquency();
    return grossIncome - this.collection;
  }

  get grandTotal(){
    var total =  this.subtotalGrossIncome();
    total += +this.budgetForm.get("otherExtDistTaxes1").value;
    total += +this.budgetForm.get("otherExtDistTaxes2").value;
    total += +this.budgetForm.get("coGenFund").value;
    total += +this.budgetForm.get("interest").value;
    for(var ud of this.userDefinedIncomes.controls){
      total += +ud.value.value;
    }
    return total;
  }
  get delinquency():number{
    return (+this.budgetForm.get("anticipatedDelinquency").value / 100 ) * this.totalGrossIncome();
  }
  get collection():number{
    return (+this.budgetForm.get("collection").value / 100 ) * this.withDelinquency();
  }
  



  addUserDefinedIncome() {
    this.userDefinedIncomes.push(this.fb.control({
      name: "",
      value: ""
    }));
  }


  userDefinedIncomeRemoved(event:number){
    this.userDefinedIncomes.removeAt(event);
  }

}

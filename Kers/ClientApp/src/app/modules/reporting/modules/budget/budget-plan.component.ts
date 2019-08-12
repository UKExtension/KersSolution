import { Component, OnInit } from '@angular/core';
import {BudgetService, BudgetPlanOfficeOperation} from './budget.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-budget-plan',
  templateUrl: './budget-plan.component.html',
  styles: [`
  .form_wizard ul li{
    cursor: pointer;
  }
  `]
})
export class BudgetPlanComponent implements OnInit {


  step = 1;


  operations:Observable<BudgetPlanOfficeOperation[]>;

  constructor(
    private service:BudgetService
  ) { 

  }

  ngOnInit() {
    this.operations = this.service.getOfficeOperations();
  }

}

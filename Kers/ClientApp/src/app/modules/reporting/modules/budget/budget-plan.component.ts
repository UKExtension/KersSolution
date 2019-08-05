import { Component, OnInit } from '@angular/core';

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

  constructor() { }

  ngOnInit() {
  }

}

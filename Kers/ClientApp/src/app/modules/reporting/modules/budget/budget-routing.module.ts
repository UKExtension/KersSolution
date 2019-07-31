import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BudgetHomeComponent } from './budget-home.component';
import { BudgetPlanComponent } from './budget-plan.component';

const routes: Routes = [{
  path: '',
  component: BudgetHomeComponent,
  children: 
    [
        {
            path: 'plan',
            component: BudgetPlanComponent
          },
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BudgetRoutingModule { }

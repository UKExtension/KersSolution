import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BudgetHomeComponent } from './budget-home.component';
import { BudgetPlanComponent } from './budget-plan.component';
import { BudgetAuthGuard } from './auth/budget-auth.guard';
import { BudgetPlanAdminComponent } from './admin/budget-plan-admin.component';
import { BudgetPlanOfficeOperationsComponent } from './admin/budget-plan-office-operations.component';

const routes: Routes = [{
  path: '',
  component: BudgetHomeComponent,
  children: 
    [
          {
            path: 'plan',
            component: BudgetPlanComponent
          },
          {
            path: 'admin',
              component: BudgetPlanAdminComponent,
              canActivate: [BudgetAuthGuard],
              children: [
                {
                  path: '',
                  canActivateChild: [BudgetAuthGuard],
                  children: [
                    { path: 'operations', component: BudgetPlanOfficeOperationsComponent },
                  ]
                }
              ]
          }
          
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BudgetRoutingModule { }

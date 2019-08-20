import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { BudgetRoutingModule } from './budget-routing.module';
import { BudgetHomeComponent } from './budget-home.component';
import { BudgetPlanComponent } from './budget-plan.component';
import { BudgetPlanOfficeOperationsComponent } from './admin/budget-plan-office-operations.component';
import { BudgetPlanAdminComponent } from './admin/budget-plan-admin.component';
import { BudgetPlanOfficeOperationsForm } from './admin/budget-plan-office-operations-form.component';
import { BudgetPlanOfficeOperationsDetailComponent } from './admin/budget-plan-office-operations-detail.component';
import { BudgetSupportStaffComponent } from './form-element/budget-support-staff.component';
import { BudgetPlanFormComponent } from './budget-plan-form.component';
import {BudgetUserDefinedIncomeComponent} from './form-element/budget-user-defined-income.component';




@NgModule({
  declarations: [
    
    BudgetHomeComponent,
      
    BudgetPlanComponent,
      
    BudgetPlanOfficeOperationsComponent,
    BudgetPlanOfficeOperationsForm,
    
    BudgetPlanAdminComponent,
      
    BudgetPlanOfficeOperationsDetailComponent,
    BudgetSupportStaffComponent,
    BudgetPlanFormComponent,
    BudgetUserDefinedIncomeComponent
  
  
  
  ],
  imports: [
    SharedModule,
    CommonModule,
    BudgetRoutingModule
  ],
  entryComponents: [BudgetHomeComponent, BudgetPlanComponent]
})
export class BudgetModule { }

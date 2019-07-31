import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { BudgetRoutingModule } from './budget-routing.module';
import { BudgetHomeComponent } from './budget-home.component';
import { BudgetPlanComponent } from './budget-plan.component';




@NgModule({
  declarations: [
    
  BudgetHomeComponent,
    
  BudgetPlanComponent],
  imports: [
    SharedModule,
    CommonModule,
    BudgetRoutingModule
  ],
  entryComponents: [BudgetHomeComponent, BudgetPlanComponent]
})
export class BudgetModule { }

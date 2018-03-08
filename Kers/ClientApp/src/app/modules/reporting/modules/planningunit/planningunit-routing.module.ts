import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PlanningunitListComponent } from './planningunit-list.component';
import { PlanningUnitAdminHomeComponent } from './admin/planning-unit-admin-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     {
         
          path: '',
          component: PlanningunitListComponent
      },
      {
        path: 'admin',
        component: PlanningUnitAdminHomeComponent
      }
             
  ])],
  exports: [ RouterModule ]
})
export class PlanningunitRoutingModule {}

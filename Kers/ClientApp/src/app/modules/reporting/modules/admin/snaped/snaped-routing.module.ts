import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SnapedHomeComponent } from './snaped-home.component';
import { SnapedComponent } from './snaped.component';
import { SnapedCountyComponent } from './snaped-county.component';
import { SnapedUserComponent } from './snaped-user.component';
import { SnapedBudgetHomeComponent } from './snaped-budget-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: SnapedComponent,
          children: 
            [
                
                {
                  path: '',
                  component: SnapedHomeComponent
                },
                {
                  path: 'budget',
                  component: SnapedBudgetHomeComponent
                },
                {
                  path: 'county/:id',
                  component: SnapedCountyComponent

                },
                {
                  path: 'user/:id',
                  component: SnapedUserComponent
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class SnapedRoutingModule {}

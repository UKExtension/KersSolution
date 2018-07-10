import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {PlansofworkHomeComponent} from './plansofwork-home.component';
import {PlansofworkReportsComponent} from './plansofwork-reports.component';


@NgModule({
  imports: [RouterModule.forChild([
      {
          path: '',          
          children: [
            {
              path: '',
              component: PlansofworkHomeComponent
            },
            {
                path: 'reports',
                component: PlansofworkReportsComponent,
            }
          ]

      }           
  ])],
  exports: [RouterModule]
})
export class PlansofworkRoutingModule {}

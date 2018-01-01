import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AffirmativeHomeComponent} from './affirmative-home.component';
import {AffirmativeReportsHomeComponent} from './reports/affirmative-reports-home.component';


@NgModule({
  imports: [RouterModule.forChild([
      {
          path: '',
          component: AffirmativeHomeComponent,
          /*
          children: [
            {
              path: '',
              component: UsersListComponent
            },
            {
                path: 'user/:id',
                component: UserComponent,
            }  
          ]
          */
      },
      {
          path: 'reports',
          component: AffirmativeReportsHomeComponent
      }           
  ])],
  exports: [RouterModule]
})
export class AffirmativeRoutingModule {}
